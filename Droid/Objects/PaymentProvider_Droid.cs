using AlwaysOn;
using AlwaysOn.Objects;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AlwaysOn_Droid
{
    public class PaymentProvider_Droid
    {
        public static SecurePaymentResponse DirectDebit(string UserId, string Email, string Mobile, string PackageId, string CardNumber, string CVV, string NameOnCard, string ExpMonth, string ExpYear)
        {
            var p = new StringBuilder("?api_key=" + MainApplication.ApiKey)
                .Append("&user_id=" + UserId)
                .Append("&email=" + Email)
                .Append("&mobile=" + Mobile)
                .Append("&package_id=" + PackageId)
                .Append("&card_number=" + CardNumber)
                .Append("&cvv=" + CVV)
                .Append("&name_on_card=" + NameOnCard)
                .Append("&card_exp_month=" + ExpMonth)
                .Append("&card_exp_year=" + ExpYear);

            try
            {
                using (var client = new WebClient())
                {
                    var json = client.UploadString(ConfigurationProvider.SecurePayment + p, "");
                    return new SecurePaymentResponse(json);
                }
            }
            catch (Exception ex)
            {
                return new SecurePaymentResponse(false, "An error occured during payment process: " + ex.Message);
            }
        }
    }
}

