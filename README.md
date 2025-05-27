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

3. Data mapper efektyvi implementacija
Faile ReportRepository.cs:
```csharp
public async Task<bool> DeleteAsync(Guid id)
{
    var count = await _context.Reports.Where(r => r.Id == id).ExecuteDeleteAsync();
    return count > 0;

    // Originalus kodas, kuris buvo pakeistas į efektyvesnį:
    // var report = await GetByIdAsync(id);
    // if (report == null)
    // {
    //     return false;
    // }

    // await DeleteAsync(report);
    // await SaveChangesAsync();
    // return true;
}
```
Kodas efektyvesnis, nes naudoja `ExecuteDeleteAsync`, kuris leidžia ištrinti įrašus tiesiogiai iš duomenų bazės, be papildomo objekto gavimo ir ištrynimo.
