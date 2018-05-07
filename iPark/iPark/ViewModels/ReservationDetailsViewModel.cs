using System;
using System.Threading.Tasks;
using Xamarin.Forms;

using iPark.Models;
using iParkShared;

namespace iPark.ViewModels
{
    class ReservationDetailsViewModel : BaseViewModel
    {
        public ReservationDetailsViewModel(Reservation reservation)
        {
            Title = "Reservation Details";
            Reservation = reservation;
            ParkingLot = reservation.ParkingLot;

            PhoneIconTappedCommand = new Command(async () => await ExecutePhoneIconTappedCommand());
            MapIconTappedCommand = new Command(async () => await ExecuteMapIconTappedCommand());
        }

        public Command PhoneIconTappedCommand { get; set; }
        public Command MapIconTappedCommand { get; set; }

        public Reservation Reservation { get; set; }
        public ParkingLot ParkingLot { get; set; }
        public string TelephoneNumber
        {
            get { return Int64.TryParse(ParkingLot.Telephone, out Int64 num) ? $"{num:(###) ### ####}" : null; }
        }

        public bool IsShortTerm { get { return Reservation.Type == ReservationType.ST.Code; } }
        public bool IsLongTerm { get { return Reservation.Type != ReservationType.ST.Code; } }

        async Task ExecutePhoneIconTappedCommand()
        {
            Device.OpenUri(new System.Uri("tel:" + ParkingLot.Telephone));
        }

        async Task ExecuteMapIconTappedCommand()
        {
            switch(Device.RuntimePlatform)
            {
                case Device.Android:
                    Device.OpenUri(new System.Uri($"http://maps.google.com/?daddr={ParkingLot.Latitude},{ParkingLot.Longitude}"));
                    break;
                case Device.iOS:
                    Device.OpenUri(new System.Uri($"http://maps.apple.com/?daddr={ParkingLot.Latitude},{ParkingLot.Longitude}"));
                    break;
            }
        }
    }
}
