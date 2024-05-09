using bloggit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using bloggit.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using bloggit.Services.Service_Interfaces;
using bloggit.Services.Service_Implements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
if (connectionString != null)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseMySQL(connectionString);

        // For logging SQL Query
        options.EnableSensitiveDataLogging();
    });
}
else
{
    // Handle the case where the connection string is null or not found in configuration
    throw new InvalidOperationException("AppDbConnectionString is not configured.");
}


// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:AccessTokenKey"]);
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
     options.RequireHttpsMetadata = false;
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidIssuer = builder.Configuration["JWT:Issuer"],
         ValidAudience = builder.Configuration["JWT:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(key),
         ValidateIssuerSigningKey = true,
         ValidateLifetime = false
     };
     });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGmailEmailProvider, GmailEmailProvider>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var rolesToCreate = new List<string> { "Admin", "User" };

    foreach (var roleName in rolesToCreate)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

app.Run();

