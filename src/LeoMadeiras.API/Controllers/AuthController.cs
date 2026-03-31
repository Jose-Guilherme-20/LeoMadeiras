//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using LeoMadeiras.API.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;

//namespace LeoMadeiras.API.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//[Produces("application/json")]
//public class AuthController : ControllerBase
//{
//    /// <summary>Gera token JWT para autenticação.</summary>
//    [HttpPost("login")]
//    [AllowAnonymous]
//    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//    public IActionResult Login(
//        [FromBody] LoginRequest request,
//        [FromServices] IConfiguration config)
//    {
//        if (request.Username != "admin" || request.Senha != "admin123")
//            return Unauthorized(new { erro = "Credenciais inválidas." });

//        var token = GerarToken(request.Username, config);
//        return Ok(new TokenResponse(token));
//    }

//    private static string GerarToken(string username, IConfiguration config)
//    {
//        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
//        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//        var claims = new[]
//        {
//            new Claim(ClaimTypes.Name, username),
//            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//        };

//        var token = new JwtSecurityToken(
//            issuer: config["Jwt:Issuer"],
//            audience: config["Jwt:Audience"],
//            claims: claims,
//            expires: DateTime.UtcNow.AddHours(
//                                    int.Parse(config["Jwt:ExpiresInHours"] ?? "8")),
//            signingCredentials: creds
//        );

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}