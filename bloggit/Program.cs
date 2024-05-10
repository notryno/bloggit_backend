using bloggit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using bloggit.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using bloggit.Hubs;
using Microsoft.IdentityModel.Tokens;
using bloggit.Services.Service_Interfaces;
using bloggit.Services.Service_Implements;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
if (connectionString != null)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseMySQL(connectionString);

        // For logging SQL Query
        // options.EnableSensitiveDataLogging();
    });
}
else
{
    // Handle the case where the connection string is null or not found in configuration
    throw new InvalidOperationException("AppDbConnectionString is not configured.");
}


builder.Services.AddSignalR();

//Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Configure two-factor authentication
        options.Tokens.ProviderMap["Default"] = new TokenProviderDescriptor(
            typeof(DataProtectorTokenProvider<ApplicationUser>));
        options.Tokens.ChangeEmailTokenProvider = "Default";
        options.Tokens.ChangePhoneNumberTokenProvider = "Default";
        options.Tokens.EmailConfirmationTokenProvider = "Default";
        options.Tokens.PasswordResetTokenProvider = "Default";
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:AccessTokenKey"]);
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,   
         ValidateIssuerSigningKey = true,
         ValidateLifetime = true,
         ValidIssuer = builder.Configuration["JWT:Issuer"],
         ValidAudience = builder.Configuration["JWT:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(key)
     };
     });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IGmailEmailProvider, GmailEmailProvider>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILogService, LogService>();

var app = builder.Build();

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:3000/", "https://localhost:3001")
        .AllowAnyMethod()
        .WithHeaders(HeaderNames.ContentType)
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

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

