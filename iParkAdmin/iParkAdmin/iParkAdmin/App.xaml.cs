using System;
using Xamarin.Forms;

using iPark.Services;
using iParkAdmin.Views;

namespace iParkAdmin
{
	public partial class App : Application
	{
        public App ()
		{
			InitializeComponent();

            CloudService = new AzureCloudService();
            User = DependencyService.Get<IDevice>().GetIdentifier();

            MainPage = new NavigationPage(new MainPage());
        }

        public static ICloudService CloudService { get; set; }
        public static string User { get; set; }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
