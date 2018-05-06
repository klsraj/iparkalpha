using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iPark.Models;
using iPark.ViewModels;

namespace iPark.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Payment : ContentPage
    {
        public Payment(ParkingLot parkingLot)
        {
            InitializeComponent();
            BindingContext = new PaymentViewModel(parkingLot) { Navigation = this.Navigation };
        }
    }
}