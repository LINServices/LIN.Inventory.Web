﻿using LIN.Inventory.Realtime.Manager.Models;
using LIN.Inventory.Realtime.Manager;
using LIN.Inventory.Web.Client.Layout;

namespace LIN.Inventory.Web.Client.Pages.Sections.Viewer;


public partial class Salida
{




    AlertPopup Alerta;


    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Modelo.
    /// </summary>
    private OutflowDataModel? Modelo { get; set; } = new();



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
      
        InventoryContext? inventoryContext = InventoryManager.FindContextByOutflow(int.Parse(Id));

        // Validar.
        if (inventoryContext == null)
        {
            // Obtener los detalles.
            var outflowDetails = await Access.Inventory.Controllers.Outflows.Read(int.Parse(Id), Access.Inventory.Session.Instance.Token, true);

            // Validar respuesta.
            if (outflowDetails.Response == Responses.Success)
                Modelo = outflowDetails.Model;

            return;
        }


        // Obtener la salida.
        var outflow = (from outflowModel in (inventoryContext.Outflows ?? new()).Models
                       where outflowModel.Id == int.Parse(Id)
                       select outflowModel).FirstOrDefault();

        // Si no hay detalles.
        if (outflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var outflowDetails = await Access.Inventory.Controllers.Outflows.Read(outflow.Id, Session.Instance.Token, true);

            if (outflowDetails.Response == Responses.Success)
            {
                outflow.Details = outflowDetails.Model.Details;
                outflow.Inversion = outflowDetails.Model.Inversion;
                outflow.Ganancia = outflowDetails.Model.Ganancia;
                outflow.Utilidad = outflowDetails.Model.Utilidad;
            }
            else if (outflowDetails.Response == Responses.Unauthorized)
            {
                Alerta.Show("No tienes autorización para visualizar los movimientos.");
            }
        }

        // Establecer el modelo.
        Modelo = outflow;
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
            deviceManager.SendToDevice($"viewOutflow({Modelo?.Id})", e.Id);
        };

        MainLayout.DevicesSelector.Show();
    }






    public static void Show(int id)
    {
        MainLayout.Navigate($"/outflow/{id}");
    }


    bool edit = false;

    void ControllerDate()
    {
        edit = !edit;
        StateHasChanged();
    }


    async void Update()
    {
        var newdate = Modelo?.Date;

        await Access.Inventory.Controllers.Outflows.Update(Modelo.Id, newdate.Value, Session.Instance.Token);
        edit = false;
        await InvokeAsync(StateHasChanged);
    }



    (string, string, string) GetGanancy()
    {

        string @base = "bg-money/20 dark:bg-green-100/20";
        string Tittle = "text-money";
        string svg = "fill-money";

        if (Modelo == null)
            return (@base, Tittle, svg);

        if (Modelo.Ganancia < Modelo.Inversion)
        {
            @base = "bg-red-500/20 dark:bg-red-100/20";
            Tittle = "text-red-500";
            svg = "fill-red-500";
        }

        if (Modelo.Ganancia == Modelo.Inversion)
        {
            @base = "bg-orange-500/20 dark:bg-orange-100/20";
            Tittle = "text-orange-500";
            svg = "fill-orange-500";
        }


        return (@base, Tittle, svg);

    }


    (string, string, string) GetUtilities()
    {



        string @base = "bg-money/20 dark:bg-green-100/20";
        string Tittle = "text-money";
        string svg = "fill-money";

        if (Modelo == null)
            return (@base, Tittle, svg);

        if (Modelo.Utilidad == 0)
        {
            @base = "bg-red-500/20 dark:bg-red-100/20";
            Tittle = "text-red-500";
            svg = "fill-red-500";
        }

        if (Modelo.Utilidad < 0)
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
            case Types.Inventory.Enumerations.OutflowsTypes.Usage:
                return "./img/Products/outflows/seller.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Contribution:
                return "./img/Products/outflows/donate.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Fraud:
                return "./img/Products/outflows/criminal.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Purchase:
                return "./img/Products/outflows/shop.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Loss:
                return "./img/Products/outflows/lost.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Expiry:
                return "./img/Products/outflows/expired.png";
            default:
                return "./img/Products/packages.png";
        }

    }

}
