﻿using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;
    private readonly ITokenRepository tokenRepository;

    public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
    {
        this.userManager = userManager;
        this.tokenRepository = tokenRepository;
    }

    // POST: /api/auth/register
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request) 
    {
        // create identity user object
        var user = new IdentityUser
        {
            UserName = request.Email?.Trim(),
            Email = request.Email?.Trim(),
        };

        // create user
        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (identityResult.Succeeded)
        {
            // add role to user (Reader)
            identityResult = await userManager.AddToRoleAsync(user, "Reader");

            if (identityResult.Succeeded)
            {
                return Ok();
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }


        }
        else
        {
            if (identityResult.Errors.Any()) { 
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }

        return ValidationProblem(ModelState);

    }

    // POST: /api/auth/login
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var identityUser = await userManager.FindByEmailAsync(request.Email);
        if (identityUser is not  null)
        {
            var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, request.Password);

            if (checkPasswordResult)
            {
                var roles = await userManager.GetRolesAsync(identityUser);

                // create a token and response
                var jwtToken = tokenRepository.CreateJwtToken(identityUser, roles.ToList());

                var response = new LoginResponseDTO()
                {
                    Email = request.Email,
                    Roles = roles.ToList(),
                    Token = jwtToken
                };

                return Ok(response);
            }
        }

        ModelState.AddModelError("", "Email or password incorrect");
        return ValidationProblem(ModelState);
    }
}
