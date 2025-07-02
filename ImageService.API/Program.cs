using ImageService.Application.Interfaces;
using ImageService.Application.Services;
using ImageService.Domain;
using ImageService.Infrastructure.Data;
using ImageService.Infrastructure.Repositories;
using ImageService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =====  DB  =====
builder.Services.AddDbContext<ImageDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// =====  DI  =====
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();

// Storage Mode 切換: appsettings  "Storage:Mode" = "Fake" | "GCS"
var mode = builder.Configuration["Storage:Mode"] ?? "GCS";
if (mode == "Fake")
    builder.Services.AddSingleton<IStorageService, FakeStorageService>();
else
    builder.Services.AddSingleton<IStorageService, GcsStorageService>();

// =====  JWT  =====
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();          // 只在本機 dev 啟用
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
