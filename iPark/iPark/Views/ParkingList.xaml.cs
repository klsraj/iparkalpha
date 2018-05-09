using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iPark.Models;
using iPark.ViewModels;

namespace iPark.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParkingList : ContentPage
    {
        public ParkingList()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
                NavigationPage.SetBackButtonTitle(this, "Back");

            BindingContext = new ParkingListViewModel() { Navigation = this.Navigation };
        }

        async void ParkingListItemTapped(object sender, ItemTappedEventArgs e)
        {
            await ((ParkingListViewModel)BindingContext).ExecuteItemSelectedCommand(e.Item);
            ((ListView)sender).SelectedItem = null;
        }
    }
}