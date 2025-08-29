using Microsoft.AspNetCore.Mvc;
using BlogApi.Models;
using BlogApi.Data;
using BlogApi.DTOs;
using BlogApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly BlogDbContext _context;
    private readonly AuthService _authService;

    public UsersController(BlogDbContext context, AuthService authService)
    {
        _authService = authService;
        _context = context;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email já cadastrado");

        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
           password: dto.Password,
           salt: new byte[128 / 8],
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        var user = new User { Username = dto.Username, Email = dto.Email, PasswordHash = passwordHash };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Usuário cadastrado com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Unauthorized("Usuário ou senha invalidos");
    }

    var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
         password: dto.Password,
           salt: new byte[128 / 8],
           prf: KeyDerivationPrf.HMACSHA256,
           iterationCount: 10000,
           numBytesRequested: 256 / 8)
        );
     if (user.PasswordHash !=passwordHash) 
        return Unauthorized("Usuário ou senha inválidos");

    var token = _authService.GenerateJwt(user);
    return Ok(new { token } );
}