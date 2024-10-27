namespace LIN.Inventory.Web.Client.Pages;

public partial class ResetPassword
{

    /// <summary>
    /// Navegación.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Usuario
    /// </summary>
    private string User { get; set; } = "";


    /// <summary>
    /// Contraseña
    /// </summary>
    private string Password { get; set; } = "";


    /// <summary>
    /// Contraseña
    /// </summary>
    private string OTP { get; set; } = "";


    /// <summary>
    /// Mensaje que se muestra al cargar
    /// </summary>
    private string LogMessage { get; set; } = "Validando datos";


    /// <summary>
    /// Mensaje de error
    /// </summary>
    private string ErrorMessage { get; set; } = "";


    /// <summary>
    /// Visibilidad del error.
    /// </summary>
    private bool ErrorVisible { get; set; } = false;



    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; } = 0;



    /// <summary>
    /// La respuesta de hub ya fue recibida.
    /// </summary>
    private bool isResponseReceive = false;



    /// <summary>
    /// Id único de inicio passkey.
    /// </summary>
    private string Unique = "";


    /// <summary>
    /// Hub passkey.
    /// </summary>
    private PassKeyHub? hub = null;



    /// <summary>
    /// Evento.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {


        if (Access.Auth.SessionAuth.IsOpen)
        {
            NavigationManager?.NavigateTo("/home");
            return;
        }

        // Quitar barra inferior.
        MainLayout.ShowNavigation = false;
        MainLayout.Update();

        _ = base.OnInitializedAsync();


    }



    /// <summary>
    /// Actualizar la sección.
    /// </summary>
    /// <param name="section">Id de la sección</param>
    private void UpdateSection(int section)
    {
        InvokeAsync(() =>
        {
            Section = section;
            StateHasChanged();
        });
    }



    /// <summary>
    /// Oculta los errores
    /// </summary>
    void HideError()
    {
        ErrorVisible = false;
        StateHasChanged();
    }



    /// <summary>
    /// Oculta los errores
    /// </summary>
    void GoToForget()
    {
        NavigationManager?.NavigateTo("/");
    }



    /// <summary>
    /// Muestra un mensaje
    /// </summary>
    void ShowError(string message)
    {
        InvokeAsync(() =>
        {
            UpdateSection(Section == 3 ? Section : 0);
            ErrorVisible = true;
            ErrorMessage = message;
            StateHasChanged();
        });
    }



    /// <summary>
    /// Inicia sesión.
    /// </summary>
    private async void Start()
    {


        if (Section == 3)
        {
            Vvv();
            return;
        }


        // Estado cargando.
        UpdateSection(4);

        // Ocultar el error.
        HideError();

        // Validar parámetros.
        if (User.Length <= 0)
        {
            ShowError("Completa todos los campos");
            return;
        }

        // Iniciar sesión.
        var response = await Access.Auth.Controllers.Security.ForgetPassword(User);

        // Validar respuesta.
        switch (response.Response)
        {

            // Correcto.
            case Responses.Success:
                UpdateSection(3);
                return;

            // No existe la cuenta.
            case Responses.NotExistAccount:
                ShowError($"No se encontró el usuario '{User}'");
                break;

            case Responses.NotRows:
                ShowError($"Esta cuenta no tiene emails asociados.");
                break;

            default:
                ShowError("Inténtalo mas tarde");
                break;


        }

    }



    async void Vvv()
    {
        var response = await Access.Auth.Controllers.Security.ResetPassword(User, OTP, Password);


        // Validar respuesta.
        switch (response.Response)
        {

            // Correcto.
            case Responses.Success:
                UpdateSection(1);
                await Task.Delay(3000);
                NavigationManager?.NavigateTo("/");
                return;

            // No existe la cuenta.
            case Responses.NotExistAccount:
                ShowError($"No se encontró el usuario '{User}'");
                break;

            // Desautorizado por la organización.
            case Responses.UnauthorizedByOrg:
                ShowError($"Tu organización no permite que accedas a esta app");
                break;

            default:
                ShowError("Inténtalo mas tarde");
                break;


        }

    }




    /// <summary>
    /// Mostrar error.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    void Show(string message)
    {
        InvokeAsync(async () =>
         {
             // Estado fallido.
             ErrorMessage = message;
             UpdateSection(2);

             // Esperar 2 segundos.
             await Task.Delay(2000);

             ShowError(message);
         });
    }




}