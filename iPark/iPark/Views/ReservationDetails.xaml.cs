using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iPark.Models;
using iPark.ViewModels;

namespace iPark.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReservationDetails : ContentPage
    {
        public ReservationDetails(Reservation reservation)
        {
            InitializeComponent();
            BindingContext = new ReservationDetailsViewModel(reservation) { Navigation = this.Navigation };
        }
    }
}