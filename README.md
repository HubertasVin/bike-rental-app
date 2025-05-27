## Setup
To run the backend, run these commands:
```
cd backend
dotnet add package Swashbuckle.AspNetCore
dotnet add package Scrutor
```

To run the frontend, run these commands:
```
cd frontend
npm i
npm run dev
```


## Techninės užduotys:
1. Memory management

Scope: Program.cs:
```csharp
builder.Services.Scan(scan =>
    scan.FromAssemblies(Assembly.GetExecutingAssembly())
        .AddClasses(classes =>
            classes.Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Service"))
        )
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);
```

SessionScoped: Niekur nenaudojamas (nėra niekur įjungiami komponentai su AddSession() metodu Program.cs faile).


2. Logging
backend/Logs/log-20250526.txt:
```
2025-05-26 23:30:16.148 +03:00 [INF] User=string; Roles=User; Time=2025-05-26 20:30:16Z; Method=IBikeService.GetAllBikesAsync
```
Faile LoggingProxy.cs:
```csharp
if (ToggleService.Enabled)
{
    var user = HttpContextAccessor.HttpContext?.User;
    var userName = user?.Identity?.Name ?? "<anonymous>";
    var roles =
        user?.Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .DefaultIfEmpty("<none>")
            .Aggregate((a, b) => $"{a},{b}") ?? "<none>";

    Logger.LogInformation(
        "User={User}; Roles={Roles}; Time={Time:u}; Method={Type}.{Method}",
        userName,
        roles,
        DateTime.UtcNow,
        typeof(T).Name,
        targetMethod.Name
    );
}
```
Kad įjungti/išjungti žurnalizavimą, iškvieskite http://localhost:5000/api/logging?enabled=false arba su true.

8. Extensibility/Glass-box extensibility

### Alternative

Scope: StandardPricingStrategy.cs:
```
using BikeRentalApp.Application.Services.Interfaces;

namespace BikeRentalApp.Application.Services.Pricing;

public class StandardPricingStrategy : IPricingStrategy
{
    public string Name => "Standard";

    public decimal CalculatePrice(decimal basePrice, double durationMinutes)
    {
        return (decimal)Math.Ceiling(durationMinutes) * basePrice;
    }
}
```

Scope: DiscountedPricingStrategy.cs:
```
using BikeRentalApp.Application.Services.Interfaces;

namespace BikeRentalApp.Application.Services.Pricing;

public class DiscountedPricingStrategy : IPricingStrategy
{
    public string Name => "Discounted";

    public decimal CalculatePrice(decimal basePrice, double durationMinutes)
    {
        var standardPrice = (decimal)Math.Ceiling(durationMinutes) * basePrice;
        // 50% discount
        return standardPrice * 0.5m;
    }
}
```

Scope: Program.cs:
```
var pricingStrategy = builder.Configuration.GetValue<string>("PricingStrategy", "Standard");
if (pricingStrategy == "Discounted")
{
    builder.Services.AddScoped<IPricingStrategy, DiscountedPricingStrategy>();
}
else
{
    builder.Services.AddScoped<IPricingStrategy, StandardPricingStrategy>();
}
```

### Decorator

Scope: Program.cs:
```
builder.Services.Decorate<IRentalRepository, LoggingRentalRepositoryDecorator>();
```