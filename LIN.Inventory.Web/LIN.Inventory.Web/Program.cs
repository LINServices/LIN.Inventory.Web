using LIN.Inventory.Realtime.Extensions;
using LIN.Inventory.Shared.Interfaces;
using LIN.Inventory.Web.Client.Services;
using LIN.Inventory.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSingleton<IDeviceSelector, DeviceSelector>();
builder.Services.AddRealTime();

var app = builder.Build();
app.Services.UseRealTime("Web", "web", Scripts.Build());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LIN.Inventory.Web.Client._Imports).Assembly);

app.Run();
