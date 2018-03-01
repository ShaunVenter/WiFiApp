using AlwaysOn;
using AlwaysOn.Objects;
using AlwaysOn_iOS.Objects;
using CoreFoundation;
using Foundation;
using NetworkExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AlwaysOn_iOS
{
    public class HotspotHelper : NEHotspotHelper
    {
        public static string WisprTAG { get; set; } = "WISPAccessGatewayParam";
        private static string GatewayLoginURL { get; set; }
        private static string GatewayLogoutURL { get; set; }
        private static string GatewayStatusURL { get; set; }
        private static string GatewayRefreshURL { get; set; }
        private static List<string> SSIDs
        {
            get
            {
                try
                {
                    var uniqueList = ScanResults.Select(sr => sr.Ssid).Distinct().ToList();
                    if (!string.IsNullOrWhiteSpace(ConnectedSSID) && !uniqueList.Contains(ConnectedSSID))
                    {
                        uniqueList.Add(ConnectedSSID);
                    }

                    var SSIDlist = BackendProvider_iOS.LookupSSIDList(uniqueList).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                    return SSIDlist;
                }
                catch //(Exception ex)
                {
                    return new List<string>();
                }
            }
        }
        private static List<NEHotspotNetwork> ScanResults { get; set; } = new List<NEHotspotNetwork>();
        private static string ConnectedSSID { get; set; }

        public HotspotHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
            //ServicePointManager.DnsRefreshTimeout = 0;

            NEHotspotHelperOptions options = new NEHotspotHelperOptions(NSDictionary.FromObjectAndKey(new NSString("• AlwaysOn Avalable Here"), NEHotspotHelperOptionInternal.DisplayName));
            //DispatchQueue queue = new DispatchQueue("com.alwayson.wifi", true);
            bool isAvailable = Register(options, DispatchQueue.MainQueue, HotspotHandler);
        }

        private void HotspotHandler(NEHotspotHelperCommand cmd)
        {
            try
            {
                NEHotspotHelperResponse response;
                var settings = BackendProvider_iOS.GetUserSettings;
                var user = BackendProvider_iOS.GetUser;

                if (user != null && user.RememberMe && settings.Notifications)
                {
                    ScanResults = (cmd.NetworkList ?? new NEHotspotNetwork[0]).ToList();
                    var ConnectedNetwork = SupportedNetworkInterfaces?.Last();
                    var tempConnectedSSID = ConnectedNetwork?.Ssid;
                    ConnectedSSID = string.IsNullOrEmpty(tempConnectedSSID) ? ConnectedSSID : tempConnectedSSID;
                    var mySSIDs = SSIDs;

                    switch (cmd.CommandType)
                    {
                        case NEHotspotHelperCommandType.FilterScanList: // Set confidence on all hotspots containing "alwayson"
                            {
                                response = cmd.CreateResponse(NEHotspotHelperResult.Success);

                                if (mySSIDs.Count > 0)
                                {
                                    var newList = ScanResults.Where(n => mySSIDs.Contains(n.Ssid)).ToList();
                                    newList.ForEach(n => n.SetConfidence(NEHotspotHelperConfidence.High));

                                    if (newList.Count > 0)
                                    {
                                        if (!mySSIDs.Contains(ConnectedSSID))
                                        {
                                            var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                                            hotspothelperdb.SessionID = "";
                                            hotspothelperdb.AuthenticatedUsername = "";
                                            hotspothelperdb.IsAccuris = false;

                                            DBHelper.Database.HotspotHelperSet(hotspothelperdb);

                                            AlwaysOnNotifications.ShowNotification(new AlwaysOnNotificationParameters()
                                            {
                                                title = "AlwaysOn Available",
                                                subtitle = "",
                                                body = "You are in reach of AlwaysOn. Get connected to the internet." + ("\nSSID:\n- " + string.Join("\n- ", mySSIDs)),
                                                badge = 1,
                                                status = AlwaysOnNotificationStatus.Available
                                            });
                                        }
                                        else
                                        {
                                            if (mySSIDs != null && mySSIDs.Contains(ConnectedSSID))
                                            {
                                                if (WisprGatewayConnected)
                                                {
                                                    AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperHasAlwaysOn.ToString());

                                                    if (DBHelper.Database.HotspotHelperGet().AuthenticatedUsername.Length == 0)
                                                    {
                                                        Authenticate();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    response.SetNetworkList(newList.ToArray());
                                }

                                response.Deliver();

                                break;
                            }
                        case NEHotspotHelperCommandType.Evaluate: // First Step "Evaluate" after a user connected to a AlwaysOn Hotspot
                        case NEHotspotHelperCommandType.PresentUI:
                            {
                                if (mySSIDs != null && mySSIDs.Contains(ConnectedSSID))
                                {
                                    ConnectedNetwork.SetConfidence(NEHotspotHelperConfidence.High);
                                    response = cmd.CreateResponse(NEHotspotHelperResult.Success);
                                    response.SetNetwork(ConnectedNetwork);
                                }
                                else
                                {
                                    var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                                    hotspothelperdb.SessionID = "";
                                    hotspothelperdb.AuthenticatedUsername = "";
                                    hotspothelperdb.IsAccuris = false;

                                    DBHelper.Database.HotspotHelperSet(hotspothelperdb);

                                    response = cmd.CreateResponse(NEHotspotHelperResult.UnsupportedNetwork); // Not an AlwaysOn Hotspot
                                }

                                response.Deliver();

                                break;
                            }
                        case NEHotspotHelperCommandType.Authenticate: // Second Step "Authenticate" after Evaluate is done.
                        case NEHotspotHelperCommandType.Maintain: // Third Step "Maintain" - random step
                            {
                                if (mySSIDs != null && mySSIDs.Contains(ConnectedSSID))
                                {
                                    ConnectedNetwork.SetConfidence(NEHotspotHelperConfidence.High);
                                    response = cmd.CreateResponse(NEHotspotHelperResult.Success);
                                    response.SetNetwork(ConnectedNetwork);

                                    if (WisprGatewayConnected)
                                    {
                                        AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperHasAlwaysOn.ToString());

                                        if (DBHelper.Database.HotspotHelperGet().AuthenticatedUsername.Length == 0)
                                        {
                                            Authenticate();
                                        }
                                    }
                                }
                                else
                                {
                                    var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                                    hotspothelperdb.SessionID = "";
                                    hotspothelperdb.AuthenticatedUsername = "";
                                    hotspothelperdb.IsAccuris = false;

                                    DBHelper.Database.HotspotHelperSet(hotspothelperdb);

                                    response = cmd.CreateResponse(NEHotspotHelperResult.UnsupportedNetwork); // Not an AlwaysOn Hotspot
                                }

                                response.Deliver();

                                break;
                            }
                        case NEHotspotHelperCommandType.Logoff:
                        case NEHotspotHelperCommandType.None:
                        default:
                            {
                                response = cmd.CreateResponse(NEHotspotHelperResult.Success);
                                response.Deliver();
                                break;
                            }
                    }
                }
                else
                {
                    response = cmd.CreateResponse(NEHotspotHelperResult.UnsupportedNetwork); // Not logged in
                    response.Deliver();
                }
            }
            catch (Exception ex)
            {
                var response = cmd.CreateResponse(NEHotspotHelperResult.UnsupportedNetwork); // Not logged in or Error
                response.Deliver();
            }
        }

        public void Authenticate()
        {
            try
            {
                var firstAvailablePackage = BackendProvider_iOS.FirstAvailablePackage;
                if (firstAvailablePackage != null)
                {
                    AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperAuthenticate.ToString());

                    var connectResponse = ConnectPackage(firstAvailablePackage.LoginUserName, firstAvailablePackage.LoginPassword, false);
                    if (connectResponse.Success)
                    {
                        AlwaysOnNotifications.ShowNotification(new AlwaysOnNotificationParameters()
                        {
                            title = "AlwaysOn Connected",
                            subtitle = "",
                            body = "You are connected to the internet. Package " + firstAvailablePackage.GroupName + " (" + firstAvailablePackage.LoginUserName + ") is now active.",
                            badge = 1,
                            status = AlwaysOnNotificationStatus.Connected
                        });
                    }
                    else
                    {
                        AlwaysOnNotifications.ShowNotification(new AlwaysOnNotificationParameters()
                        {
                            title = "AlwaysOn Not Connected",
                            subtitle = connectResponse.ResponseCode,
                            body = connectResponse.ResultMessage,
                            badge = 1,
                            status = AlwaysOnNotificationStatus.NotConnected
                        });
                    }
                }
                else
                {
                    AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperNoPackages.ToString());

                    AlwaysOnNotifications.ShowNotification(new AlwaysOnNotificationParameters()
                    {
                        title = "AlwaysOn No Packages",
                        subtitle = "",
                        body = "No available packages to use.",
                        badge = 1,
                        status = AlwaysOnNotificationStatus.NotConnected
                    });
                }

            }
            catch
            {
                //AlwaysOnNotifications.ShowNotification(new AlwaysOnNotificationParameters()
                //{
                //    title = "AlwaysOn Exception",
                //    subtitle = ex.Message,
                //    body = ex.StackTrace,
                //    badge = 1,
                //    status = AlwaysOnNotificationStatus.NotConnected
                //});
            }
        }

        private static string DefaultGatewayIP
        {
            get
            {
                var DefaultGatewayIP = "";
                try
                {
                    foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            foreach (var addrInfo in netInterface.GetIPProperties().GatewayAddresses)
                            {
                                DefaultGatewayIP = addrInfo.Address.ToString();
                            }
                        }
                    }
                }
                catch { }

                return DefaultGatewayIP;
            }
        }

        public static void WisprGatewayStatus(Action<WisprGatewayResponse.JsonGet> Callback)
        {
            Task.Factory.StartNew(() =>
            {
                var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                if (hotspothelperdb.IsAccuris)
                {
                    var json = new WisprGatewayResponse.JsonGet();
                    if (!string.IsNullOrEmpty(hotspothelperdb.AuthenticatedUsername))
                    {
                        json.Username = hotspothelperdb.AuthenticatedUsername;
                        json.ConnectedAs = json.Username;
                        json.Connected = "true";

                        Callback?.Invoke(json);

                        return;
                    }
                }
                else
                {
                    using (var client = new HotspotHelperWebClient())
                    {
                        client.Timeout = 1000;
                        try
                        {
                            var text = client.DownloadString(GatewayStatusURL);
                            var json = new WisprGatewayResponse(text).Json;
                            json.Username = json.Username.Replace("IPASS/", "").Replace("@alwayson.co.za", "");

                            Callback?.Invoke(json);

                            return;
                        }
                        catch //(Exception ex)
                        {
                        }
                    }
                }

                Callback?.Invoke(null);

            }, TaskCreationOptions.LongRunning);
        }

        public static bool DisconnectPackage()
        {
            var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
            if (hotspothelperdb.IsAccuris)
            {
                var logoutresponse = BackendProvider.AccurisLogout(AppDelegate.ApiKey, hotspothelperdb.SessionID);
                if (logoutresponse.Success)
                {
                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");

                    return true;
                }
            }
            else
            {
                using (var client = new HotspotHelperWebClient())
                {
                    try
                    {
                        var text = client.DownloadString(GatewayLogoutURL);
                        var wr = new WisprLogOffResponse(text);
                        if (wr.Success)
                        {
                            DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");

                            return true;
                        }
                    }
                    catch (Exception ex) { var exception = ex; }
                }
            }

            return false;
        }

        public static WisprAuthenticationResponse ConnectPackage(string Username, string Password, bool WithCallback)
        {
            var waresponse = new WisprAuthenticationResponse(false, "Timed out");

            var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
            if (hotspothelperdb.IsAccuris)
            {
                var loginresponse = BackendProvider.AccurisLogin(AppDelegate.ApiKey, Username, Password, hotspothelperdb.SessionID);
                if (loginresponse.Success)
                {
                    waresponse = new WisprAuthenticationResponse(true, "");

                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername(Username.Replace("IPASS/", "").Replace("@alwayson.co.za", ""));
                }
                else
                {
                    waresponse = new WisprAuthenticationResponse(false, loginresponse.Message);

                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                }
            }
            else
            {
                var LoginURL = GatewayLoginURL;// + (GatewayLoginURL.Contains("?") ? "&" : "?") + "username=" + Username + "@alwayson.co.za&password=" + Password;

                if (!System.Uri.TryCreate(LoginURL, UriKind.Absolute, out System.Uri uriResult) || (uriResult.Scheme != System.Uri.UriSchemeHttp && uriResult.Scheme != System.Uri.UriSchemeHttps))
                {
                    return waresponse;
                }

                try
                {
                    var text = "";

                    var usernme = (LoginURL.ToLower().Contains("ipass") ? "IPASS/" : "") + Username + "@alwayson.co.za";

                    try
                    {
                        if (new System.Uri(LoginURL).Scheme == System.Uri.UriSchemeHttp)
                        {
                            using (var client = new HotspotHelperWebClient() { Timeout = 10000 })
                            {
                                text = System.Text.Encoding.UTF8.GetString(client.UploadValues(LoginURL, new System.Collections.Specialized.NameValueCollection
                                {
                                    ["UserName"] = usernme,
                                    ["Password"] = Password
                                }));
                            }
                        }
                        else
                        {
                            text = BackendProvider.HTTPClientModernHTTPPost(LoginURL, (LoginURL.Contains("?") ? "&" : "?") + "UserName=" + usernme + "&Password=" + Password);
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    waresponse = new WisprAuthenticationResponse(text);
                    if (waresponse.Success)
                    {
                        DBHelper.Database.HotspotHelperSetAuthenticatedUsername(Username.Replace("IPASS/", "").Replace("@alwayson.co.za", ""));
                    }
                    else
                    {
                        if (waresponse.ResultMessage.Contains("You are already logged in"))
                        {
                            DisconnectPackage();
                        }

                        DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (waresponse != null && waresponse.Success && WithCallback)
            {
                //CheckState(DBHelper.Database.HotspotHelperGet().AuthenticatedUsername == "" ? State.ConnectedToSSID : State.Authenticated);
            }

            return waresponse;
        }

        private static bool ConnectedAlwaysOnGateway
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ConnectedSSID) || !SSIDs.Contains(ConnectedSSID))
                    return false;

                return WisprGatewayConnected;
            }
        }

        static object gwconnectedlocker = new object();
        private static bool WisprGatewayConnected
        {
            get
            {
                lock (gwconnectedlocker)
                {
                    string text = "";
                    string gwURL = "";

                    var hotspothelperdb = DBHelper.Database.HotspotHelperGet();

                    if (string.IsNullOrEmpty(hotspothelperdb.SessionID))
                    {
                        DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");

                        using (var client = new TimeoutWebClient())
                        {
                            try
                            {
                                gwURL = "http://" + DefaultGatewayIP;
                                var urlList = new List<string> { gwURL };
                                var gotWispr = false;

                                try
                                {
                                    text = client.DownloadString(gwURL);
                                    gotWispr = text.Contains(WisprTAG);
                                }
                                catch //(Exception ex)
                                {
                                }

                                var redirected = 0;
                                while (client.ResponseStatusCode == HttpStatusCode.Redirect && redirected <= 3)
                                {
                                    if (client.ResponseHeaders != null && !string.IsNullOrEmpty(client.ResponseHeaders[HttpResponseHeader.Location]))
                                    {
                                        gwURL = client.ResponseHeaders[HttpResponseHeader.Location];
                                        urlList.Add(gwURL);

                                        if (!gotWispr)
                                        {
                                            var i = 0;

                                            while (i < 10)
                                            {
                                                try
                                                {
                                                    client.Headers.Add(HttpRequestHeader.Host, new Uri(gwURL).Host);

                                                    text = client.DownloadString(gwURL);
                                                    gotWispr = text.Contains(WisprTAG);
                                                    i = 10;
                                                }
                                                catch (Exception ex)
                                                {
                                                    try
                                                    {
                                                        gwURL = new UriBuilder(gwURL) { Scheme = System.Uri.UriSchemeHttp, Port = -1 }.ToString();
                                                        text = client.DownloadString(gwURL);
                                                        gotWispr = text.Contains(WisprTAG);
                                                        i = 10;
                                                    }
                                                    catch (Exception exs)
                                                    {
                                                        i++;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    redirected++;
                                }

                                var sessionid = "";

                                var sessionidlist = new List<string>();
                                if (urlList.Count > 1)
                                {
                                    sessionidlist = urlList.Where(n => n.Contains("session_id")).ToList();
                                    if (sessionidlist.Count > 0 && !string.IsNullOrEmpty(sessionidlist.FirstOrDefault()))
                                    {
                                        sessionid = StripSessionID(sessionidlist.FirstOrDefault());

                                        hotspothelperdb.SessionID = sessionid ?? hotspothelperdb.SessionID;
                                        DBHelper.Database.HotspotHelperSetSessionID(hotspothelperdb.SessionID);
                                    }
                                }

                                if (!gotWispr)
                                {
                                    if (sessionidlist.Count < 1)
                                    {
                                        try
                                        {
                                            gwURL = "http://" + DefaultGatewayIP + "/status";
                                            urlList.Add(gwURL);

                                            text = client.DownloadString(gwURL);

                                            if (!string.IsNullOrEmpty(text))
                                            {
                                                hotspothelperdb.SessionID = "";
                                                DBHelper.Database.HotspotHelperSetSessionID("");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    else
                                    {
                                        sessionid = sessionidlist.FirstOrDefault();

                                        hotspothelperdb.SessionID = sessionid ?? hotspothelperdb.SessionID;
                                        DBHelper.Database.HotspotHelperSetSessionID(hotspothelperdb.SessionID);
                                    }
                                }
                                else
                                {
                                    text = text.Substring(text.IndexOf("<" + WisprTAG));
                                    text = text.Substring(0, text.IndexOf(WisprTAG + ">") + WisprTAG.Length + 1);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Try above again
                            }
                        }
                    }

                    if (!hotspothelperdb.IsAccuris)
                    {
                        var wr = new WisprGatewayResponse(text);
                        switch (wr.wisprGatewayResponseType)
                        {
                            case WisprGatewayResponse.WisprGatewayResponseType.WisprType:
                                {
                                    GatewayLoginURL = wr.Wispr.LoginURL;
                                    GatewayLogoutURL = wr.Wispr.AbortLoginURL;
                                    GatewayStatusURL = wr.Wispr.StatusURL;
                                    GatewayRefreshURL = null;

                                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                                    return true;
                                }
                            case WisprGatewayResponse.WisprGatewayResponseType.JsonType:
                                {
                                    if (wr.Json.IsIPASS)
                                    {
                                        GatewayStatusURL = new UriBuilder(gwURL) { Scheme = "https", Path = "/status", Port = -1 }.ToString();
                                        GatewayLoginURL = new UriBuilder(gwURL) { Scheme = "https", Path = "/login", Port = -1 }.ToString();
                                        GatewayLogoutURL = new UriBuilder(gwURL) { Scheme = "https", Path = "/logout", Port = -1 }.ToString();

                                        GatewayRefreshURL = null;
                                    }
                                    else
                                    {
                                        GatewayLoginURL = wr.Json.LinkLogin;
                                        GatewayLogoutURL = wr.Json.LinkLogout;
                                        GatewayStatusURL = wr.Json.LinkStatus;
                                        GatewayRefreshURL = null;
                                    }

                                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername(wr.Json?.Username?.Replace("IPASS/", "").Replace("@alwayson.co.za", ""));
                                    return true;
                                }
                            case WisprGatewayResponse.WisprGatewayResponseType.RefreshType:
                                {
                                    GatewayRefreshURL = wr.Refresh.RefreshURL;

                                    return WisprGatewayConnected;
                                }
                            case WisprGatewayResponse.WisprGatewayResponseType.NotConnected:
                            default:
                                {
                                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                                    return false;
                                }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hotspothelperdb.SessionID))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public class StatePackage
        {
            public List<string> AvailableSSIDs { get; set; }
            public string ConnectedSSID { get; set; }
            public bool ConnectedToAlwaysOn { get; set; }
            public string AuthenticatedUsername { get; set; }
            public bool Authenticated { get; set; }
        }

        public static string StripSessionID(string url)
        {
            try
            {
                var param = "session_id=";
                url = url.Substring(url.IndexOf(param) + param.Length);
                url = url.Substring(0, url.IndexOf("&"));

                return url;
            }
            catch
            {
                return url;
            }
        }

        public static void GetConnectivityStatePackage(Action<StatePackage> Callback)
        {
            Task.Factory.StartNew(() =>
            {
                if (string.IsNullOrEmpty(HotspotHelper.ConnectedSSID))
                {
                    var ConnectedNetwork = SupportedNetworkInterfaces?.Last();
                    var tempConnectedSSID = ConnectedNetwork?.Ssid;
                    HotspotHelper.ConnectedSSID = tempConnectedSSID;
                }

                var sp = new StatePackage()
                {
                    AvailableSSIDs = SSIDs,
                    ConnectedSSID = HotspotHelper.ConnectedSSID,
                    ConnectedToAlwaysOn = ConnectedAlwaysOnGateway
                };
                var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                if (!sp.ConnectedToAlwaysOn)
                {
                    hotspothelperdb.AuthenticatedUsername = "";
                    hotspothelperdb.IsAccuris = false;
                    hotspothelperdb.SessionID = "";

                    DBHelper.Database.HotspotHelperSet(hotspothelperdb);
                }
                sp.AuthenticatedUsername = hotspothelperdb.AuthenticatedUsername;
                sp.Authenticated = sp.AuthenticatedUsername.Length > 0;

                Callback?.Invoke(sp);
            }, TaskCreationOptions.LongRunning);
        }
    }
}