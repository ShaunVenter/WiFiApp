using System;

namespace AlwaysOn_Droid
{
	public class DebitTransactionRequest
	{
		//fields

		private string packageId = string.Empty;
		private string email = string.Empty;
		private string mobile = string.Empty;

		private static string paymentType = "DB";
		private decimal amount = 0.00M;
		private PaymentBrands paymentBrand;
		private string cardNumber = string.Empty;
		private string cardHolder = string.Empty;
		private int cardExpiryMonth = 0;
		private int cardExpiryYear = 0;
		private string cardCvv = string.Empty;


		//properties

		public string PackageId 
		{ 
			get { return packageId; } 

			private set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new Exception("Package Id is required");
				}

				packageId = value.Trim();
			}
		}

		public string Email		
		{ 
			get { return email; } 

			private set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new Exception("Email is required");
				}

				email = value.Trim();
			}
		}

		public string Mobile		
		{ 
			get { return mobile; } 

			private set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new Exception("Mobile is required");
				}

				mobile = value.Trim();
			}
		}

		public string Amount		
		{ 
			get { return amount.ToString(); } 
		}

		public string PaymentBrand		
		{ 
			get { return paymentBrand.ToString(); }
		}


		public string PaymentType 
		{ 
			get { return paymentType; } 
		}


		public string CardNumber		
		{ 
			get { return cardNumber; } 

			private set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new Exception("Card Number is required");
				}

				cardNumber = value.Trim();
			}
		}

		public string CardHolder		
		{ 
			get { return cardHolder; } 

			private set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new Exception("Card Holder is required");
				}

				cardHolder = value.Trim();
			}
		}

		public string CardExpiryMonth 
		{
			get 
			{
				return cardExpiryMonth.ToString().PadLeft(2, '0');
			}
		}

		public string CardExpiryYear 
		{
			get 
			{
				return cardExpiryYear.ToString();
			}
		}

		public string CardCvv		
		{ 
			get { return cardCvv; } 

			private set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new Exception("Card CVV is required");
				}

				cardCvv = value.Trim();
			}
		}


		//methods

		public DebitTransactionRequest(string packageId, string email, string mobile, decimal amount, PaymentBrands paymentBrand, string cardNumber, string cardHolder, int cardExpiryMonth, int cardExpiryYear, string cardCvv)
		{
			PackageId = packageId;
			Email = email;
			Mobile = mobile;

			if (amount < 0)
			{
				throw new Exception("Amount can not be negative");
			}
			this.amount = amount;

			this.paymentBrand = paymentBrand;

			CardNumber = cardNumber;
			CardHolder = cardHolder;

			if (cardExpiryMonth < 1 || cardExpiryMonth > 12)
			{
				throw new Exception("Card Expiry Month is not in range");
			}
			this.cardExpiryMonth = cardExpiryMonth;

			if (cardExpiryYear < 0)
			{
				throw new Exception("Card Expiry Year is not in range");
			}
			this.cardExpiryYear = cardExpiryYear;

			CardCvv = cardCvv;
		}
	}

    public enum PaymentBrands
    {
        VISA = 1,
        MASTER = 2
    }
}

