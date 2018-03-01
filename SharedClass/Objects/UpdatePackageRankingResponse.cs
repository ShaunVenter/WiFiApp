using System;
using Newtonsoft.Json.Linq;

namespace AlwaysOn
{
    public class UpdatePackageRankingResponse
    {
        //properties

        public bool Success { get; set; }
        public string Message { get; set; }


        //methods

        public UpdatePackageRankingResponse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new Exception("No response from Update Package Ranking request");
            }

            try
            {
                var jsonObj = JObject.Parse(json);

                Success = (bool)(jsonObj["Success"] ?? false);
                Message = (jsonObj["Message"] ?? "").ToString().Trim();
            }
            catch (Exception ex)
            {
                throw new Exception("Update Package Ranking response translation error. More details: " + ex.Message);
            }
        }
    }
}

