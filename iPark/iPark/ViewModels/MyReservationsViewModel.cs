using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

using iPark.Models;
using iPark.Statics;
using iPark.Views;

namespace iPark.ViewModels
{
    class MyReservationsViewModel : BaseViewModel
    {
        public MyReservationsViewModel()
        {
            Title = "My Reservations";
            RefreshList();
        }

        ObservableCollection<Reservation> items = new ObservableCollection<Reservation>();
        public ObservableCollection<Reservation> Items
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
                var list = await App.CloudService.GetClient().InvokeApiAsync<List<Reservation>>(
                                    "ReservationSvc", HttpMethod.Get,
                                    new Dictionary<string, string> {
                                        { "customerRef", App.User }
                                    });

                Items.Clear();
                foreach (var item in list)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MyReservations] Error loading items: {ex.Message}");
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
            await PushAsync(new ReservationDetails((Reservation) selectedItem));
        }
    }
}
