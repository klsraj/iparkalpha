using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using iPark.Services;
using iPark.Statics;
using iPark.ViewModels;

namespace iPark.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SplashPage : ContentPage
	{
        readonly IAuthenticationService _AuthenticationService;
        SplashViewModel viewModel;

        public SplashPage ()
		{
			InitializeComponent ();

            BindingContext = viewModel = new SplashViewModel();
            _AuthenticationService = DependencyService.Get<IAuthenticationService>();

            SignInButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    NumberOfTapsRequired = 1,
                    Command = new Command(SignInButtonTapped)
                });

            SkipSignInButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    NumberOfTapsRequired = 1,
                    Command = new Command(SkipSignInButtonTapped)
                });
        }

        async Task<bool> Authenticate()
        {
            bool success = false;
            try
            {
                // The underlying call behind App.Authenticate() calls the ADAL library, which presents the login UI and awaits success.
                success = await _AuthenticationService.AuthenticateAsync();
            }
            catch (Exception ex)
            {
                if (ex is AdalException && (ex as AdalException).ErrorCode == "authentication_canceled")
                {
                    // Do nothing, just duck the exception. The user just cancelled out of the login UI.
                }
                else
                    await DisplayAlert("Login error", "An unknown login error has occurred. Please try again.", "OK");
            }
            finally
            {
                // When the App.Authenticate() returns, the login UI is hidden, regardless of success (for example, if the user taps "Cancel" in iOS).
                // This means the SplashPage will be visible again, so we need to make the sign in button clickable again by hiding the activity indicator (via the IsPresentingLoginUI property).
                viewModel.IsPresentingLoginUI = false;
            }

            return success;
        }

        async void SignInButtonTapped()
        {
            await App.ExecuteIfConnected(async () =>
            {
                // trigger the activity indicator through the IsPresentingLoginUI property on the ViewModel
                viewModel.IsPresentingLoginUI = true;

                if (await Authenticate())
                {
                    // Pop off the modally presented SplashPage.
                    // Note that we're not popping the ADAL auth UI here; that's done automatcially by the ADAL library when the Authenticate() method returns.
                    App.GoToRoot();

                    // Broadcast a message that we have sucessdully authenticated.
                    // This is mostly just for Android. We need to trigger Android to call the SalesDashboardPage.OnAppearing() method,
                    // because unlike iOS, Android does not call the OnAppearing() method each time that the Page actually appears on screen.
                    MessagingCenter.Send(this, MessagingServiceConstants.AUTHENTICATED);
                }
            });
        }

        async void SkipSignInButtonTapped()
        {
            _AuthenticationService.BypassAuthentication();

            // Broadcast a message that we have sucessdully authenticated.
            // This is mostly just for Android. We need to trigger Android to call the SalesDashboardPage.OnAppearing() method,
            // because unlike iOS, Android does not call the OnAppearing() method each time that the Page actually appears on screen.
            //MessagingCenter.Send(this, MessagingServiceConstants.AUTHENTICATED);

            App.GoToRoot();
        }
    }
}