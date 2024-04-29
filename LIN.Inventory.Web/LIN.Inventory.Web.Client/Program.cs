global using LIN;
global using LIN.Types.Responses;
global using LIN.Types.Cloud.Identity.Enumerations;
global using Microsoft.AspNetCore.Components;
global using LIN.Types.Cloud.Identity.Models;
global using LIN.Access.Auth.Hubs;
global using Microsoft.JSInterop;
global using LIN.Types.Contacts.Models;
global using LIN.Types.Contacts.Enumerations;
global using LIN.Access.Inventory;
global using LIN.Types.Inventory.Transient;

global using LIN.Types.Inventory.Models;
global using LIN.Types.Inventory.Enumerations;

global using LIN.Inventory.Shared.Services.Observers;

global using LIN.Inventory.Shared.Popup;

global using LIN.Inventory.Shared.Utilities;
global using LIN.Inventory.Shared.Drawers;




using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

LIN.Access.Auth.Build.Init();
LIN.Access.Inventory.Build.Init();
LIN.Access.Search.Build.Init();

await builder.Build().RunAsync();
