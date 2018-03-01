using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace AlwaysOn
{
    public class SecurePaymentResponse
    {
        //properties

        public bool Success { get; private set; }
        public string Message { get; private set; }
        public PaymentReturn paymentReturn { get; private set; }

        public SecurePaymentResponse(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }

        public SecurePaymentResponse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception("No response from secure payment request");
            }

            try
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<SecurePayment>(json);

                Success = obj.Success;
                Message = obj.Message;
                paymentReturn = obj.PaymentReturn;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not complete secure payment. More details: " + ex.Message);
            }
        }

        public class SecurePayment
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public PaymentReturn PaymentReturn { get; set; }
        }

        public class PaymentReturn
        {
            public bool PaymentSuccess { get; set; }
            public string ErrorMessage { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Package_Type { get; set; }
            public string Package_Bought { get; set; }
            public string Package_Bought_Expiration { get; set; }
        }
    }
}

