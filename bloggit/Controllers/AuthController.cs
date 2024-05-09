﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bloggit.Controllers;

[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("/api/auth/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest login)
    {
        var token = await _authenticationService.TokenLoginAsync(login.Email, login.Password);
        return Ok(new TokenLoginResponse
        {
            Token = token
        });
    }

    [HttpPost("/api/auth/register")]
    public async Task<IActionResult> RegisterAsync([FromBody] DTOs.RegisterRequest register)
    {
        await _authenticationService.Register(register.FirstName, register.LastName, register.Email, register.Password);
        return Ok(new { message = "Registration successful" });
    }

}

