using LIN.Inventory.Realtime.Extensions;
using LIN.Inventory.Shared.Interfaces;
using LIN.Inventory.Shared.Services;
using LIN.Inventory.Web.Client.Services;
using LIN.Inventory.Web.Components;
using LIN.Cloud.SDK;

var builder = WebApplication.CreateBuilder(args);

// Servicios de contenedor.
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();
builder.Services.AddRealTime();
builder.Services.AddSingleton<IDeviceSelector, DeviceSelector>();
builder.Services.AddSingleton<IOpenFiles, LIN.Inventory.Web.Client.Services.File>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddCloudSDK("key.e27vRz6166SbhnuLVuUM8ZFI91UXzKm");

// Aplicación.
var app = builder.Build();
app.Services.UseRealTime(name: "Web", platform: "web", [], Scripts.Get(app.Services));

if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Redireción.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

LIN.Access.Search.Build.Init();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LIN.Inventory.Web.Client._Imports).Assembly);

app.Run();