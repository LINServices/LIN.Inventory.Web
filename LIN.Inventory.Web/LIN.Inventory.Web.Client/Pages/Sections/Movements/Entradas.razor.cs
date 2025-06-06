﻿using LIN.Access.Inventory.Controllers;
using LIN.Inventory.Realtime.Manager.Models;

namespace LIN.Inventory.Web.Client.Pages.Sections.Movements;

public partial class Entradas : IInflowModelObserver, IDisposable
{


    /// <summary>
    /// Id del inventario.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;



    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    private InventoryContext? Contexto { get; set; }



    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<InflowDataModel>? Response => Contexto?.Inflows;



    /// <summary>
    /// Entrada seleccionada.
    /// </summary>
    public static InflowDataModel? Selected { get; set; } = null;



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = InventoryManager.Get(int.Parse(Id));

        InflowObserver.Add(Contexto?.Inventory.Id ?? 0, this);

        // Evaluar la respuesta.
        if (Response == null)
            GetData();

        // Base.
        base.OnParametersSet();
    }



    /// <summary>
    /// Obtener la data.
    /// </summary>
    /// <param name="force">Forzado.</param>
    private async void GetData(bool force = false)
    {

        // Validación.
        if (!force && Response != null || IsLoading)
            return;

        // Cambiar el estado.
        IsLoading = true;
        StateHasChanged();

        // Obtiene los dispositivos
        var result = await Inflows.ReadAll(Contexto?.Inventory.Id ?? 0, Session.Instance.Token);

        // Nuevos estados.
        IsLoading = false;

        if (Contexto == null)
            return;

        Contexto.Inflows = result;

        if (Contexto != null)
            Contexto.Inflows = Response;

        StateHasChanged();
    }



    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        InvokeAsync(StateHasChanged);
    }



    /// <summary>
    /// Evento dispose.
    /// </summary>
    public void Dispose()
    {
        InflowObserver.Remove(this);
    }



    /// <summary>
    /// Evento: Al renderizar.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = GoNew,
            Section = 1,
            DockIcon = 0
        });

        base.OnAfterRender(firstRender);
    }



    /// <summary>
    /// Ir a una entrada.
    /// </summary>
    /// <param name="e">Model de la entrada.</param>
    void Go(Types.Inventory.Models.InflowDataModel e)
    {
        Selected = e;
        nav.NavigateTo($"/inflow/{e.Id}");
    }



    /// <summary>
    /// Abrir new.
    /// </summary>
    void GoNew()
    {
        nav.NavigateTo($"/new/inflow/{Contexto?.Inventory.Id}");
    }




}