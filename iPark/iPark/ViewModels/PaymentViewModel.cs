using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Stripe;

using iPark.Models;
using iPark.Statics;
using iParkShared;

namespace iPark.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private MobileServiceClient mobileService = App.CloudService.GetClient();

        public PaymentViewModel(ParkingLot selectedLot)
        {
            Title = "Payment";
            ParkingLot = selectedLot;
            ShortTerm = true;
            SelectedParkingPeriod = ReservationType.LTDaily;
            ParkingDate = MinimumDate = DateTime.Now.Date;

            GetPriceModel();
        }

        public ParkingLot ParkingLot { get; set; }

        public List<ReservationType> ParkingPeriod { get { return ReservationType.LongTerm; } }
        public ReservationType SelectedParkingPeriod { get; set; }

        public DateTime ParkingDate { get; set; }
        public DateTime MinimumDate { get; set; }

        private PriceModel priceModel;
        public PriceModel PriceModel
        {
            get { return priceModel; }
            set
            {
                SetProperty(ref priceModel, value, "PriceModel");
            }
        }

        private bool shortTerm;
        public bool ShortTerm
        {
            get { return shortTerm; }
            set
            {
                SetProperty(ref shortTerm, value, "ShortTerm");
            }
        }

        private decimal totalCharge;
        public decimal TotalCharge
        {
            get { return totalCharge; }
            set
            {
                SetProperty(ref totalCharge, value, "TotalCharge");
            }
        }

        private DateTime checkinTime;
        public DateTime CheckinTime
        {
            get { return checkinTime; }
            set
            {
                SetProperty(ref checkinTime, value, "CheckinTime");
            }
        }

        public string CreditCardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }

        Command makePaymentCmd;
        public Command MakePaymentCommand => makePaymentCmd ?? (makePaymentCmd = new Command(async () => await ExecuteMakePaymentCommand()));

        async Task ExecuteMakePaymentCommand()
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                string confNumber = Utils.CreateStripeToken(CreditCardNumber, ExpiryMonth, ExpiryYear, CVV);

                DateTime startDate;

                if (ShortTerm)
                    startDate = ParkingLot.ConvertToLocalTime(DateTimeOffset.UtcNow).Date; // Today at Parking Lot
                else
                    startDate = new DateTime(ParkingDate.Year, ParkingDate.Month, ParkingDate.Day); // User selected date

                DateTimeOffset startDateUtc = TimeZoneInfo.ConvertTimeToUtc(startDate, ParkingLot.GetTimezone());

                Reservation resItem = new Reservation
                {
                    Id = Guid.NewGuid().ToString(),
                    LotId = ParkingLot.Id,
                    PriceModelId = PriceModel.Id,
                    Type = ShortTerm ? ReservationType.ST.Code : SelectedParkingPeriod.Code,
                    CustomerRef = App.User,
                    StartDate = startDateUtc,
                    ExpectedCheckin = ShortTerm ? DateTimeOffset.UtcNow.AddHours((double)PriceModel.AdvanceTime) : startDateUtc,
                    ConfNumAdvance = confNumber
                };

                string msg = await mobileService.InvokeApiAsync<Reservation, string>("ReservationSvc/MakeReservation", resItem);

                if (msg == null)
                {
                    MessagingCenter.Send<PaymentViewModel>(this, MessagingServiceConstants.PAYMENT_MADE);
                    await PopAsync();
                }
                else
                    await ShowError("Reservation Failed", msg);
                    //await Application.Current.MainPage.DisplayAlert("Error", msg, "OK");
            }
            catch (Exception ex)
            {
                await ShowError("Reservation Failed", ex.Message);
                Debug.WriteLine($"[Payment] Error in Make Payment: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GetPriceModel()
        {
            try
            {
                PriceModel = await mobileService.InvokeApiAsync<ParkingLot, PriceModel>("ReservationSvc/GetPriceModel", ParkingLot);
                TotalCharge = (decimal) priceModel.MinTime * priceModel.Rate;
                CheckinTime = ParkingLot.ConvertToLocalTime(DateTime.UtcNow, TimeZoneInfo.Utc).AddHours((double)priceModel.AdvanceTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Payment] Error while getting Price Model: {ex.Message}");
            }
        }
    }
}
