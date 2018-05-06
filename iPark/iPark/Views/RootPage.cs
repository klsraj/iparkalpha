using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

using iPark.ViewModels;

namespace iPark.Views
{
    public class RootPage : MasterDetailPage
    {
        Dictionary<MenuType, NavigationPage> Pages { get; set; }

        public RootPage()
        {
            Pages = new Dictionary<MenuType, NavigationPage>();
            Master = new MenuPage(this);
            BindingContext = new BaseViewModel();

            //setup home page
            NavigateAsync(MenuType.Reservations);
        }

        void SetDetailIfNull(Page page)
        {
            if (Detail == null && page != null)
                Detail = page;
        }

        public async Task NavigateAsync(MenuType id)
        {
            Page newPage;
            if (!Pages.ContainsKey(id))
            {
                NavigationPage page;

                switch (id)
                {
                    case MenuType.Parking:
                        page = new NavigationPage(new ParkingList());
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                    case MenuType.Reservations:
                        page = new NavigationPage(new MyReservations());
                        SetDetailIfNull(page);
                        Pages.Add(id, page);
                        break;
                }
            }

            newPage = Pages[id];
            if (newPage == null)
                return;

            /*
            //pop to root for Windows Phone
            if (Detail != null && Device.OS == TargetPlatform.WinPhone)
            {
                await Detail.Navigation.PopToRootAsync();
            }
            */

            Detail = newPage;

            if (Device.Idiom != TargetIdiom.Tablet)
                IsPresented = false;
        }
    }

    /*
    public class RootTabPage : TabbedPage
    {
        public RootTabPage()
        {
            Children.Add(new iParkNavigationPage(new SalesDashboardPage
                    { 
                        Title = TextResources.MainTabs_Sales, 
                        Icon = new FileImageSource { File = "sales.png" }
                    })
                { 
                    Title = TextResources.MainTabs_Sales, 
                    Icon = new FileImageSource { File = "sales.png" }
                });
            Children.Add(new iParkNavigationPage(new CustomersPage
                    { 
                        BindingContext = new CustomersViewModel() { Navigation = this.Navigation }, 
                        Title = TextResources.MainTabs_Customers, 
                        Icon = new FileImageSource { File = "customers.png" } 
                    })
                {  
                    Title = TextResources.MainTabs_Customers, 
                    Icon = new FileImageSource { File = "customers.png" } 
                });
            Children.Add(new iParkNavigationPage(new CategoryListPage
                    { 
                        BindingContext = new CategoriesViewModel() { Navigation = this.Navigation }, 
                        Title = TextResources.MainTabs_Products, 
                        Icon = new FileImageSource { File = "products.png" } 
                    })
                { 
                    Title = TextResources.MainTabs_Products, 
                    Icon = new FileImageSource { File = "products.png" },
                });
            Children.Add(new iParkNavigationPage(new AboutItemListPage
                    { 
                        Title = TextResources.MainTabs_About, 
                        Icon = new FileImageSource { File = "about.png" },
                        BindingContext = new AboutItemListViewModel() { Navigation = this.Navigation }
                    })
                { 
                    Title = TextResources.MainTabs_About, 
                    Icon = new FileImageSource { File = "about.png" } 
                });
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            this.Title = this.CurrentPage.Title;
        }
    }
    */

    /*
    public class iParkNavigationPage : NavigationPage
    {
        public iParkNavigationPage(Page root)
            : base(root)
        {
            Init();
        }

        public iParkNavigationPage()
        {
            Init();
        }

        void Init()
        {

            //BarBackgroundColor = Palette._001;
            //BarTextColor = Color.White;
        }
    }
    */

    public enum MenuType
    {
        Parking,
        Reservations
    }

    public class HomeMenuItem
    {
        public HomeMenuItem()
        {
            MenuType = MenuType.Reservations;
        }

        public string Icon { get; set; }

        public MenuType MenuType { get; set; }

        public string Title { get; set; }

        public string Details { get; set; }

        public int Id { get; set; }
    }

    /*public class RootPage : TabbedPage
    {
        

        public RootPage()
        {
            
            // the Sales tab page
            this.Children.Add(
                new NavigationPage(new SalesDashboardPage())
                { 
                    Title = TextResources.MainTabs_Sales, 
                    Icon = new FileImageSource() { File = "SalesTab" }
                }
            );

            // the Customers tab page
            this.Children.Add(
                new CustomersPage()
                { 
                    BindingContext = new CustomersViewModel(Navigation), 
                    Title = TextResources.MainTabs_Customers, 
                    Icon = new FileImageSource() { File = "CustomersTab" } 
                }
            );

            // the Products tab page
            this.Children.Add(
                new NavigationPage(new CategoryListPage() { BindingContext = new CategoriesViewModel() } )
                { 
                    Title = TextResources.MainTabs_Products, 
                    Icon = new FileImageSource() { File = "ProductsTab" } 
                }
            );
        }
    }*/
}

