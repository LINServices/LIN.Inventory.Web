using LIN.Inventory.Shared.Interfaces;

namespace LIN.Inventory.Web.Client.Services;

internal class File : IOpenFiles
{

    /// <summary>
    /// Carga la imagen de perfil
    /// </summary>
    public async Task<byte[]> OpenFile()
    {
        return await Open();
    }


    /// <summary>
    /// Carga la imagen de perfil
    /// </summary>
    public static async Task<byte[]> Open()
    {
        return [];
    }

}