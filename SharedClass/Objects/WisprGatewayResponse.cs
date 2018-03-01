using System;

namespace AlwaysOn
{
    public class WisprGatewayResponse
    {
        public enum WisprGatewayResponseType { WisprType = 0, JsonType = 1, NotConnected = 2, RefreshType = 3 }

        public class WisprGet
        {
            private static string xmlFieldAccessProcedure = "AccessProcedure";
            private static string xmlFieldAccessLocation = "AccessLocation";
            private static string xmlFieldLocationName = "LocationName";
            private static string xmlFieldLoginURL = "LoginURL";
            private static string xmlFieldAbortLoginURL = "AbortLoginURL";
            private static string xmlFieldMessageType = "MessageType";
            private static string xmlFieldResponseCode = "ResponseCode";

            public string AccessProcedure { get; private set; }
            public string AccessLocation { get; private set; }
            public string LocationName { get; private set; }
            public string LoginURL { get; private set; }
            public string AbortLoginURL { get; private set; }
            public string MessageType { get; private set; }
            public string ResponseCode { get; private set; }
            public string StatusURL { get; private set; }

            public bool ExtractWispr(string pageHtml)
            {
                try
                {
                    AccessProcedure = GetValue(pageHtml, xmlFieldAccessProcedure);
                    AccessLocation = GetValue(pageHtml, xmlFieldAccessLocation);
                    LocationName = GetValue(pageHtml, xmlFieldLocationName);
                    LoginURL = GetURLValue(pageHtml, xmlFieldLoginURL);
                    AbortLoginURL = GetValue(pageHtml, xmlFieldAbortLoginURL);
                    MessageType = GetValue(pageHtml, xmlFieldMessageType);
                    ResponseCode = GetValue(pageHtml, xmlFieldResponseCode);

                    if (AccessProcedure.Length == 0 && AccessLocation.Length == 0 && LocationName.Length == 0 && LoginURL.Length == 0 && AbortLoginURL.Length == 0 && MessageType.Length == 0 && ResponseCode.Length == 0)
                    {
                        return false;
                    }

                    StatusURL = new UriBuilder(LoginURL) { Scheme = "https", Path = "/status", Port = -1 }.ToString();

                    return true;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    return false;
                }
            }

            private string GetURLValue(string pageHtml, string fieldName)
            {
                var openingTag = "<" + fieldName + ">";
                var closingTag = "</" + fieldName + ">";

                var startIndex = pageHtml.IndexOf(openingTag);
                var endIndex = pageHtml.IndexOf(closingTag);

                if (startIndex == -1 || endIndex == -1)
                {
                    var formLoginIndex = pageHtml.ToLower().IndexOf("name=\"login\"");
                    if (formLoginIndex > -1)
                    {
                        var stripped = pageHtml.Substring(formLoginIndex);
                        var actionIndex = stripped.ToLower().IndexOf("action=\"");
                        if (actionIndex > -1)
                        {
                            actionIndex += 8;
                            stripped = stripped.Substring(actionIndex);

                            var endQuoteIndex = stripped.IndexOf("\"");
                            if (endQuoteIndex > -1)
                            {
                                var url = stripped.Substring(0, endQuoteIndex);
                                if (!url.Contains(" ") && url.Contains("http"))
                                {
                                    return url;
                                }
                            }
                        }
                    }

                    return string.Empty; //reliable tag data not found
                }

                var data = pageHtml.Substring(startIndex, endIndex - startIndex);
                data = data.Replace(openingTag, string.Empty);

                return (data ?? "").Trim();
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

                return (data ?? "").Trim();
            }
        }

