using AlwaysOn.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlwaysOn
{
    public class HotspotResponse
    {
        private List<Hotspot> hotspotList;
        private string _message;
        private bool _success;

        public List<Hotspot> HotspotList { get { return hotspotList; } }
        public string Message { get { return _message; } }
        public bool Success { get { return _success; } }
        
        public HotspotResponse(string json)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                JToken token = JToken.Parse(json);

                if ((bool)jsonObject["Success"] != false)
                {
                    JArray hotspots = (JArray)token.SelectToken("HotspotMarkers");

                    hotspotList = hotspots.Select(item => new Hotspot()
                    {
                        data = item["data"].ToString().Trim(),
                        lat = item["lat"].ToString().Trim(),
                        lng = item["lng"].ToString().Trim(),
                        superwifi = (bool)item["superwifi"],
                        international = (bool)item["international"],
                        distance = double.Parse(item["distanceinkilometers"].ToString().Trim())
                    })
                    .OrderBy(n => n.distance)
                    .ToList();

                    _success = (bool)jsonObject["Success"];
                    _message = (string)jsonObject["Message"];
                }

                _success = (bool)jsonObject["Success"];
                _message = (string)jsonObject["Message"];
            }
            catch (Exception ex)
            {
                throw new Exception("Could not translate hotspot response. More details: " + ex.Message);
            }
        }
    }
}

