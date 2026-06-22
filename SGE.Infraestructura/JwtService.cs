using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SGE.Aplicacion.Comun;

//Chequear
namespace SGE.Infraestructura;

public class JwtService(JwtSettings settings) : IJwtService
{
    public string GenerarToken(Guid userId)
    {
        // El claim NameIdentifier es el que .NET va a leer automáticamente como User.FindFirstValue(ClaimTypes.NameIdentifier) en los endpoints de la WebApi.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpiracionMinutos),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}