using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using bloggit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bloggit.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }


    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest login)
    {
        var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, true, lockoutOnFailure: false);

        if (result.Succeeded)
            return Ok();
        return Unauthorized();
    }

    [HttpPost("/api/auth/register")]
    public async Task<IActionResult> RegisterAsync([FromBody] CustomRegisterRequest register)
    {
        var user = new ApplicationUser { UserName = register.UserName, Email = register.Email, FirstName = register.FirstName, LastName = register.LastName };

        var result = await _userManager.CreateAsync(user, register.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false); // Automatically sign in the user after registration
            return Ok();
        }

        // If registration fails, return the error messages
        return BadRequest(result.Errors);
    }

}

