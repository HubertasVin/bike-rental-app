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
   a. Scope: Program.cs:
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
    b. SessionScoped: Niekur nenaudojamas


2. Logging
   backend/Logs/log-20250526.txt:
   ```
2025-05-26 23:30:16.148 +03:00 [INF] User=string; Roles=User; Time=2025-05-26 20:30:16Z; Method=IBikeService.GetAllBikesAsync
   ```