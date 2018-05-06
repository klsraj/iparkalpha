using Android.Provider;

using iPark.Services;

[assembly: Xamarin.Forms.Dependency(typeof(iPark.Droid.Services.Device))]

namespace iPark.Droid.Services
{
    public class Device : IDevice
    {
        public string GetIdentifier()
        {
            return Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Settings.Secure.AndroidId);
        }
    }
}
