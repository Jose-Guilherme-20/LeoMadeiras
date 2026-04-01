using System.Reflection;
using System.Text;
using LeoMadeiras.API.Middlewares;
using LeoMadeiras.Application.UseCases.Auth.Login;
using LeoMadeiras.Application.UseCases.Auth.RegistrarUsuario;
using LeoMadeiras.Application.UseCases.Produtos.AtualizarProduto;
using LeoMadeiras.Application.UseCases.Produtos.BuscarProduto;
using LeoMadeiras.Application.UseCases.Produtos.CriarProduto;
using LeoMadeiras.Application.UseCases.Produtos.DeletarProduto;
using LeoMadeiras.Application.UseCases.Produtos.ListarProdutos;
using LeoMadeiras.Application.UseCases.Produtos.MaisVendidos;
using LeoMadeiras.Application.UseCases.Vendas.RegistrarVenda;
using LeoMadeiras.Infrastructure.Data;
using LeoMadeiras.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ───────────────────────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// ─── Controllers ───────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─── Swagger ───────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LeoMadeiras API",
        Version = "v1",
        Description = "API REST para gerenciamento de produtos e registro de vendas."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT assim: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ─── JWT ───────────────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurado.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ─── Infrastructure (DbContext, Repositories, UnitOfWork) ──────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ─── Use Cases ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ICriarProdutoUseCase, CriarProdutoUseCase>();
builder.Services.AddScoped<IListarProdutosUseCase, ListarProdutosUseCase>();
builder.Services.AddScoped<IBuscarProdutoUseCase, BuscarProdutoUseCase>();
builder.Services.AddScoped<IAtualizarProdutoUseCase, AtualizarProdutoUseCase>();
builder.Services.AddScoped<IDeletarProdutoUseCase, DeletarProdutoUseCase>();
builder.Services.AddScoped<IMaisVendidosUseCase, MaisVendidosUseCase>();
builder.Services.AddScoped<IRegistrarVendaUseCase, RegistrarVendaUseCase>();
builder.Services.AddScoped<IRegistrarUsuarioUseCase, RegistrarUsuarioUseCase>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
// ─── Build ─────────────────────────────────────────────────────────────────
var app = builder.Build();

// ─── Migrations automáticas ────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// ─── Pipeline ──────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LeoMadeiras API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseSerilogRequestLogging();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { }