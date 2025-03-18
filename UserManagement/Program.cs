using System.Configuration;
using UserManagement.Extensions;
using UserMgmtDAL;
using UserMgmtDAL.Repositories.Abstract;
using UserMgmtDAL.Repositories.Concrete;
using UserMgmtDAL.Models;
using Microsoft.Extensions.DependencyInjection;
using UserMgmtDAL.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JWTAuthenticationManager;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using UserManagement.Controllers;


IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .Build();
IHttpContextAccessor httpContextAccessor;
var builder = WebApplication.CreateBuilder(args);

// Default to Database1 configuration
string databaseConfigFile = "appsettings.Database1.json";
// Configure configuration settings
builder.Services.Configure<ApptypeSettings>(builder.Configuration.GetSection("ApptypeSettings"));
builder.Services.Configure<AppdbtypeSettings>(builder.Configuration.GetSection("AppdbtypeSettings"));

// Access configuration settings
var appSettings = builder.Configuration.GetSection("ApptypeSettings").Get<ApptypeSettings>();

// Determine the appropriate connection string
string connectionString;



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// Register your data access services and implementations
builder.Services.AddTransient<IUserRepository, UserRepository>();
//builder.Services.AddTransient<Iaddress, addressRepository>();
builder.Services.AddTransient<IRolerepository, Rolesrepository>();
builder.Services.AddTransient<ICommonRepository, CommonRepository>();
builder.Services.AddTransient<IrefreshToken, RefreshTokenRepository>();
builder.Services.AddTransient<Ipassword, Passwordrepository>();
builder.Services.AddTransient<Iactivity, Useractivityrepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ITokenhandler, Tokenhandler>();

builder.Services.AddSingleton<TokenService>();
//builder.Services.AddServices(configuration, httpContextAccessor);
//builder.Services.AddTransient<IUserRepository>(provider =>
//new UserRepository(httpContextAccessor));
//builder.Services.AddTransient<Iaddress>(provider =>
//    new addressRepository(builder.Configuration.GetConnectionString(connectionString)));
//builder.Services.AddTransient<IUserRepository>(provider =>
//    new UserRepository(builder.Configuration.GetConnectionString(connectionString)));
var tokenOptions = builder.Configuration.GetSection("Jwt").Get<TokenOptions>();
builder.Services.AddSingleton(tokenOptions);
builder.Services.AddCustomeJWTAuthExtension();
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//        .AddJwtBearer(options =>
//        {
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = builder.Configuration["Jwt:Issuer"],
//                ValidAudience = builder.Configuration["Jwt:Audience"],
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
//            };
//        });

//builder.Services.AddAuthorization();
builder.Services.AddCors(options =>

{
    options.AddPolicy(name: "corsDevelopment",
        builder =>
        {
           // builder.WithOrigins("http://localhost:4200", "https://patshala.co.in", "https://localhost:44453", "https://patshala.azurewebsites.net", "*");
           builder.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://localhost:3000", "https://localhost:3000", "https://rndtechiesservices-gme3bhgcb3bsa8cz.southindia-01.azurewebsites.net", "https://Onlytwam.com", "http://Onlytwam.com", "https://localhost:7074").AllowAnyHeader().AllowAnyMethod().SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Set the maximum age for preflight requests;
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("corsDevelopment");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
