using LeoMadeiras.Domain.Exceptions;

namespace LeoMadeiras.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("NotFoundException: {Message}", ex.Message);
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                await ctx.Response.WriteAsJsonAsync(new { erro = ex.Message });
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("DomainException: {Message}", ex.Message);
                ctx.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                await ctx.Response.WriteAsJsonAsync(new { erro = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado");
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new { erro = "Erro interno no servidor." });
            }
        }
    }
}
