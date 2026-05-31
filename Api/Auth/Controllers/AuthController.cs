using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Auth.Dtos;
using Api.Common.Controllers;
using Application.Abstractions;
using Application.Common.Security;
using Domain.ValueObjects.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Api.Auth.Controllers;

[AllowAnonymous]
public sealed class AuthController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthController(IUnitOfWork uow, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var usuario = await _uow.Usuarios.GetByCorreoAsync(CorreoUsuario.Create(request.Correo), cancellationToken);
        if (usuario is null || !usuario.Activo || !_passwordHasher.Verify(request.Contrasena, usuario.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciales invalidas." });
        }

        if (usuario.Rol is null)
        {
            return Conflict(new { message = "El usuario no tiene un rol cargado." });
        }

        var rol = NormalizeRole(usuario.Rol.Nombre.Value);
        var expiraEn = DateTime.UtcNow.AddMinutes(GetExpirationMinutes());
        var token = CreateToken(usuario, rol, expiraEn);

        return Ok(new LoginResponse(
            token,
            expiraEn,
            new UsuarioAutenticadoDto(
                usuario.Id,
                usuario.Correo.Value,
                usuario.Nombre.Value,
                rol)));
    }

    private string CreateToken(Domain.Entities.Usuario usuario, string rol, DateTime expiraEn)
    {
        var jwt = _configuration.GetSection("Jwt");
        var key = jwt["Key"] ?? throw new InvalidOperationException("La clave JWT no esta configurada.");
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo.Value),
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo.Value),
            new Claim(ClaimTypes.Name, usuario.Nombre.Value),
            new Claim(ClaimTypes.Role, rol)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiraEn,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetExpirationMinutes()
    {
        return int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutes)
            ? minutes
            : 120;
    }

    private static string NormalizeRole(string value)
    {
        var normalized = value.Trim();

        if (string.Equals(normalized, RoleNames.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return RoleNames.Admin;
        }

        if (string.Equals(normalized, RoleNames.Mecanico, StringComparison.OrdinalIgnoreCase))
        {
            return RoleNames.Mecanico;
        }

        if (string.Equals(normalized, RoleNames.Recepcionista, StringComparison.OrdinalIgnoreCase))
        {
            return RoleNames.Recepcionista;
        }

        return normalized;
    }
}
