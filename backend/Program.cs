// Program.cs
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using BikeRentalApp.Application.Services.Interfaces;
using BikeRentalApp.Application.Services.Pricing;
using BikeRentalApp.Data;
using BikeRentalApp.Infrastructure.Extensions;
using BikeRentalApp.Repositories.Decorators;
using BikeRentalApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Core;
using Serilog.Events;

// 1) Create a Serilog level switch that we can mutate at runtime
var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

// 2) Configure Serilog to use that switch
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(levelSwitch)
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// 3) Tell the generic host to use Serilog
builder.Host.UseSerilog();

// 4) Register the level switch in DI so controllers/endpoints can change it
builder.Services.AddSingleton(levelSwitch);

// 5) Authentication & JWT setup
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            ),

            NameClaimType = "name",
            RoleClaimType = ClaimTypes.Role,
        };
    });

// 6) Make [Authorize] implicit on all controllers
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// 7) EF Core DbContext (scoped by default)
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 8) Register pricing strategy (can be configured via appsettings)
var pricingStrategy = builder.Configuration.GetValue<string>("PricingStrategy", "Standard");
if (pricingStrategy == "Discounted")
{
    builder.Services.AddScoped<IPricingStrategy, DiscountedPricingStrategy>();
}
else
{
    builder.Services.AddScoped<IPricingStrategy, StandardPricingStrategy>();
}

// 9) Auto-scan your Services & Repos as scoped
builder.Services.AddScoped<JwtService>();
builder.Services.Scan(scan =>
    scan.FromAssemblies(Assembly.GetExecutingAssembly())
        .AddClasses(classes =>
            classes.Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("Repository"))
        )
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

// 10) Decorator for RentalRepository
builder.Services.Decorate<IRentalRepository, LoggingRentalRepositoryDecorator>();

builder.Services.AddSingleton<ILoggingToggleService, LoggingToggleService>();

// 11) Enable AOP logging on all your I*Service
builder.Services.AddServiceLogging();

// 12) Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header. Enter your token; the 'Bearer' prefix is added automatically.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        }
    );
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

// 13) CORS
builder.Services.AddCors(o =>
    o.AddPolicy(
        "AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod();
        }
    )
);

var app = builder.Build();

// 14) Development-only middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Apply EF migrations on startup in DEV
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// 15) Standard middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

// 16) Map controllers (including your LoggingController if you added one)
app.MapControllers();

// 17) Example minimal endpoint
var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};
app.MapGet(
        "/weatherforecast",
        () =>
        {
            var forecast = Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        }
    )
    .WithName("GetWeatherForecast");

// 18) Run the app
app.Run();

// record kept at bottom
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
