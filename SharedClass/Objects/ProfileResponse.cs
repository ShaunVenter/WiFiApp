using System;
using Newtonsoft.Json.Linq;

namespace AlwaysOn
{
	public class ProfileResponse
	{
		private string _message;
		private bool _success;

		public string Message { get { return _message; } }
		public bool Success { get { return _success; } }

		public ProfileResponse (string json)
		{
			try {
				var jsonObject = JObject.Parse(json);

				_success = (bool)jsonObject["Success"];
				_message = (string)jsonObject["Message"];

			} catch (Exception ex) {
				throw new Exception("Could not translate user reset password response. More details: " + ex.Message);
			}
		}
	}
}

