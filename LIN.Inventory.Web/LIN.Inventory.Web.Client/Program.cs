global using LIN.Access.Auth.Hubs;
global using LIN.Access.Inventory;
global using LIN.Inventory.Shared.Drawers;
global using LIN.Inventory.Shared.Popup;
global using LIN.Inventory.Shared.Services.Observers;
global using LIN.Inventory.Shared.Utilities;
global using LIN.Types.Cloud.Identity.Enumerations;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Types.Contacts.Models;
global using LIN.Types.Inventory.Enumerations;
global using LIN.Types.Inventory.Models;
global using LIN.Types.Inventory.Transient;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Components;
using LIN.Access.Auth;
using LIN.Inventory.Shared.Interfaces;
using LIN.Inventory.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LIN.Inventory.Realtime.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<IDeviceSelector, DeviceSelector>();
builder.Services.AddAuthenticationService();
builder.Services.AddRealTime();
LIN.Access.Inventory.Build.Init();
LIN.Access.Search.Build.Init();

var app = builder.Build();
app.Services.UseRealTime("Web", Scripts.Build());

await builder.Build().RunAsync();