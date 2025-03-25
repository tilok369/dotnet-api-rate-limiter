using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using RateLimiter.Api;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddFixedWindowRateLimiter();

var app = builder.Build();

static string GetTime() => (DateTime.Now).ToString("HH:mm:ss");
app.MapGet("/rate-limit/fixed-window", () => $"Fixed-window rate limiter: Ticks:{GetTime()}")
    .RequireRateLimiting("fixed-window");

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.Run();