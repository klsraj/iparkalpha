using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Geolocator;

using iPark.Models;
using iPark.Statics;
using iPark.Views;
using Plugin.Geolocator.Abstractions;

namespace iPark.ViewModels
{
    public class ParkingListViewModel : BaseViewModel
    {
        public ParkingListViewModel()
        {
            Title = "Parking List";
            RefreshList();
        }

        ObservableCollection<ParkingLot> items = new ObservableCollection<ParkingLot>();
        public ObservableCollection<ParkingLot> Items
        {
            get { return items; }
            set { SetProperty(ref items, value, "Items"); }
        }

        Command refreshCmd;
        public Command RefreshCommand => refreshCmd ?? (refreshCmd = new Command(async () => await ExecuteRefreshCommand()));

        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                Position position = await GetPosition();

                if (position == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Cannot determine current location", "OK");
                    return;
                }

                var list = await App.CloudService.GetClient().InvokeApiAsync<List<ParkingLot>>(
                                    "ReservationSvc/GetParkingList", HttpMethod.Get,
                                    new Dictionary<string, string> {
                                        { "lat", position.Latitude.ToString() },
                                        { "lng", position.Longitude.ToString() }
                                    });

                Items.Clear();
                foreach (var item in list)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ParkingList] Error loading items: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task RefreshList()
        {
            await ExecuteRefreshCommand();
            MessagingCenter.Subscribe<PaymentViewModel>(this, MessagingServiceConstants.PAYMENT_MADE, async (sender) =>
            {
                await ExecuteRefreshCommand();
            });
        }

        public async Task ExecuteItemSelectedCommand(object selectedItem)
        {
            ParkingLot parkingLot = (ParkingLot) selectedItem;

            if (parkingLot.Available > 0)
                await PushAsync(new Payment(parkingLot));
            else
                await Application.Current.MainPage.DisplayAlert("Error", "Parking not available at the selected Parking lot", "OK");
        }

        async Task<Position> GetPosition()
        {
            Position position = null;
            var locator = CrossGeolocator.Current;

            if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
            {
                position = await locator.GetLastKnownLocationAsync();

                if (position == null)
                    position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            }

            return position;
        }
    }
}
