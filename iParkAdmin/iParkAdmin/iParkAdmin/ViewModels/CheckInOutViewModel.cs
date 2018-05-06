using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Stripe;

using iParkShared;

namespace iParkAdmin.ViewModels
{
    public class CheckInOutViewModel : BaseViewModel
    {
        private MobileServiceClient mobileService = App.CloudService.GetClient();
        private Boolean checkin;
        private string checkoutTime = null;

        public CheckInOutViewModel(Boolean checkin, string barCode)
        {
            this.checkin = checkin;
            BarCode = barCode;
            Message = null;
            IsSuccess = IsError = IsBalanceDue = false;

            MakePaymentCommand = new Command(async () => await ExecuteMakePaymentCommand());

            if (checkin)
                Checkin();
            else
                Checkout();
        }

        public string BarCode { get; set; }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value, "Message");
            }
        }

        private bool isSuccess;
        public bool IsSuccess
        {
            get { return isSuccess; }
            set
            {
                SetProperty(ref isSuccess, value, "IsSuccess");
            }
        }

        private bool isError;
        public bool IsError
        {
            get { return isError; }
            set
            {
                SetProperty(ref isError, value, "IsError");
            }
        }

        private bool isBalanceDue;
        public bool IsBalanceDue
        {
            get { return isBalanceDue; }
            set
            {
                SetProperty(ref isBalanceDue, value, "IsBalanceDue");
            }
        }

        public Command MakePaymentCommand { get; set; }

        public string CreditCardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }

        async Task Checkin()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                string msg = await App.CloudService.GetClient().InvokeApiAsync<string>(
                                    "ReservationSvc/Checkin", HttpMethod.Post,
                                    new Dictionary<string, string> {
                                    { "resCode", BarCode }
                                    });

                Message = msg;
                IsSuccess = msg == null ? true : false;
                IsError = msg == null ? false : true;
            }
            catch(Exception ex)
            {
                await ShowError("Checkin Failed", ex.Message);
                Debug.WriteLine($"[CheckInOut] Error in Checkin: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task Checkout()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                APIResponse resp = await App.CloudService.GetClient().InvokeApiAsync<APIResponse>(
                                    "ReservationSvc/Checkout", HttpMethod.Post,
                                    new Dictionary<string, string> {
                                        { "resCode", BarCode }
                                    });

                Message = resp.Message;
                IsSuccess = resp.Code == Constants.PROCESS_SUCCESS || resp.Code == Constants.PROCESS_WARNING ? true : false;
                IsError = resp.Code == Constants.PROCESS_ERROR ? true : false;
                IsBalanceDue = resp.Code == Constants.BALANCE_DUE ? true : false;

                // Remeber the checkout time returned from the API
                if (resp.Code == Constants.BALANCE_DUE)
                    checkoutTime = resp.Result;
            }
            catch (Exception ex)
            {
                await ShowError("Checkout Failed", ex.Message);
                Debug.WriteLine($"[CheckInOut] Error in Checkout: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteMakePaymentCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                string confNumber = Utils.CreateStripeToken(CreditCardNumber, ExpiryMonth, ExpiryYear, CVV);

                string msg = await App.CloudService.GetClient().InvokeApiAsync<string>(
                                    "ReservationSvc/PayBalance", HttpMethod.Post,
                                    new Dictionary<string, string> {
                                        { "resCode", BarCode },
                                        { "confNum", confNumber },
                                        { "checkout", checkoutTime }
                                    });

                Message = msg;
                IsSuccess = msg == null ? true : false;
                IsBalanceDue = msg == null ? false : true;
            }
            catch (Exception ex)
            {
                await ShowError("Payment Failed", ex.Message);
                Debug.WriteLine($"[CheckInOut] Error in MakePayment: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
