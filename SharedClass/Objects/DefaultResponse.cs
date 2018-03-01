using Newtonsoft.Json.Linq;
using System;

namespace AlwaysOn
{
    public class DefaultResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        
        public DefaultResponse(bool Success, string Message)
        {
            this.Success = Success;
            this.Message = Message;
        }

        public DefaultResponse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception("No response from request");
            }

            try
            {
                var jsonObj = JObject.Parse(json);

                Success = (bool)(jsonObj["Success"] ?? false);
                Message = (jsonObj["Message"] ?? "").ToString().Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Response translation error. More details: " + ex.Message);
            }
        }
    }
}
