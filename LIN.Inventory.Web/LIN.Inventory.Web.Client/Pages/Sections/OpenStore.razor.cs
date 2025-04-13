namespace LIN.Inventory.Web.Client.Pages.Sections;

public partial class OpenStore
{


    bool Mode = false;

    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    OpenStoreSettings Settings { get; set; } = new();



    protected override async Task OnParametersSetAsync()
    {


        // Obtener el contexto.
        var contexto = InventoryManager.Get(int.Parse(Id));

        // Obtener la información de OpenStore.
        var ss = await Access.Inventory.Controllers.OpenStore.ReadSettings(Session.Instance.Token, int.Parse(Id));

        contexto.Inventory.OpenStoreSettings = ss.Model;

        if (ss.Response == Responses.NotRows)
        {
            Mode = true;
        }
        else if (ss.Response != Responses.Success)
        {
            Mode = false;
        }
        else
        {
            Settings = ss.Model;
            Mode = false;
        }

        StateHasChanged();
        await base.OnParametersSetAsync();
    }

    string user = string.Empty;
    string password = string.Empty;
    string tokenMercado = string.Empty;

    async Task Create()
    {

        var create = await LIN.Access.Inventory.Controllers.OpenStore.CreateSettings(Session.Instance.Token, tokenMercado, int.Parse(Id), user, password);

    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            StateHasChanged();
        base.OnAfterRender(firstRender);
    }

}
