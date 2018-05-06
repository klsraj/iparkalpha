using System.Globalization;
using Xamarin.Forms;
using iPark.Localization;

[assembly:Dependency(typeof(iPark.Droid.Localization.Localize))]

namespace iPark.Droid.Localization
{
    public class Localize : ILocalize
    {
        public CultureInfo GetCurrentCultureInfo ()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace ("_", "-"); // turns pt_BR into pt-BR
            return new CultureInfo(netLanguage);
        }
    }
}

