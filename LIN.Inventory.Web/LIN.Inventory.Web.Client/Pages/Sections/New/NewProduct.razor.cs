using LIN.Inventory.Realtime.Manager.Models;
using LIN.Inventory.Shared.Controls;

namespace LIN.Inventory.Web.Client.Pages.Sections.New;

public partial class NewProduct
{
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Model del producto.
    /// </summary>
    public ProductModel Product { get; set; } = new()
    {
        Details = [new()]
    };

    /// <summary>
    /// Categoría.
    /// </summary>
    public int Category { get; set; }

    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; }

    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    private InventoryContext? Contexto { get; set; }

    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {
        // Obtener el contexto.
        Contexto = InventoryManager.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }

    /// <summary>
    /// Crear.
    /// </summary>
    private async void Create()
    {
        try
        {
            Section = 3;
            StateHasChanged();

            //Product.Provider = 1;
            Product.InventoryId = Contexto?.Inventory?.Id ?? 0;
            Product.Category = (ProductCategories)Category;
            Product.Statement = ProductBaseStatements.Normal;

            Product.Image = await SaveImage();

            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Create(Product, Session.Instance.Token);

            switch (response.Response)
            {
                case Responses.Success:
                    break;

                case Responses.Unauthorized:
                    Section = 2;
                    StateHasChanged();
                    return;

                default:
                    Section = 2;
                    StateHasChanged();
                    return;
            }

            Section = 1;
            StateHasChanged();

            await Task.Delay(3000);
            Section = 0;
            StateHasChanged();

        }
        catch (Exception)
        {
        }

    }

    private ImageUploader? uploader;

    private async Task<string> SaveImage()
    {
        if (uploader?.ImageBytes == null)
        {
            // manejar: no hay imagen
            return string.Empty;
        }

        // aquí tienes el byte[] listo para usar
        byte[] imagen = uploader.ImageBytes;

        using var memoryStream = new MemoryStream(imagen);

        var response = await Client.Upload(memoryStream, "inventory", (e)=> { }, filePublic: true);

        if (response.Response != Responses.Success)
        {
            // manejar error de subida
            return string.Empty;
        }

        return response.Model.PublicPath;
    }

}