# How create a project c#, asp.net core mvc

From ps1
> dotnet new mvc --name LittleFirmManagement

> cd .\LittleFirmManagement\

> dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

> dotnet add package Microsoft.EntityFrameworkCore.Design

> dotnet add package Npgsql.EntityFrameworkCore.Tools

create appsettings

`{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=firm;Username=postgres;Password=admin;Port=5433"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}`

From ps1
> dotnet ef dbcontext scaffold name=DefaultConnection Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models --data-annotations

add in program.cs
```
using LittleFirmManagement.Models;
using Microsoft.EntityFrameworkCore;
builder.Services.AddDbContext<FirmContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

from ps1
> dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 6

> dotnet aspnet-codegenerator controller -dc FirmContext -name FInvoicesController -m FInvoice --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

> dotnet aspnet-codegenerator controller -dc FirmContext -name FCategoriesController -m FCategory --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

> dotnet aspnet-codegenerator controller -dc FirmContext -name FClientsController -m FClient --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

> dotnet aspnet-codegenerator controller -dc FirmContext -name FPurchasesController -m FPurchase --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

> dotnet aspnet-codegenerator controller -dc FirmContext -name FInterventionsController -m FIntervention --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

> dotnet aspnet-codegenerator controller -dc FirmContext -name FCitiesController -m FCity --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

> dotnet aspnet-codegenerator controller -dc FirmContext -name FCategoryTypesController -m FCategoryType --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

copier dans `Views\Shared\_Layout.cshtml`
```
<nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
    <div class="navbar-collapse collapse d-md-inline-flex flex-md-row-reverse">
        @{
            var controllers = typeof(Program).Assembly.GetExportedTypes()
            .Where(x => typeof(ControllerBase).IsAssignableFrom(x) && !x.IsAbstract);

            <ul class="navbar-nav mr-auto">
                @foreach (var controller in controllers)
                {
                    var controllerName = controller.Name.Replace("Controller", "");
                    <li class="nav-item"><a class="nav-link" asp-controller="@controllerName" asp-action="Index">@controllerName</a></li>
                }
            </ul>
        }
    </div>

</nav>
```

