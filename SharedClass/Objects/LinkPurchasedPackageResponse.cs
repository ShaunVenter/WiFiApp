using System;
using Newtonsoft.Json.Linq;

namespace AlwaysOn
{
	public class LinkPurchasedPackageResponse
	{
		//properties

		public bool Success { get; set; }
		public string Message { get; set; }


		//methods

		public LinkPurchasedPackageResponse (string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new Exception("No response from Link Purchased Package request");
			}

			JObject jsonObj;

			try 
			{
				jsonObj = JObject.Parse(json);

				Success = jsonObj["Success"] == null? false : (bool) jsonObj["Success"];
				Message = jsonObj["Message"] == null? string.Empty : jsonObj["Message"].ToString();
			} 
			catch (Exception ex) 
			{
				throw new Exception("Link Purchased Package response translation error. More details: " + ex.Message);
			}
			finally
			{
				jsonObj = null;
			}
		}
	}
}

