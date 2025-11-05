using LIN.Inventory.Shared.Controls;

namespace LIN.Inventory.Web.Client.Pages.Sections.Edits;


public partial class EditProduct
{
    /// <summary>
    /// Id del producto.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    public ProductModel ProductBase { get; set; }





    /// <summary>
    /// Model del producto.
    /// </summary>
    public ProductModel Product { get; set; } = new()

    {
        Details = [new()]
    };


    private string ErrorMessage = "";



    private bool isNewPhoto = false;

    private async void OpenImage()
    {
        //  Photo =  await Services.File.Open();
        isNewPhoto = true;
        StateHasChanged();
    }


    private async void SetImage(string photo)
    {
        previewUrl = photo.Length <= 0
            ? null
            : "https://api.linplatform.com/bucket/PublicFiles/" + photo + ".png";

        isNewPhoto = false;
        StateHasChanged();
    }


    private async void DeleteImage()
    {
        Image = [];
        previewUrl = null;
        isNewPhoto = true;
        StateHasChanged();
    }



    /// <summary>
    /// Categoría.
    /// </summary>
    public int Category { get; set; }



    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        var result = InventoryManager.GetProduct(int.Parse(Id));


        if (result == null)
        {
            nav.NavigateTo("/home");
            return;
        }

        Product = new()
        {
            Category = result.Category,
            Statement = result.Statement,
            Code = result.Code,
            Description = result.Description,
            Name = result.Name,
            Inventory = result.Inventory,
            InventoryId = result.InventoryId,
            Id = result.Id,
            Image = result.Image,
            Details = [
                new ProductDetailModel(){
                    Status = result.DetailModel.Status,
                    Id = result.DetailModel.Id,
                    PurchasePrice = result.DetailModel.PurchasePrice,
                    SalePrice = result.DetailModel.SalePrice,
                    Quantity = result.DetailModel.Quantity
                }
                ],
        };


        SetImage(result.Image);
        Category = (int)Product.Category;

        ProductBase = result;

        // Base.
        base.OnParametersSet();
    }



    protected override void OnInitialized()
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = Update,
            Section = 1,
            DockIcon = 3
        });

        base.OnInitialized();
    }


    /// <summary>
    /// Crear.
    /// </summary>
    private async void Update()
    {
        try
        {

            Section = 3;
            StateHasChanged();


            if (!NeedUpdateDetail())
            {
                ProductBase.Details = Product.Details;
                Product.Details = [];
            }

            Product.Category = (ProductCategories)Category;


            if (!isNewPhoto)
                Product.Image = null!;
            else
                Product.Image = await SaveImage();

            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Update(Product, Session.Instance.Token);

            Product.Details = ProductBase.Details;

            switch (response.Response)
            {

                case Responses.Success:
                    break;

                case Responses.Unauthorized:
                    Section = 2;
                    ErrorMessage = "No tienes autorización para modificar este producto.";
                    StateHasChanged();
                    return;

                default:
                    Section = 2;
                    ErrorMessage = "Hubo un error al modificar este producto.";
                    StateHasChanged();
                    return;
            }


            // Actualizar modelo existente.
            ProductBase.Category = Product.Category;
            ProductBase.Code = Product.Code;
            ProductBase.Description = Product.Description;
            ProductBase.Name = Product.Name;
            ProductBase.Image = Product.Image;

            if (Product.DetailModel != null && ProductBase.DetailModel != null)
            {
                ProductBase.DetailModel.PurchasePrice = Product.DetailModel.PurchasePrice;
                ProductBase.DetailModel.SalePrice = Product.DetailModel.SalePrice;
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



    private void GoNormal()
    {
        Section = 0;
        StateHasChanged();
    }



    private bool NeedUpdateDetail()
    {

        try
        {
            if (ProductBase.DetailModel.PurchasePrice != Product.DetailModel.PurchasePrice)
                return true;


            if (ProductBase.DetailModel.SalePrice != Product.DetailModel.SalePrice)
                return true;


            return false;
        }
        catch
        {

        }

        return false;


    }


    private string GetImage()
    {
        return ProductBase?.Image.Length <= 0
            ? "./img/Products/packages.png"
            : "https://api.linplatform.com/bucket/PublicFiles/" + ProductBase?.Image + ".png";
    }



    private GenericFilePicker? filePicker;
    private string? previewUrl;
    private string? fileName;

    private async Task SelectFile()
    {
        if (filePicker is null) return;

        await filePicker.OpenAsync();

    }

    private byte[] Image = Array.Empty<byte>();

    private async Task OnSelectedPicture()
    {

        var fileBytes = filePicker.GetFile();
        fileName = filePicker.GetFileName();

        if (fileBytes != null)
        {
            isNewPhoto = true;
            Image = fileBytes;
            var base64 = Convert.ToBase64String(fileBytes);
            previewUrl = $"data:image/png;base64,{base64}";
        }
    }


    private async Task<string> SaveImage()
    {
        if (Image == null || Image.Length <= 0)
        {
            // manejar: no hay imagen
            return string.Empty;
        }

        using var memoryStream = new MemoryStream(Image);

        var response = await Client.Upload(memoryStream, "inventory", (e) => { }, filePublic: true);

        if (response.Response != Responses.Success)
        {
            // manejar error de subida
            return string.Empty;
        }

        return response.Model.PublicPath;
    }

}