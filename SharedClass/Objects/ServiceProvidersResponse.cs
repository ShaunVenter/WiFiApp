using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace AlwaysOn
{
	public class ServiceProvidersResponse
	{
		//properties

		public bool Success { get; private set; }
		public  string Message { get; private set; }
		public List<ServiceProvider> ServiceProviders{ get; private set; }


		public ServiceProvidersResponse (string json)
		{
			if (string.IsNullOrWhiteSpace(json))
			{
				throw new Exception("No response from service providers request");
			}

			JObject jsonObj;
			JToken token;
			JArray items;

			try 
			{
				jsonObj = JObject.Parse(json);

				Success = jsonObj["Success"] == null? false : (bool)jsonObj["Success"];
				Message = jsonObj["Message"] == null? string.Empty : jsonObj["Message"].ToString();

				token = JToken.Parse(json);
				items = (JArray)token.SelectToken("ServiceProviders");
				ServiceProviders = new List<ServiceProvider>();

				foreach (JToken item in items)
				{
					var obj = new ServiceProvider(item["Id"].ToString(), item["Description"].ToString());
					ServiceProviders.Add(obj);
				}
			} 
			catch (Exception ex) 
			{
				throw new Exception("Could not create service providers response. More details: " + ex.Message);
			}
			finally
			{
				items = null;
				token = null;
				jsonObj = null;
			}
		}
	
	}
}

