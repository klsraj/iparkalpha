using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iParkAdmin.ViewModels;

namespace iParkAdmin.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CheckInOut : ContentPage
	{
        public CheckInOut(Boolean checkin, string barCode)
        {
            InitializeComponent();

            BindingContext = new CheckInOutViewModel(checkin, barCode) { Navigation = this.Navigation };
        }
    }
}