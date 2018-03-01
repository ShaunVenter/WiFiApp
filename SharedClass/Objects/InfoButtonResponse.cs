using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AlwaysOn.Objects;

namespace AlwaysOn
{
    public class InfoButtonResponse
    {
        private string _message;
        private bool _success;


        private string _title;
        private string _url;

        public string Message { get { return _message; } }
        public bool Success { get { return _success; } }
        public string Title { get { return _title; } }
        public string URL { get { return _url; } }

        public InfoButtonResponse(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                //var infoButton = (object)jsonObject["InfoButton"];
                
                var obj = JsonConvert.SerializeObject((object)jsonObject["InfoButton"]);
                var infoButtonObject = JsonConvert.DeserializeObject<InfoButton>(obj);

                if (obj == "null")
                {
                    _success = (bool)jsonObject["Success"];
                    _message = (string)jsonObject["Message"];
                }
                else
                {
                    _success = (bool)jsonObject["Success"];
                    _message = (string)jsonObject["Message"];

                    _title = string.IsNullOrWhiteSpace(infoButtonObject.Title) ? string.Empty : infoButtonObject.Title;
                    _url = string.IsNullOrWhiteSpace(infoButtonObject.Url) ? string.Empty : infoButtonObject.Url;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not translate info button. More details: " + ex.Message);
            }
        }
    }
}

