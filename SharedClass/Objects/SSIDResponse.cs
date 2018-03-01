using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysOn
{
    public class SSIDResponse
    {
        private List<string> ssidList;
        private string _message;
        private bool _success;

        public List<string> SSIDList { get { return ssidList; } }
        public string Message { get { return _message; } }
        public bool Success { get { return _success; } }

        public SSIDResponse(bool Success, string Message)
        {
            _success = Success;
            _message = Message;
            ssidList = new List<string>();
        }

        public SSIDResponse(List<string> SSIDs)
        {
            ssidList = SSIDs ?? new List<string>();
            _success = true;
            _message = "";
        }

        public SSIDResponse(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                JToken token = JToken.Parse(json);

                if ((bool)jsonObject["Success"] != false)
                {
                    JArray hotspots = (JArray)token.SelectToken("SSIDList");

                    ssidList = hotspots.Select(item => item.ToString().Trim()).ToList();

                    _success = (bool)jsonObject["Success"];
                    _message = (string)jsonObject["Message"];
                }

                _success = (bool)jsonObject["Success"];
                _message = (string)jsonObject["Message"];
            }
            catch (Exception ex)
            {
                throw new Exception("Could not translate SSID response. More details: " + ex.Message);
            }
        }
    }
}
