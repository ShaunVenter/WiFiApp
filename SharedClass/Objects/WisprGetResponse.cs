using System;

namespace AlwaysOn
{
	public class WisprGetResponse
	{
		//fields

		private static string xmlFieldAccessProcedure = "AccessProcedure";
		private static string xmlFieldAccessLocation = "AccessLocation";
		private static string xmlFieldLocationName = "LocationName";
		private static string xmlFieldLoginURL = "LoginURL";
		private static string xmlFieldAbortLoginURL = "AbortLoginURL";
		private static string xmlFieldMessageType = "MessageType";
		private static string xmlFieldResponseCode = "ResponseCode";


		//properties

		public string AccessProcedure { get; private set; }
		public string AccessLocation { get; private set; }
		public string LocationName { get; private set; }
		public string LoginURL { get; private set; }
		public string AbortLoginURL { get; private set; }
		public string MessageType { get; private set; }
		public string ResponseCode { get; private set; }

		public bool Success { get; private set; }
		public string ResultMessage { get; private set; }


		//methods

		public WisprGetResponse (string pageHtml)
		{
			ExtractFields(pageHtml);
			Verify();
		}

		private void ExtractFields(string pageHtml)
		{
			try 
			{
				AccessProcedure = GetValue(pageHtml, xmlFieldAccessProcedure);
				AccessLocation = GetValue(pageHtml, xmlFieldAccessLocation);
				LocationName = GetValue(pageHtml, xmlFieldLocationName);
				LoginURL = GetValue(pageHtml, xmlFieldLoginURL);
				AbortLoginURL = GetValue(pageHtml, xmlFieldAbortLoginURL);
				MessageType = GetValue(pageHtml, xmlFieldMessageType);
				ResponseCode = GetValue(pageHtml, xmlFieldResponseCode);
			} 
			catch (Exception ex) 
			{
				throw new Exception("Could not extract WISPr GET fields. More details: " + ex.Message);
			}
		}

		private string GetValue(string pageHtml, string fieldName)
		{
			var openingTag = "<" + fieldName + ">";
			var closingTag = "</" + fieldName + ">";

			var startIndex = pageHtml.IndexOf(openingTag);
			var endIndex = pageHtml.IndexOf(closingTag);

			if (startIndex == -1 || endIndex == -1)
			{
				return string.Empty; //reliable tag data not found
			}

			var data = pageHtml.Substring(startIndex, endIndex - startIndex);
			data = data.Replace(openingTag, string.Empty);

			if (string.IsNullOrWhiteSpace(data))
			{
				return string.Empty;
			}

			return data;
		}

		private void Verify()
		{
			//all data received 

			if (string.IsNullOrWhiteSpace(AccessProcedure)
				|| string.IsNullOrWhiteSpace(AccessLocation)
				|| string.IsNullOrWhiteSpace(LocationName)
				|| string.IsNullOrWhiteSpace(LoginURL)
				//|| string.IsNullOrWhiteSpace(AbortLoginURL)
				|| string.IsNullOrWhiteSpace(MessageType)
				|| string.IsNullOrWhiteSpace(ResponseCode))
			{
				Success = false;
				ResultMessage = "Internal malfunction of access gateway (not all data was received)";
				return;
			}


			//response code is not 0

			if (ResponseCode.Trim() != "0")
			{
				Success = false;
				ResultMessage = "Internal malfunction of access gateway (response code " + ResponseCode + " received)";
				return;
			}


			//all good

			Success = true;
			ResultMessage = string.Empty;
		}
	}
}

