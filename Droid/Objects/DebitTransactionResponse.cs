using System;
using Newtonsoft.Json.Linq;

namespace AlwaysOn_Droid
{
	public class DebitTransactionResponse
	{
		//properties

		public string Id { get; set; }
		public string PaymentType { get; set; }
		public string PaymentBrand { get; set; }
		public string Amount { get; set; }
		public string Currency { get; set; }
		public string Descriptor { get; set; }
		public string BuildNumber { get; set; }
		public string Timestamp { get; set; }
		public string Ndc { get; set; }

		public Result Result { get; set; }
		public Card Card { get; set; }
		public Risk Risk { get; set; }


		//methods

		public DebitTransactionResponse (string json)
		{
			JObject jsonObj;
			JToken token;

			try 
			{
				jsonObj = JObject.Parse(json);

				Id = jsonObj["id"].ToString();
				PaymentType = jsonObj["paymentType"].ToString();
				PaymentBrand = jsonObj["paymentBrand"].ToString();
				Amount = jsonObj["amount"].ToString();
				Currency = jsonObj["currency"].ToString();
				Descriptor = jsonObj["descriptor"].ToString();
				BuildNumber = jsonObj["buildNumber"].ToString();
				Timestamp = jsonObj["timestamp"].ToString();
				Ndc = jsonObj["ndc"].ToString();


				token = JObject.Parse(json);

				var node = token.SelectToken("result").ToString();
				jsonObj = JObject.Parse(node);
				Result = new Result();
				Result.Code = jsonObj["code"].ToString();
				Result.Description = jsonObj["description"].ToString();

				node = token.SelectToken("card").ToString();
				jsonObj = JObject.Parse(node);
				Card = new Card();
				Card.Bin = jsonObj["bin"].ToString();
				Card.Last4Digits = jsonObj["last4Digits"].ToString();
				Card.Holder = jsonObj["holder"].ToString();
				Card.ExpiryMonth = jsonObj["expiryMonth"].ToString();
				Card.ExpiryYear = jsonObj["expiryYear"].ToString();

				node = token.SelectToken("risk").ToString();
				jsonObj = JObject.Parse(node);
				Risk = new Risk();
				Risk.Score = jsonObj["score"].ToString();

				//JToken token = JToken.Parse(json);
				//JArray packages = (JArray)token.SelectToken("result");
				/*
				Result = new Result();
				foreach (JToken item in packages)
				{
					Result.Code = item["code"].ToString();	
					Result.Description = item["description"].ToString();
				}
				*/
			} 
			catch (Exception ex) 
			{
				throw new Exception(ex.Message);
			}
			finally
			{
				jsonObj = null;
				token = null;
			}
		}
	}

	public class Result
	{
		public string Code { get; set; }		
		public string Description { get; set; }
	}

	public class Card
	{
		public string Bin { get; set; }		
		public string Last4Digits { get; set; }
		public string Holder { get; set; }
		public string ExpiryMonth { get; set; }
		public string ExpiryYear { get; set; }
	}

	public class Risk
	{
		public string Score { get; set; }		
	}
}

