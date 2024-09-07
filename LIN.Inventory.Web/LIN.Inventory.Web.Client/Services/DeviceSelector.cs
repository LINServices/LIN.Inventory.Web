using LIN.Inventory.Shared.Interfaces;

namespace LIN.Inventory.Web.Client.Services
{
    public class DeviceSelector : IDeviceSelector
    {
        public void Send(string command)
        {
            // Nuevo onInvoque.
            MainLayout.DevicesSelector.OnInvoke = (e) =>
            {
                Services.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
                {
                    Command = command
                });
            };

            MainLayout.DevicesSelector.Show();
        }
    }
}
