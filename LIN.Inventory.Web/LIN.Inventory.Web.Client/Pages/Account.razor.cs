

using LIN.Inventory.Web.Client.Layout;
using LIN.Inventory.Web.Client.Services;

namespace LIN.Inventory.Web.Client.Pages;


public partial class Account
{


    /// <summary>
    /// Cerrar sesión.
    /// </summary>
    async void Close()
    {

        // Cerrar la sesión.
        Session.CloseSession();

        // Limpiar error.
        InventoryContext.Dictionary.Clear();

        // Limpiar.
        Inventory.Clean();
        Contactos.Response = null;
        Home.Clean();

        // Limpiar Hub.
        deviceManager.CloseSession();

        // Navegar al inicio.
        nav.NavigateTo("/", true, true);

    }



    /// <summary>
    /// Evento: Al inicializar.
    /// </summary>
    protected override void OnInitialized()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = () => { },
            Section = 3,
            DockIcon = 0
        });

        base.OnInitialized();
    }


}
