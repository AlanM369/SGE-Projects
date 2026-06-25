using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SGE.Aplicacion.Comun;


namespace SGE.Infraestructura.Seguridad;

// Creación y firma de tokens de autenticación bajo el estándar JSON Web Token (JWT).
public class JwtService(JwtSettings settings) : IJwtService
{
    public string GenerarToken(Guid userId)
    {
        // 1. Definición de la identidad básica del usuario mediante claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        // 2. Configuración de las credenciales criptográficas utilizando cifrado simétrico HMAC-SHA256
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        // 3. Construcción del objeto token con los parámetros de emisor, audiencia y tiempo de expiración
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpiracionMinutos),
            signingCredentials: credenciales
        );

        // 4. Serialización del token a su representación en formato de cadena de texto
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}