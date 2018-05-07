using UIKit;

using iPark.Services;

[assembly: Xamarin.Forms.Dependency(typeof(iPark.iOS.Services.Device))]

namespace iPark.iOS.Services
{
    public class Device : IDevice
    {
        public string GetIdentifier()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }
    }
}
