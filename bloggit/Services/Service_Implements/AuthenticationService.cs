using bloggit.DTOs;
using bloggit.Models;
using bloggit.Services.Service_Interfaces;
using bloggit.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace bloggit.Services.Service_Implements
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthenticationService(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, ITokenService tokenService, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<string> TokenLoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new DomainException("Invalid email or password", 401);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded) throw new DomainException("Invalid email or password", 401);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            return _tokenService.GenerateToken(user, roles);
        }

        public async Task Register(string firstName, string lastName, string email, string password, string userName, string country, string gender, string? profilePicture)
        {
            var newUser = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = userName,
                Country = country,
                Gender = gender,
                ProfilePicture = profilePicture,
                CreatedOn = DateTime.Now,
                isDeleted = false
            };
            var result = await _userManager.CreateAsync(newUser, password);
            // ValidateIdentityResult(result);

            await _userManager.AddToRoleAsync(newUser, "User");
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var token = ToUrlSafeBase64(emailConfirmationToken);
            await _emailService.SendEmailConfirmationEmailAsync(firstName, lastName, newUser.Id, email, token);
        }

        public async Task<IActionResult> CreateAdmin(string firstName, string lastName, string email, string username,
            string password, string country, string gender, string? profilePicture)
        {
            try
            {
                var newAdmin = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = username,
                    Country = country,
                    Gender = gender,
                    ProfilePicture = profilePicture,
                    CreatedOn = DateTime.Now,
                    isDeleted = false
                };

                var result = await _userManager.CreateAsync(newAdmin, password);
                // ValidateIdentityResult(result);

                await _userManager.AddToRoleAsync(newAdmin, "Admin");

                return new OkObjectResult(new { message = "Admin created successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Failed to create admin", error = ex.Message })
                    { StatusCode = 500 };
            }
        }

        public async Task ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var emailConfirmationToken = FromUrlSafeBase64(token);
            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
            ValidateIdentityResult(result);
        }

        public async Task ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var token = ToUrlSafeBase64(passwordResetToken);
                await _emailService.SendForgotPasswordEmailAsync(user.FirstName, user.LastName, email, token);
            }
        }

        public async Task ResetPassword(string email, string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var passwordResetToken = FromUrlSafeBase64(token);
                var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, password);

                ValidateIdentityResult(result);
            }
        }

        private void ValidateIdentityResult(IdentityResult result)
        {
            if (result.Succeeded) return;
            var errors = result.Errors.Select(x => x.Description);
            throw new DomainException(string.Join('\n', errors));
        }

        private static string ToUrlSafeBase64(string base64String)
        {
            return base64String.Replace('+', '-').Replace('/', '~').Replace('=', '_');
        }

        private static string FromUrlSafeBase64(string urlSafeBase64String)
        {
            return urlSafeBase64String.Replace('-', '+').Replace('~', '/').Replace('_', '=');
        }
    }
}