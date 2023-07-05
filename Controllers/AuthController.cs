using dotnet_rpg.Data;
using dotnet_rpg.Dtos.UserRegisterDto;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserRegisterDto request)
    {
        var response = await _authRepository.Register(new User {Username = request.Username}, request.Password);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginDto request)
    {
        var response = await _authRepository.Login(request.Username, request.Password);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

}