using LIN.Inventory.Realtime.Manager;
using LIN.Inventory.Shared.Interfaces;

namespace LIN.Inventory.Web.Client.Services
{
    public class DeviceSelector(IDeviceManager deviceManager) : IDeviceSelector
    {
        public void Send(string command)
        {
            // Nuevo onInvoque.
            MainLayout.DevicesSelector.OnInvoke = (e) =>
            {
                deviceManager.SendToDevice(command, e.Id);
            };

            MainLayout.DevicesSelector.Show();
        }
    }
}
