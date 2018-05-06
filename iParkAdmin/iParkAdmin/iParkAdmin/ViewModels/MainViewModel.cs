using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

using iParkAdmin.Views;
using System.Net.Http;
using System.Collections.Generic;

namespace iParkAdmin.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public Command CheckinCommand { get; set; }
        public Command CheckoutCommand { get; set; }

        public MainViewModel()
        {
            Title = "Admin";
            CheckinCommand = new Command(async () => await ExecuteScanCommand(true));
            CheckoutCommand = new Command(async () => await ExecuteScanCommand(false));
        }

        async Task ExecuteScanCommand(Boolean checkin)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var scanPage = new ZXingScannerPage();

                scanPage.OnScanResult += (result) => {

                    scanPage.IsScanning = false; // Stop scanning

                    // Pop the page and process the result
                    Device.BeginInvokeOnMainThread(() => {
                        Page pg = Navigation.NavigationStack[Navigation.NavigationStack.Count - 1];
                        ProcessCode(checkin, result.Text);
                        Navigation.RemovePage(pg);
                    });
                };

                // Navigate to the scanner page
                await Navigation.PushAsync(scanPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async void ProcessCode(Boolean checkin, string barCode)
        {
            await Navigation.PushAsync(new CheckInOut(checkin, barCode));
        }
    }
}