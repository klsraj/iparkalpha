// The MIT License (MIT)
//
// Copyright (c) 2015 Xamarin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iPark.Services;
using iPark.Localization;
using iPark.Views;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace iPark
{
    public partial class App : Application
    {
        static Application app;
        public static Application CurrentApp
        {
            get { return app; }
        }

        readonly IAuthenticationService _AuthenticationService;

        public static ICloudService CloudService { get; set; }

        public static string User { get; set; }

        public App()
        {
            InitializeComponent();

            app = this;
            _AuthenticationService = DependencyService.Get<IAuthenticationService>();

            CloudService = new AzureCloudService();
            User = DependencyService.Get<IDevice>().GetIdentifier();

            /* if we were targeting Windows Phone, we'd want to include the next line. */
            // if (Device.OS != TargetPlatform.WinPhone) 
            TextResources.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();


            // If the App.IsAuthenticated property is false, modally present the SplashPage.
            if (!_AuthenticationService.IsAuthenticated)
            {
                MainPage = new SplashPage();
            }
            else
            {
                GoToRoot();
            }

        }

        public static void GoToRoot()
        {
            CurrentApp.MainPage = new MainPage();

            /*
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    CurrentApp.MainPage = new RootPage();
                    break;
                case Device.iOS:
                    CurrentApp.MainPage = new RootTabPage();
                    break;
            }
            */

            /*
            if (Device.OS == TargetPlatform.iOS)
            {
                CurrentApp.MainPage = new RootTabPage();
            }
            else
            {
                CurrentApp.MainPage = new RootPage();
            }
            */
        }

        public static async Task ExecuteIfConnected(Func<Task> actionToExecuteIfConnected)
        {
            if (IsConnected)
            {
                await actionToExecuteIfConnected();
            }
            else
            {
                await ShowNetworkConnectionAlert();
            }
        }

        static async Task ShowNetworkConnectionAlert()
        {
            await CurrentApp.MainPage.DisplayAlert(
                TextResources.NetworkConnection_Alert_Title,
                TextResources.NetworkConnection_Alert_Message,
                TextResources.NetworkConnection_Alert_Confirm);
        }

        public static bool IsConnected
        {
            get
            { return true;
              //return CrossConnectivity.Current.IsConnected;
            }
        }

        public static int AnimationSpeed = 250;
    }
}
