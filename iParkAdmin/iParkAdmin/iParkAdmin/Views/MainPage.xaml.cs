using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iParkAdmin.ViewModels;

namespace iParkAdmin.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainViewModel() { Navigation = this.Navigation };
        }
    }
}