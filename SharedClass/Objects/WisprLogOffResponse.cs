using System;

namespace AlwaysOn
{
	public class WisprLogOffResponse
	{
		//fields

		private static string xmlFieldMessageType = "MessageType";
		private static string xmlFieldResponseCode = "ResponseCode";


		//properties

		public string MessageType { get; private set; }
		public string ResponseCode { get; private set; }

		public bool Success { get; private set; }
		public string ResultMessage { get; private set; }


		//methods

		public WisprLogOffResponse (string pageHtml)
		{
			Success = false;
			ResultMessage = string.Empty;

			ExtractFields(pageHtml);
			Verify();
		}


		private void ExtractFields(string pageHtml)
		{
			try 
			{
				MessageType = GetValue(pageHtml, xmlFieldMessageType);
				ResponseCode = GetValue(pageHtml, xmlFieldResponseCode);
			} 
			catch (Exception ex) 
			{
				throw new Exception("Could not extract WISPr Logoff fields. More details: " + ex.Message);
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
			if (string.IsNullOrWhiteSpace(ResponseCode))
			{
				Success = false;
				ResultMessage = "Internal malfunction of access gateway (could not determine logoff response code)";
				return;
			}

			switch (ResponseCode.Trim())
			{
			case "150":
				Success = true;
				ResultMessage = string.Empty;
				return;

			case "255":
				Success = false;
				ResultMessage = "255: Access Gateway internal error";
				return;

			default:
				Success = false;
				ResultMessage = "Internal malfunction of access gateway (could not determine response code)";
				return;				
			}
		}
	}
}

