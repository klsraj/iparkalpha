using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iPark.Models;
using iPark.ViewModels;

namespace iPark.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyReservations : ContentPage
    {
        public MyReservations()
        {
            InitializeComponent();
            BindingContext = new MyReservationsViewModel() { Navigation = this.Navigation };
        }

        async void ReservationItemTapped(object sender, ItemTappedEventArgs e)
        {
            await ((MyReservationsViewModel)BindingContext).ExecuteItemSelectedCommand(e.Item);
            ((ListView)sender).SelectedItem = null;
        }
    }
}