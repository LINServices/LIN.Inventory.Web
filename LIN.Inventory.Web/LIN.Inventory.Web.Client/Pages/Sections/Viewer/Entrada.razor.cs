using LIN.Inventory.Realtime.Manager.Models;
using LIN.Inventory.Realtime.Manager;
using LIN.Inventory.Web.Client.Layout;

namespace LIN.Inventory.Web.Client.Pages.Sections.Viewer;


public partial class Entrada
{


    AlertPopup Alerta;

    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;


    bool edit = false;


    /// <summary>
    /// Modelo
    /// </summary>
    private InflowDataModel? Modelo { get; set; } = new();



    protected override async Task OnParametersSetAsync()
    {

        InventoryContext? inventoryContext = InventoryManager.FindContextByInflow(int.Parse(Id));

        // Validar.
        if (inventoryContext == null)
        {
            // Obtener los detalles.
            var inflowDetails = await Access.Inventory.Controllers.Inflows.Read(int.Parse(Id), Access.Inventory.Session.Instance.Token, true);

            // Validar respuesta.
            if (inflowDetails.Response == Responses.Success)
            {
                Modelo = inflowDetails.Model;
            }


            return;
        }


        // Obtener la salida.
        var inflow = (from inflowModel in (inventoryContext.Inflows ?? new()).Models
                      where inflowModel.Id == int.Parse(Id)
                      select inflowModel).FirstOrDefault();

        // Si no hay detalles.
        if (inflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var inflowDetails = await Access.Inventory.Controllers.Inflows.Read(inflow.Id, Session.Instance.Token, true);

            if (inflowDetails.Response == Responses.Success)
            {
                inflow.Details = inflowDetails.Model.Details;
                inflow.Inversion = inflowDetails.Model.Inversion;
                inflow.Prevision = inflowDetails.Model.Prevision;
            }
            else if (inflowDetails.Response == Responses.Unauthorized)
            {
                Alerta.Show("No tienes autorización para visualizar los movimientos.");
            }

        }

        // Establecer el modelo.
        Modelo = inflow;



        await base.OnParametersSetAsync();


    }
    /// <summary>
    /// Evento al iniciar.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = Send,
            Section = 1,
            DockIcon = 2
        });

        await base.OnInitializedAsync();

    }


    /// <summary>
    /// Enviar el comando al selector.
    /// </summary>
    void Send()
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (e) =>
        {
            deviceManager.SendToDevice($"viewInflow({Modelo?.Id})", e.Id);
        };

        MainLayout.DevicesSelector.Show();
    }






    public static void Show(int id)
    {
        MainLayout.Navigate($"/inflow/{id}");
    }

    void ControllerDate()
    {
        edit = !edit;
        StateHasChanged();
    }


    async void Update()
    {
        var newdate = Modelo?.Date;

        await Access.Inventory.Controllers.Inflows.Update(Modelo.Id, newdate.Value, Session.Instance.Token);
        edit = false;
        await InvokeAsync(StateHasChanged);
    }


    (string, string, string) GetPrevision()
    {

        string @base = "bg-money/20 dark:bg-green-100/20";
        string Tittle = "text-money";
        string svg = "fill-money";

        if (Modelo == null)
            return (@base, Tittle, svg);


        if (Modelo.Prevision < 0)
        {
            @base = "bg-red-500/20 dark:bg-red-100/20";
            Tittle = "text-red-500";
            svg = "fill-red-500";
        }

        if (Modelo.Prevision == 0)
        {
            @base = "bg-orange-500/20 dark:bg-orange-100/20";
            Tittle = "text-orange-500";
            svg = "fill-orange-500";
        }


        return (@base, Tittle, svg);

    }


    private string GetImage()
    {



        switch (Modelo?.Type)
        {
            case Types.Inventory.Enumerations.InflowsTypes.Purchase:
                return "./img/Products/inflows/cart.png";
            case Types.Inventory.Enumerations.InflowsTypes.Refund:
                return "./img/Products/inflows/return.png";
            case Types.Inventory.Enumerations.InflowsTypes.Gift:
                return "./img/Products/inflows/gift.png";
            case Types.Inventory.Enumerations.InflowsTypes.Correction:
                return "./img/Products/inflows/setting.png";
            default:
                return "./img/Products/packages.png";
        }

    }

}