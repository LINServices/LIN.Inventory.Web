global using LIN.Access.Auth.Hubs;
global using LIN.Access.Inventory;
global using LIN.Inventory.Realtime.Manager.Interfaces;
global using LIN.Inventory.Shared.Drawers;
global using LIN.Inventory.Shared.Popup;
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
using LIN.Inventory.Realtime.Extensions;
using LIN.Inventory.Shared.Interfaces;
using LIN.Inventory.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<IDeviceSelector, DeviceSelector>();
builder.Services.AddSingleton<IOpenFiles, LIN.Inventory.Web.Client.Services.File>();
builder.Services.AddAuthenticationService();
builder.Services.AddRealTime();
LIN.Access.Inventory.Build.Init();
LIN.Access.Search.Build.Init();

var app = builder.Build();
app.Services.UseRealTime("Web client", "WEB", Scripts.Get(app.Services));

LIN.Access.Search.Build.Init();

await builder.Build().RunAsync();