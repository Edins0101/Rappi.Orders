using Microsoft.AspNetCore.RateLimiting;
using Rappi.Orders.Api.Common;
using Rappi.Orders.Application;
using Rappi.Orders.Application.Interfaces;
using Rappi.Orders.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddOpenApi();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (allowedOrigins is { Length: > 0 })
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
    });
});

var rateLimit = builder.Configuration.GetValue<int>("RateLimit:RequestsPerMinute");

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiter =>
    {
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.PermitLimit = rateLimit;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    // OpenAPI JSON
    app.MapOpenApi().DisableRateLimiting();

    // UI (Scalar)
    app.MapScalarApiReference().DisableRateLimiting();
}

app.UseCors("DefaultCors");

// Evitar rate limit sobre la UI y OpenAPI (para que no se auto-bloquee)
app.UseWhen(
    ctx => !(ctx.Request.Path.StartsWithSegments("/scalar") || ctx.Request.Path.StartsWithSegments("/openapi")),
    appBuilder => appBuilder.UseRateLimiter()
);
app.UseRateLimiter();
app.MapControllers();
app.Run();