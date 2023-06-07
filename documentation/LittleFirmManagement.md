# How create a project c#, asp.net core mvc

## postgre sql database

just after migration of the data in  the postgre sql database

aware about "sequences" in the postgre sql database
> SELECT setval('f_client_c_id_seq', (SELECT MAX(c_id) FROM f_client));

convert "bit(1)" to "boolean" in the postgre sql database
> ALTER TABLE f_client ALTER COLUMN c_is_pro TYPE boolean USING c_is_pro::text::boolean;

> ALTER TABLE f_invoice ALTER COLUMN in_is_eligible_deferred_tax_credit TYPE boolean USING in_is_eligible_deferred_tax_credit::text::boolean;


## c# code

From ps1
> dotnet new mvc --name LittleFirmManagement

> cd .\LittleFirmManagement\

> dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

> dotnet add package Microsoft.EntityFrameworkCore.Design

> dotnet add package Npgsql.EntityFrameworkCore.Tools

complete `f_purchase` table

> UPDATE f_purchase SET p_invoice_date = COALESCE(p_invoice_date, p_disbursement_date),p_debit_date = COALESCE(p_debit_date, p_disbursement_date) WHERE p_invoice_date IS NULL OR p_debit_date IS NULL;

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
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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

replace `DateOnly` by `DateTime`

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

remplacer dans `Views\FCategories\Index.cshtml`

`@Html.DisplayFor(modelItem => item.CaFkCategoryType.CtName)`

remplacer dans `Controllers\FCategoriesController.cs`

`ViewData["CaFkCategoryTypeId"] = new SelectList(_context.FCategoryTypes, "CtId", "CtName");`

ajouter une carte

```<div id="mapid" style="height: 400px;"></div>```


```
@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/leaflet@1.7.1/dist/leaflet.js"></script>

    <!-- Leaflet CSS -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/leaflet.min.css" crossorigin="anonymous" referrerpolicy="no-referrer" />

    <!-- Leaflet JavaScript -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/leaflet.min.js" crossorigin="anonymous" referrerpolicy="no-referrer"></script>

    <script>
        var mymap = L.map('mapid').setView([47.9375, -3.9585], 11);

        var redIcon = L.icon({
            iconUrl: 'https://cdn.rawgit.com/pointhi/leaflet-color-markers/master/img/marker-icon-red.png',
            iconSize: [25, 41],
            iconAnchor: [12, 41],
            popupAnchor: [1, -34],
            tooltipAnchor: [16, -28],
            shadowSize: [41, 41]
        });

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors'
        }).addTo(mymap);

        // loop through each city and add a marker to the map
        @foreach (var c in Model)
        {
            <text>var marker = L.marker([@c.CLocationLat, @c.CLocationLong], { icon: redIcon }).addTo(mymap);
            marker.bindPopup("<b>@c.CName</b>"); </text>

        }
    </script>
}
```