        public class JsonGet
        {
            public string LinkLogout { get; private set; }
            public string LinkLogin { get; private set; }
            public string LinkStatus { get; private set; }
            public string Username { get; set; }
            public string Ip { get; private set; }
            public string BytesIn { get; private set; }
            public string BytesInNice { get; private set; }
            public string BytesOut { get; private set; }
            public string BytesOutNice { get; private set; }
            public string RemainBytesIn { get; private set; }
            public string RemainBytesOut { get; private set; }
            public string PacketsOut { get; private set; }
            public string PacketsIn { get; private set; }
            public string RemainBytesTotal { get; private set; }
            public string UpTime { get; private set; }
            public string RefreshTimeout { get; private set; }
            public string SessionTimeLeftSecs { get; private set; }
            public string Connected { get; set; }
            public string Mac { get; private set; }
            public string TimeRemaining { get; private set; }
            public string DataRemaining { get; private set; }
            public string ConnectedAs { get; set; }
            public string ConnectedDuration { get; private set; }
            public string CurrentReceived { get; private set; }
            public string CurrentSent { get; private set; }
            public string CurrentTotalUsed { get; private set; }
            public bool IsIPASS { get; private set; }

            public bool ExtractJson(string response)
            {
                try
                {
                    response = response.StartsWith("callback") ? response.Replace("callback(", "").Replace(")", "") : response;

                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<HotspotStatus>(response);

                    LinkLogout = obj.link_logout;
                    Username = obj.username;
                    Ip = obj.ip;
                    BytesIn = obj.bytes_in;
                    BytesInNice = obj.bytes_in_nice;
                    BytesOut = obj.bytes_out;
                    BytesOutNice = obj.bytes_out_nice;
                    RemainBytesIn = obj.remain_bytes_in;
                    RemainBytesOut = obj.remain_bytes_out;
                    PacketsOut = obj.packets_out;
                    PacketsIn = obj.packets_in;
                    RemainBytesTotal = obj.remain_bytes_total;
                    UpTime = obj.uptime;
                    RefreshTimeout = obj.refreshTimeout;
                    SessionTimeLeftSecs = obj.session_time_left_secs;
                    Connected = obj.connected;
                    Mac = obj.mac;

                    SetExtras();
                }
                catch (Exception ex)
                {
                    if ((response.ToLower().Contains("ipass") || response.Contains("IPASS/")) && (response.Contains("connected") && response.Contains("left") && response.Contains("refresh:")))
                    {
                        var ipassindex = response.IndexOf("IPASS/");
                        if (ipassindex > -1)
                        {
                            var stripped = response.Substring(ipassindex);
                            var aoindex = stripped.IndexOf("@alwayson.co.za");
                            if (aoindex > -1)
                            {
                                stripped = stripped.Substring(0, aoindex + "@alwayson.co.za".Length);

                                Username = stripped;

                                ConnectedAs = Username;
                                Connected = "true";
                            }
                        }

                        IsIPASS = true;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            private void SetExtras()
            {
                LinkLogin = new UriBuilder(LinkLogout) { Scheme = "https", Path = "/login", Port = -1 }.ToString();
                LinkStatus = new UriBuilder(LinkLogout) { Scheme = "https", Path = "/status", Port = -1 }.ToString();

                TimeRemaining = ReadableSeconds(SessionTimeLeftSecs);
                DataRemaining = CheckData(RemainBytesTotal, RemainBytesOut);
                ConnectedAs = Username;
                ConnectedDuration = UpTime;
                CurrentReceived = ReadableBytes(BytesOut);
                CurrentSent = ReadableBytes(BytesIn);

                BytesOut = string.IsNullOrEmpty(BytesOut) ? "0" : BytesOut;
                BytesIn = string.IsNullOrEmpty(BytesIn) ? "0" : BytesIn;

                CurrentTotalUsed = ReadableBytes((Convert.ToDouble(BytesOut) + Convert.ToDouble(BytesIn)).ToString());
            }

            private string ReadableSeconds(string Seconds)
            {
                //if (string.IsNullOrEmpty(Seconds)) return "Unlimited";
                double dSec = 0;
                if (double.TryParse(Seconds, out dSec))
                {
                    if (dSec > 0)
                    {
                        var d = Math.Floor(dSec / (60 * 60 * 24));
                        var h = Math.Floor((dSec % (60 * 60 * 24)) / (60 * 60));
                        var m = Math.Floor((dSec % (60 * 60)) / (60));
                        var s = Math.Floor((dSec % (60)));
                        return d + "d " + h + "h" + m + "m" + s + "s";
                    }
                    else
                        return "Unlimited";
                }
                else
                    return "Unlimited";
            }

            private string CheckData(string RemainingBytesTotal, string RemainingBytesOut)
            {
                if (ReadableBytes((RemainingBytesTotal)) != "Unlimited" && RemainingBytesTotal != "")
                    return ReadableBytes(RemainingBytesTotal);

                if (ReadableBytes((RemainingBytesOut)) != "Unlimited" && RemainingBytesOut != "")
                    return ReadableBytes((RemainingBytesOut));

                return "Unlimited";
            }

            private string ReadableBytes(string Bytes)
            {
                //if (string.IsNullOrEmpty(Bytes)) return "Unlimited";
                double dBytes = 0;
                if (double.TryParse(string.IsNullOrEmpty(Bytes) ? "0" : Bytes, out dBytes))
                {
                    var s = new string[] { "bytes", "kB", "MB", "GB", "TB", "PB" };
                    if (dBytes == 0)
                    {
                        return dBytes.ToString("N2") + " " + s[0];
                    }
                    else
                    {
                        var e = Math.Floor(Math.Log(dBytes) / Math.Log(1000));
                        return (dBytes / Math.Pow(1000, Math.Floor(e))).ToString("N2") + " " + s[(int)e];
                    }
                }
                else
                    return "Unlimited";
            }

            private class HotspotStatus
            {
                public string link_logout { get; set; }
                public string username { get; set; }
                public string ip { get; set; }
                public string bytes_in { get; set; }
                public string bytes_in_nice { get; set; }
                public string bytes_out { get; set; }
                public string bytes_out_nice { get; set; }
                public string remain_bytes_in { get; set; }
                public string remain_bytes_out { get; set; }
                public string packets_out { get; set; }
                public string packets_in { get; set; }
                public string remain_bytes_total { get; set; }
                public string uptime { get; set; }
                public string refreshTimeout { get; set; }
                public string session_time_left_secs { get; set; }
                public string connected { get; set; }
                public string mac { get; set; }
            }
        }

        public class RefreshGet
        {
            public string RefreshURL { get; private set; }

            public bool ExtractRefreshURL(string pageHtml)
            {
                RefreshURL = GetRefreshURL(pageHtml);
                return !string.IsNullOrEmpty(RefreshURL);
            }

            private string GetRefreshURL(string pageHtml)
            {
                var refresh = "refresh";
                var content = "url=";

                var refreshIndex = pageHtml.ToLower().IndexOf(refresh);
                if (refreshIndex == -1) { return ""; }
                var contentIndex = pageHtml.ToLower().IndexOf(content);
                if (contentIndex == -1) { return ""; }

                var tPage = pageHtml.Substring(contentIndex + content.Length)
                                    .Replace("'", " ")
                                    .Replace("\"", " ");

                return tPage.Substring(0, tPage.IndexOf(" ")).Trim();
            }
        }

        public WisprGet Wispr;
        public JsonGet Json;
        public RefreshGet Refresh;

        public WisprGatewayResponseType wisprGatewayResponseType { get; private set; } = WisprGatewayResponseType.NotConnected;

        public WisprGatewayResponse(string response)
        {
            var wg = new WisprGet();
            var jg = new JsonGet();
            var rr = new RefreshGet();

            if (wg.ExtractWispr(response))
            {
                //Then its Wispr Code
                wisprGatewayResponseType = WisprGatewayResponseType.WisprType;
                Wispr = wg;
            }
            else if (jg.ExtractJson(response))
            {
                //Then its Json Code
                wisprGatewayResponseType = WisprGatewayResponseType.JsonType;
                Json = jg;
            }
            else if (rr.ExtractRefreshURL(response))
            {
                //Then its a http-equiv refresh
                wisprGatewayResponseType = WisprGatewayResponseType.RefreshType;
                Refresh = rr;
            }
            else
            {
                //Not connected to a gateway
                wisprGatewayResponseType = WisprGatewayResponseType.NotConnected;
            }
            wg = null;
            jg = null;
            rr = null;
        }
    }
}

