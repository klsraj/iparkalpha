using System;
using Stripe;

namespace iParkShared
{
    public class Utils
    {
        public static string CreateStripeToken(string cardNumber, string cardExpMonth, string cardExpYear, string cardCVC)
        {
            StripeConfiguration.SetApiKey("pk_test_kwCcalSyhmEyq3CmxJqfG6ZJ");

            var tokenOptions = new StripeTokenCreateOptions()
            {
                Card = new StripeCreditCardOptions()
                {
                    Number = cardNumber,
                    ExpirationYear = Int32.Parse(cardExpYear),
                    ExpirationMonth = Int32.Parse(cardExpMonth),
                    Cvc = cardCVC
                }
            };

            var tokenService = new StripeTokenService();
            StripeToken stripeToken = tokenService.Create(tokenOptions);

            return stripeToken.Id;
        }

        public static void CreateStripeCharge(decimal dollarAmount, string parkingLotName, string confNum)
        {
            var charge = new StripeChargeCreateOptions
            {
                Amount = (int)(dollarAmount * 100), // In cents, not dollars
                Currency = "USD",
                Description = "Parking at " + parkingLotName,
                SourceTokenOrExistingSourceId = confNum
            };

            var service = new StripeChargeService("sk_test_eDnOLpG1jgHU4jk5D4S6PEHD");
            var response = service.Create(charge);
        }
    }
}