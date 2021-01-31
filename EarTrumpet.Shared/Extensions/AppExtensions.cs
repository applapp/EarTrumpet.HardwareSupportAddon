using EarTrumpet.UI.ViewModels;
using EarTrumpet.UI.Views;
using System.Reflection;

namespace EarTrumpet.Shared.Extensions
{
    public static class AppExtensions
    {
        public static FlyoutWindow GetFlyoutWindow(this App app)
        {
            return (FlyoutWindow)typeof(App).GetField("_flyoutWindow", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(app);
        }

        public static DeviceCollectionViewModel GetDeviceCollection(this App app)
        {
            return (DeviceCollectionViewModel)typeof(App).GetField("_collectionViewModel", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(app);
        }
    }
}
