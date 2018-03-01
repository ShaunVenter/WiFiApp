using System;

namespace AlwaysOn
{
    public class WisprConnectionStatusResponse
    {
        //fields

        private static string jsonResponseContainer = "callback";

        private static string jsonLinkLogout = "'link_logout'";
        private static string jsonUsername = "'username'";
        private static string jsonIp = "'ip'";

        private static string jsonBytesIn = "'bytes_in'";
        private static string jsonBytesInNice = "'bytes_in_nice'";

        private static string jsonBytesOut = "'bytes_out'";
        private static string jsonBytesOutNice = "'bytes_out_nice'";

        private static string jsonRemainBytesIn = "'remain_bytes_in'";
        private static string jsonRemainBytesOut = "'remain_bytes_out'";

        private static string jsonPacketsOut = "'packets_out'";
        private static string jsonPacketsIn = "'packets_in'";

        private static string jsonRemainBytesTotal = "'remain_bytes_total'";
        private static string jsonUpTime = "'uptime'";
        private static string jsonRefreshTimeout = "'refreshTimeout'";
        private static string jsonSessionTimeLeftSecs = "'session_time_left_secs'";
        private static string jsonConnected = "'connected'";
        private static string jsonMac = "'mac'";


        //properties

        public string LinkLogout { get; private set; }
        public string Username { get; private set; }
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
        public string Connected { get; private set; }
        public string Mac { get; private set; }

        //extra properties
        public string TimeRemaining { get; private set; }
        public string DataRemaining { get; private set; }
        public string ConnectedAs { get; private set; }
        public string ConnectedDuration { get; private set; }
        public string CurrentReceived { get; private set; }
        public string CurrentSent { get; private set; }
        public string CurrentTotalUsed { get; private set; }

        public bool HasActiveConnection { get; private set; }

        public bool Success { get; private set; }
        public string ResultMessage { get; private set; }


        //methods

        public WisprConnectionStatusResponse(bool Success, string Error)
        {
            HasActiveConnection = false;
            this.Success = Success;
            ResultMessage = Error;
        }

        public WisprConnectionStatusResponse(string json)
        {
            HasActiveConnection = false;
            Success = true;
            ResultMessage = string.Empty;

            ExtractFields(json);
            if (Verify())
            {
                SetExtras();
            }
        }

        private void ExtractFields(string json)
        {
            try
            {
                LinkLogout = GetValue(json, jsonLinkLogout);
                Username = GetValue(json, jsonUsername);
                Ip = GetValue(json, jsonIp);

                BytesIn = GetValue(json, jsonBytesIn);
                BytesInNice = GetValue(json, jsonBytesInNice);

                BytesOut = GetValue(json, jsonBytesOut);
                BytesOutNice = GetValue(json, jsonBytesOutNice);

                RemainBytesIn = GetValue(json, jsonRemainBytesIn);
                RemainBytesOut = GetValue(json, jsonRemainBytesOut);

                PacketsOut = GetValue(json, jsonPacketsOut);
                PacketsIn = GetValue(json, jsonPacketsIn);

                RemainBytesTotal = GetValue(json, jsonRemainBytesTotal);
                UpTime = GetValue(json, jsonUpTime);
                RefreshTimeout = GetValue(json, jsonRefreshTimeout);
                SessionTimeLeftSecs = GetValue(json, jsonSessionTimeLeftSecs);
                Connected = GetValue(json, jsonConnected);
                Mac = GetValue(json, jsonMac);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not extract WISPr Connection Status fields. More details: " + ex.Message);
            }
        }

        private string GetValue(string json, string fieldName)
        {
            try
            {
                if (json.IndexOf(jsonResponseContainer) != 0) //no active connection
                {
                    return string.Empty;
                }

                var startIndex = json.IndexOf(fieldName);
                var endIndex = json.IndexOf(",", startIndex);

                if (startIndex == -1 || endIndex == -1)
                {
                    return string.Empty; //reliable tag data not found
                }

                var dataKeyValuePair = json.Substring(startIndex, endIndex - startIndex);

                var seperatorIndex = dataKeyValuePair.IndexOf(":") + 1;
                var dataValue = dataKeyValuePair.Substring(seperatorIndex, dataKeyValuePair.Length - seperatorIndex);

                dataValue = dataValue.Replace("\"", string.Empty);

                if (string.IsNullOrWhiteSpace(dataValue))
                {
                    return string.Empty;
                }

                return dataValue.Trim();
            }
            catch (Exception ex)
            {
                HasActiveConnection = false;
                Success = false;
                ResultMessage = ex.Message;
            }

            return string.Empty;
        }

        private bool Verify()
        {
            if (!Success)
            {
                return false;
            }

            //active connection check

            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(LinkLogout))
            {
                HasActiveConnection = true;
            }

            //all good

            Success = true;
            ResultMessage = string.Empty;

            return Success;
        }

        private void SetExtras()
        {
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
            if (double.TryParse(Bytes, out dBytes))
            {
                var s = new string[] { "bytes", "kB", "MB", "GB", "TB", "PB" };
                var e = Math.Floor(Math.Log(dBytes) / Math.Log(1000));
                return (dBytes / Math.Pow(1000, Math.Floor(e))).ToString("N2") + " " + s[(int)e];
            }
            else
                return "Unlimited";
        }
    }
}

