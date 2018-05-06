using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace iPark.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : ContentPage
	{
        RootPage root;
        List<HomeMenuItem> menuItems;

        public MenuPage (RootPage root)
		{
            this.root = root;
            InitializeComponent();

            ListViewMenu.ItemsSource = menuItems = new List<HomeMenuItem>
                {
                    new HomeMenuItem { Title = "My Reservations", MenuType = MenuType.Reservations, Icon = "about.png" },
                    new HomeMenuItem { Title = "Find Parking", MenuType = MenuType.Parking, Icon = "products.png" },
                };

            ListViewMenu.SelectedItem = menuItems[0];

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;

                await this.root.NavigateAsync(((HomeMenuItem)e.SelectedItem).MenuType);
            };
        }
    }
}