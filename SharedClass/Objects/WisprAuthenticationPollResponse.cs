using System;

namespace AlwaysOn
{
	public class WisprAuthenticationPollResponse
	{		
		//fields

		private static string xmlFieldMessageType = "MessageType";
		private static string xmlFieldResponseCode = "ResponseCode";
		private static string xmlFieldDelay = "Delay";
		private static string xmlFieldLogoffURL = "LogoffURL";


		//properties

		public string MessageType { get; private set; }
		public string ResponseCode { get; private set; }
		public string Delay { get; private set; }
		public string LogoffURL { get; private set; }


		public bool Success { get; private set; }
		public bool Retry { get; private set; }
		public string ResultMessage { get; private set; }

		public int MilliSecondDelay
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Delay))
				{
					return 1000;
				}

				int milliSecondDelay = 0;

				if (!int.TryParse(Delay, out milliSecondDelay))
				{
					return 1000;
				}

				return milliSecondDelay  * 1000;
			}
		}


		//methods

		public WisprAuthenticationPollResponse (string pageHtml)
		{
			Success = false;
			Retry = false;
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
				Delay = GetValue(pageHtml, xmlFieldDelay);
				LogoffURL = GetValue(pageHtml, xmlFieldLogoffURL);
			} 
			catch (Exception ex) 
			{
				throw new Exception("Could not extract WISPr Authentication Poll fields. More details: " + ex.Message);
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
				Retry = false;
				ResultMessage = "Internal malfunction of access gateway (could not determine poll response code)";
				return;
			}

			switch (ResponseCode.Trim())
			{
			case "50":
				Success = true;
				Retry = false;
				ResultMessage = string.Empty;
				return;

			case "100":
				Success = false;
				Retry = false;
				ResultMessage = "100: Login failed (Access Reject)";
				return;

			case "102":
				Success = false;
				Retry = false;
				ResultMessage = "102: RADIUS server error or timeout";
				return;

			case "201":
				Success = false;
				Retry = true;
				ResultMessage = "201: Authentication pending";
				return;

			case "255":
				Success = false;
				Retry = false;
				ResultMessage = "255: Access Gateway internal error";
				return;

			default:
				Success = false;
				Retry = false;
				ResultMessage = "Internal malfunction of access gateway (could not determine poll response code)";
				return;				
			}
		}
	}
}

