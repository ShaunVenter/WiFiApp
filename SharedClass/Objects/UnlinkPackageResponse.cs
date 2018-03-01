using System;
using Newtonsoft.Json.Linq;

namespace AlwaysOn
{
    public class UnlinkPackageResponse
    {
        //properties

        public bool Success { get; set; }
        public string Message { get; set; }


        //methods

        public UnlinkPackageResponse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception("No response from Unlink Package request");
            }

            try
            {
                var jsonObj = JObject.Parse(json);

                Success = (bool)(jsonObj["Success"] ?? false);
                Message = (jsonObj["Message"] ?? "").ToString().Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Unlink Package response translation error. More details: " + ex.Message);
            }
        }
    }
}

