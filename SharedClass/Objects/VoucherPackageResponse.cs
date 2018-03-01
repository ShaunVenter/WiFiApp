using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AlwaysOn.Objects;


namespace AlwaysOn
{
	public class VoucherPackageResponse
	{

		private List<VoucherPackage> packageList;

		private string _message;
		private bool _success;

		public List<VoucherPackage> PackageList { get { return packageList; } }
		public string Message { get { return _message; } }
		public bool Success { get { return _success; } }

		public VoucherPackageResponse (string json)
		{
			try {
				var jsonObject = JObject.Parse(json);
				JToken token = JToken.Parse(json);
				JArray packages = (JArray)token.SelectToken("PackageItems");

				var package = new List<VoucherPackage>();

				foreach (JToken item in packages)
				{

					var obj = new VoucherPackage();

					obj.Id = int.Parse(item["id"].ToString().Trim());
					obj.currency = item["currency"].ToString().Trim();
					obj.defaultPackage = item["defaultPackage"].ToString().Trim();
					obj.displayPrice = item["displayPrice"].ToString().Trim();
					obj.errormessage = item["errormessage"].ToString().Trim();
					obj.expirationDays = item["expirationDays"].ToString().Trim();
					obj.expirationHours = item["expirationHours"].ToString().Trim();
					obj.expiryDays = item["expiryDays"].ToString().Trim();
					obj.isfromfirstlogin = item["isfromfirstlogin"].ToString().Trim();
					obj.iveriPrice = item["iveriPrice"].ToString().Trim();
					obj.listOrder = item["listOrder"].ToString().Trim();
					obj.optionDesc = item["optionDesc"].ToString().Trim();
					obj.packageDesc = item["packageDesc"].ToString().Trim();
					obj.packageName = item["packageName"].ToString().Trim();
					obj.packageType = item["packageType"].ToString().Trim();
					obj.radgroupcheckid = item["radgroupcheckid"].ToString().Trim();
					obj.usageDescription = item["usageDescription"].ToString().Trim();

					obj.Description = item["additionalInfo"]["Description"].ToString().Trim();
					obj.ExpiryDays = item["additionalInfo"]["ExpiryDays"].ToString().Trim();
					obj.HeaderPost = item["additionalInfo"]["HeaderPost"].ToString().Trim();
					obj.HeaderPre = item["additionalInfo"]["HeaderPre"].ToString().Trim();
					obj.Period = item["additionalInfo"]["Period"].ToString().Trim();
					obj.Price = item["additionalInfo"]["Price"].ToString().Trim();
					obj.PricePer = item["additionalInfo"]["PricePer"].ToString().Trim();
					obj.Songs = item["additionalInfo"]["Songs"].ToString().Trim();
					obj.Value = item["additionalInfo"]["Value"].ToString().Trim();
					obj.Videos = item["additionalInfo"]["Videos"].ToString().Trim();
					obj.Voice = item["additionalInfo"]["Voice"].ToString().Trim();


					package.Add(obj);
				}

				_success = (bool)jsonObject["Success"];
				_message = (string)jsonObject["Message"];
				packageList = package;

			} catch (Exception ex) 
			{
				throw new Exception("Could not translate user package response. More details: " + ex.Message);
			}
		}
	}
}

