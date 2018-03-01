using AlwaysOn;
using AlwaysOn.Objects;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AlwaysOn_Droid
{
    public class HotspotHelper : BroadcastReceiver
    {
        static Context _context;
        static HotspotHelper helper;

        private static WifiManager Wifi { get; set; }
        private static ConnectivityManager CManager { get; set; }
        private static string GatewayLoginURL { get; set; }
        private static string GatewayLogoutURL { get; set; }
        private static string GatewayStatusURL { get; set; }
        private static string GatewayRefreshURL { get; set; }
        private static bool AuthenticatedOnFirstWiFiOn { get; set; } = false;
        private static State WiFiState { get; set; } = State.None;
        private static bool IsConnected { get; set; } = false;
        private static string ConnectedSSID { get { return Wifi?.ConnectionInfo?.SSID?.Replace("\"", "").Trim() ?? ""; } }
        private static string RegisteredSSID { get; set; }
        private static List<string> SSIDs
        {
            get
            {
                try
                {
                    if (Wifi == null || !Wifi.IsWifiEnabled) return new List<string>();

                    var uniqueList = Wifi.ScanResults.Select(sr => sr.Ssid).Distinct().ToList();
                    if (!string.IsNullOrWhiteSpace(ConnectedSSID) && !uniqueList.Contains(ConnectedSSID))
                    {
                        uniqueList.Add(ConnectedSSID);
                    }

                    var SSIDlist = BackendProvider_Droid.LookupSSIDList(uniqueList).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                    return SSIDlist;
                }
                catch //(Exception ex)
                {
                    return new List<string>();
                }
            }
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

        public static Action<StatePackage> WiFiChanged { get; set; }

        public HotspotHelper(Context context)
        {
            _context = context;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };

            Wifi = (WifiManager)context.GetSystemService(Context.WifiService);
            CManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
        }

        private static void ResetVars()
        {
            GatewayLoginURL = null;
            GatewayLogoutURL = null;
            GatewayStatusURL = null;
            GatewayRefreshURL = null;
            AuthenticatedOnFirstWiFiOn = false;
            WiFiState = State.None;
        }

        public static void RegisterHotspotHelper(Context context)
        {
            if (helper == null)
            {
                helper = new HotspotHelper(context);

                var inf = new IntentFilter();
                inf.AddAction(WifiManager.NetworkStateChangedAction);
                inf.AddAction(WifiManager.WifiStateChangedAction);
                inf.AddAction(WifiManager.ScanResultsAvailableAction);
                inf.AddAction("com.is.alwayson.hotspotservice.keepalive");
                inf.AddAction("com.is.alwayson.hotspotservice.StartScan");

                context.RegisterReceiver(helper, inf);
            }
            else
            {
                context.SendBroadcast(new Intent("com.is.alwayson.hotspotservice.keepalive"));
            }
        }

        static bool keepAliveBusy = false;
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == "com.is.alwayson.hotspotservice.keepalive" && keepAliveBusy) return;

            var user = BackendProvider_Droid.GetUser;
            var userSettings = BackendProvider_Droid.GetUserSettings;
            if (user != null && user.RememberMe)
            {
                if (CManager == null)
                {
                    CManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                }

                switch (intent.Action)
                {
                    case "com.is.alwayson.hotspotservice.keepalive":
                        {
                            //keepAliveBusy = true;

                            //var networkInfo = CManager.GetAllNetworks().ToList().Select(n => CManager.GetNetworkInfo(n)).Where(n => n?.Type == ConnectivityType.Wifi).FirstOrDefault();
                            //if (networkInfo != null)
                            //{
                            //    var ssid = (networkInfo.ExtraInfo ?? "").Replace("\"", "").Trim();

                            //    IsConnected = networkInfo.IsConnected;

                            //    GetConnectivityStatePackage((sp) =>
                            //    {
                            //        if (sp.ConnectedToAlwaysOn && !sp.Authenticated)
                            //        {
                            //            if (!userSettings.DisableConnectionManager)
                            //            {
                            //                WiFiState = State.Reconnecting;

                            //                var networkRequest = new NetworkRequest.Builder().AddTransportType(Android.Net.TransportType.Wifi).Build();
                            //                CManager.RegisterNetworkCallback(networkRequest, new NetworkCallback(context, ssid));
                            //            }
                            //        }
                            //        keepAliveBusy = false;
                            //    });
                            //}
                            //else
                            //{
                            //    keepAliveBusy = false;
                            //}
                            break;
                        }
                    case WifiManager.NetworkStateChangedAction:
                        {
                            var networkInfo = (NetworkInfo)intent.GetParcelableExtra(WifiManager.ExtraNetworkInfo);
                            if (networkInfo != null && networkInfo.Type == ConnectivityType.Wifi && networkInfo.IsConnectedOrConnecting)
                            {
                                CheckState(context, State.Enabled);

                                IsConnected = networkInfo.IsConnected;
                                var ssid = (networkInfo.ExtraInfo ?? "").Replace("\"", "").Trim();

                                var mySSIDs = SSIDs;
                                if (mySSIDs.Contains(ssid) || ssid == "captive_portal_detected")
                                {
                                    if (!userSettings.DisableConnectionManager)
                                    {
                                        if (ssid != RegisteredSSID)
                                        {
                                            RegisteredSSID = ssid;

                                            ResetVars();

                                            var networkRequest = new NetworkRequest.Builder().AddTransportType(Android.Net.TransportType.Wifi).Build();
                                            CManager.RegisterNetworkCallback(networkRequest, new NetworkCallback(context, ssid));
                                        }
                                        else
                                        {
                                            var networkRequest = new NetworkRequest.Builder().AddTransportType(Android.Net.TransportType.Wifi).Build();
                                            CManager.RegisterNetworkCallback(networkRequest, new NetworkCallback(context, ssid));
                                        }
                                    }
                                }
                                else
                                {
                                    if (IsConnected)
                                    {
                                        if (!ConnectedAlwaysOnGateway && mySSIDs.Count > 0)
                                        {
                                            AuthenticatedOnFirstWiFiOn = false;

                                            if (!userSettings.DisableConnectionManager)
                                            {
                                                AlwaysOnNotifications.ShowNotification(context, new AlwaysOnNotificationParameters()
                                                {
                                                    title = "AlwaysOn Available",
                                                    subtitle = "You are in reach of AlwaysOn",
                                                    body = "Get connected to the internet." + ("\nSSID:\n- " + string.Join("\n- ", mySSIDs)),
                                                    badge = 1,
                                                    status = AlwaysOnNotificationStatus.Available
                                                });
                                            }

                                            WiFiState = State.SSIDAvailable;
                                        }
                                    }
                                    else
                                    {
                                        if (HotspotHelperConsts.IsMarshmallow)
                                        {
                                            CManager.BindProcessToNetwork(null);
                                        }
                                        else
                                        {
                                            ConnectivityManager.SetProcessDefaultNetwork(null);
                                        }

                                        var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                                        hotspothelperdb.SessionID = "";
                                        hotspothelperdb.AuthenticatedUsername = "";
                                        hotspothelperdb.IsAccuris = false;

                                        DBHelper.Database.HotspotHelperSet(hotspothelperdb);

                                        if (!userSettings.DisableConnectionManager)
                                        {
                                            Wifi = (WifiManager)context.GetSystemService(Context.WifiService);
                                            Wifi.StartScan();
                                        }
                                    }

                                    GetConnectivityStatePackage((sp) => WiFiChanged?.Invoke(sp));
                                }
                            }
                            else
                            {
                                if (networkInfo == null || !networkInfo.IsConnectedOrConnecting)
                                {
                                    if (HotspotHelperConsts.IsMarshmallow)
                                    {
                                        CManager.BindProcessToNetwork(null);
                                    }
                                    else
                                    {
                                        ConnectivityManager.SetProcessDefaultNetwork(null);
                                    }
                                }
                            }

                            break;
                        }
                    case WifiManager.WifiStateChangedAction:
                        {
                            //WiFi Off/On
                            CheckState(context, Wifi.IsWifiEnabled ? State.Enabled : State.Disabled);

                            break;
                        }
                    case WifiManager.ScanResultsAvailableAction:
                        {
                            if (ConnectedAlwaysOnGateway)
                            {
                                CheckState(context, State.ConnectedToSSID);
                            }
                            else
                            {
                                DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                                AuthenticatedOnFirstWiFiOn = false;

                                CheckState(context, Wifi.IsWifiEnabled ? State.Enabled : State.Disabled);
                            }

                            break;
                        }
                    case "com.is.alwayson.hotspotservice.StartScan":
                        {
                            Wifi = (WifiManager)context.GetSystemService(Context.WifiService);
                            Wifi.StartScan();
                            break;
                        }
                }
            }
            else
            {
                //Not logged in

                if (HotspotHelperConsts.IsMarshmallow)
                {
                    CManager.BindProcessToNetwork(null);
                }
                else
                {
                    ConnectivityManager.SetProcessDefaultNetwork(null);
                }
            }
        }

        private static void CheckState(Context context, State state)
        {
            var userSettings = BackendProvider_Droid.GetUserSettings;
            try
            {
                switch (state)
                {
                    case State.None:
                        {
                            DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                            AuthenticatedOnFirstWiFiOn = false;

                            break;
                        }
                    case State.Disabled:
                        {
                            if (WiFiState != state)
                            {
                                WiFiState = state;

                                var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
                                hotspothelperdb.SessionID = "";
                                hotspothelperdb.AuthenticatedUsername = "";
                                hotspothelperdb.IsAccuris = false;

                                DBHelper.Database.HotspotHelperSet(hotspothelperdb);

                                AuthenticatedOnFirstWiFiOn = false;
                                IsConnected = false;

                                GetConnectivityStatePackage((sp) => WiFiChanged?.Invoke(sp));
                            }
                            break;
                        }
                    case State.Enabled:
                        {
                            if (WiFiState != state)
                            {
                                WiFiState = state;

                                GetConnectivityStatePackage((sp) => WiFiChanged?.Invoke(sp));
                            }
                            break;
                        }
                    case State.SSIDAvailable:
                        {
                            GetConnectivityStatePackage((sp) => WiFiChanged?.Invoke(sp));
                            break;
                        }
                    case State.ConnectedToSSID:
                        {
                            if (WiFiState != state)
                            {
                                WiFiState = state;

                                IsConnected = true;

                                if (!userSettings.DisableConnectionManager)
                                {
                                    if (DBHelper.Database.HotspotHelperGet().AuthenticatedUsername.Length == 0)
                                    {
                                        AlwaysOnNotifications.ShowNotification(context, new AlwaysOnNotificationParameters()
                                        {
                                            title = "AlwaysOn Not Connected",
                                            subtitle = "You are not connected to the internet",
                                            body = "But you are connected to an AlwaysOn hotspot",
                                            badge = 1,
                                            status = AlwaysOnNotificationStatus.NotConnected
                                        });
                                    }
                                }

                                GetConnectivityStatePackage((sp) => WiFiChanged?.Invoke(sp));
                            }
                            break;
                        }
                    case State.Authenticated:
                        {

                            WiFiState = state;
                            AuthenticatedOnFirstWiFiOn = false;
                            IsConnected = true;

                            if (!userSettings.DisableConnectionManager)
                            {
                                var connectedPackage = BackendProvider_Droid.GetStoredPackages().Where(n => n.LoginUserName == DBHelper.Database.HotspotHelperGet().AuthenticatedUsername).FirstOrDefault();

                                AlwaysOnNotifications.ShowNotification(context, new AlwaysOnNotificationParameters()
                                {
                                    title = "AlwaysOn Connected",
                                    subtitle = "You are connected to the internet",
                                    body = connectedPackage != null ? "Package " + connectedPackage.GroupName + " (" + connectedPackage.LoginUserName + ") is now active." : "",
                                    badge = 1,
                                    status = AlwaysOnNotificationStatus.Connected
                                });
                            }

                            GetConnectivityStatePackage((sp) => WiFiChanged?.Invoke(sp));
                            break;
                        }
                }
            }
            catch //(Exception ex)
            {
            }
        }

        public static void EnableWiFi()
        {
            Task.Factory.StartNew(() => Wifi.SetWifiEnabled(true), TaskCreationOptions.LongRunning);
        }

        public static void ConnectToSSID(Context context, string SSID)
        {
            if (!string.IsNullOrEmpty(SSID))
            {
                SSID = "\"" + SSID + "\"";

                if (HotspotHelperConsts.IsLollipop)
                {
                    RegisteredSSID = SSID.Replace("\"", "").Trim();

                    ResetVars();

                    var networkRequest = new NetworkRequest.Builder().AddTransportType(Android.Net.TransportType.Wifi).Build();
                    CManager.RegisterNetworkCallback(networkRequest, new NetworkCallback(context, SSID));
                }

                var NetworkConfig = Wifi.ConfiguredNetworks.Where(n => n.Ssid != null && n.Ssid == SSID).FirstOrDefault();
                if (NetworkConfig == null)
                {
                    NetworkConfig = new WifiConfiguration()
                    {
                        Ssid = SSID,
                        StatusField = WifiStatus.Enabled,
                        Priority = Wifi.ConfiguredNetworks.Max(n => n.Priority) + 1
                    };
                    NetworkConfig.AllowedKeyManagement.Set((int)KeyManagementType.None);
                    NetworkConfig.NetworkId = Wifi.AddNetwork(NetworkConfig);

                    Wifi.SaveConfiguration();
                }
                if (Wifi.EnableNetwork(NetworkConfig.NetworkId, true))
                {
                    //CheckState(context, State.ConnectedToSSID);
                }
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

                        if (Utils.IsOnline(_context))
                        {
                            using (var client = new TimeoutWebClient())
                            {
                                try
                                {
                                    gwURL = "http://" + Android.Text.Format.Formatter.FormatIpAddress(Wifi.DhcpInfo.Gateway);
                                    var urlList = new List<string> { gwURL };
                                    var gotWispr = false;

                                    try
                                    {
                                        text = client.DownloadString(gwURL);
                                        gotWispr = text.Contains(HotspotHelperConsts.WisprTAG);
                                    }
                                    catch (WebException ex)
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
                                                try
                                                {
                                                    text = client.DownloadString(gwURL);
                                                    gotWispr = text.Contains(HotspotHelperConsts.WisprTAG);
                                                }
                                                catch (Exception ex)
                                                {
                                                    try
                                                    {
                                                        gwURL = new UriBuilder(gwURL) { Scheme = System.Uri.UriSchemeHttp, Port = -1 }.ToString();
                                                        text = client.DownloadString(gwURL);
                                                        gotWispr = text.Contains(HotspotHelperConsts.WisprTAG);
                                                    }
                                                    catch (Exception exs)
                                                    {
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
                                                gwURL = "http://" + Android.Text.Format.Formatter.FormatIpAddress(Wifi.DhcpInfo.Gateway) + "/status";
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
                                        text = text.Substring(text.IndexOf("<" + HotspotHelperConsts.WisprTAG));
                                        text = text.Substring(0, text.IndexOf(HotspotHelperConsts.WisprTAG + ">") + HotspotHelperConsts.WisprTAG.Length + 1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Try above again
                                }
                            }
                        }
                        else
                        {
                            //Offline
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
                            var text = "";
                            try
                            {
                                text = client.DownloadString(GatewayStatusURL);
                            }
                            catch (Exception ex)
                            {
                                GatewayStatusURL = new UriBuilder(GatewayStatusURL) { Scheme = System.Uri.UriSchemeHttp, Port = -1 }.ToString();
                                text = client.DownloadString(GatewayStatusURL);
                            }

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
            AuthenticatedOnFirstWiFiOn = true;

            var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
            if (hotspothelperdb.IsAccuris)
            {
                var logoutresponse = BackendProvider.AccurisLogout(MainApplication.ApiKey, hotspothelperdb.SessionID);
                if (logoutresponse.Success)
                {
                    WiFiState = State.ConnectedToSSID;
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
                            WiFiState = State.ConnectedToSSID;
                            DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");

                            return true;
                        }
                    }
                    catch (Exception ex) { var exception = ex; }
                }
            }

            return false;
        }

        public static WisprAuthenticationResponse ConnectPackage(Context context, string Username, string Password, bool WithCallback)
        {
            var waresponse = new WisprAuthenticationResponse(false, "Timed out");

            var hotspothelperdb = DBHelper.Database.HotspotHelperGet();
            if (hotspothelperdb.IsAccuris)
            {
                var loginresponse = BackendProvider.AccurisLogin(MainApplication.ApiKey, Username, Password, hotspothelperdb.SessionID);
                if (loginresponse.Success)
                {
                    waresponse = new WisprAuthenticationResponse(true, "");

                    WiFiState = State.Authenticated;
                    DBHelper.Database.HotspotHelperSetAuthenticatedUsername(Username.Replace("IPASS/", "").Replace("@alwayson.co.za", ""));
                }
                else
                {
                    waresponse = new WisprAuthenticationResponse(false, loginresponse.Message);

                    WiFiState = State.ConnectedToSSID;
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
                        WiFiState = State.Authenticated;
                        DBHelper.Database.HotspotHelperSetAuthenticatedUsername(Username.Replace("IPASS/", "").Replace("@alwayson.co.za", ""));
                    }
                    else
                    {
                        if (waresponse.ResultMessage.Contains("You are already logged in"))
                        {
                            DisconnectPackage();
                        }

                        WiFiState = State.ConnectedToSSID;
                        DBHelper.Database.HotspotHelperSetAuthenticatedUsername("");
                    }
                }
                catch (Exception ex)
                {
                }

            }

            if (waresponse != null && waresponse.Success && WithCallback)
            {
                CheckState(context, DBHelper.Database.HotspotHelperGet().AuthenticatedUsername == "" ? State.ConnectedToSSID : State.Authenticated);
            }

            return waresponse;
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

        public class StatePackage
        {
            public State State { get; set; }
            public List<string> AvailableSSIDs { get; set; }
            public string ConnectedSSID { get; set; }
            public bool ConnectedToAlwaysOn { get; set; }
            public string AuthenticatedUsername { get; set; }
            public bool Authenticated { get; set; }
            //public bool IsConnectedToWiFi { get; set; }
            public bool Disabled { get; set; }
        }

        static object stateLocker = new object();
        public static void GetConnectivityStatePackage(Action<StatePackage> Callback)
        {
            Task.Factory.StartNew(() =>
            {
                lock (stateLocker)
                {
                    var sp = new StatePackage()
                    {
                        State = HotspotHelper.WiFiState,
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
                    //sp.IsConnectedToWiFi = sp.ConnectedToAlwaysOn || sp.Authenticated || HotspotHelper.IsConnected;
                    sp.Disabled = !Wifi.IsWifiEnabled;

                    Callback?.Invoke(sp);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private class NetworkCallback : ConnectivityManager.NetworkCallback
        {
            private string NetworkSSID;
            private Context context;

            private static bool OnAvailableLocked = false;

            public NetworkCallback(Context context, string networkSSID)
            {
                this.context = context;
                NetworkSSID = networkSSID ?? "";
            }

            public override void OnAvailable(Network network)
            {
                try
                {
                    NetworkInfo networkInfo = CManager.GetNetworkInfo(network);

                    if (networkInfo != null
                    && (networkInfo.ExtraInfo == "captive_portal_detected"
                     || networkInfo.ExtraInfo.Replace("\"", "").Trim().Contains(NetworkSSID.Replace("\"", "").Trim())))
                    {
                        CManager.UnregisterNetworkCallback(this);

                        if (OnAvailableLocked) return;

                        OnAvailableLocked = true;

                        if (HotspotHelperConsts.IsMarshmallow)
                        {
                            CManager.BindProcessToNetwork(network);
                        }
                        else
                        {
                            ConnectivityManager.SetProcessDefaultNetwork(network);
                        }

                        GetConnectivityStatePackage((sp) =>
                        {
                            if (sp.ConnectedToAlwaysOn)
                            {
                                AnalyticsProvider_Droid.TrackEventGA(context, TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperHasAlwaysOn.ToString());

                                if (!sp.Authenticated)
                                {
                                    if (!AuthenticatedOnFirstWiFiOn)
                                    {
                                        //Auth
                                        var firstAvailablePackage = BackendProvider_Droid.FirstAvailablePackage;
                                        if (firstAvailablePackage != null)
                                        {
                                            AnalyticsProvider_Droid.TrackEventGA(context, TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperAuthenticate.ToString());

                                            var KeepCurrentState = WiFiState == State.Reconnecting;
                                            var TempState = WiFiState;

                                            var connectResponse = ConnectPackage(context, firstAvailablePackage.LoginUserName, firstAvailablePackage.LoginPassword, true);
                                            if (connectResponse.Success)
                                            {
                                                if (KeepCurrentState)
                                                {
                                                    WiFiState = TempState;
                                                }
                                                CheckState(context, State.Authenticated);
                                            }
                                            else
                                            {
                                                CheckState(context, State.ConnectedToSSID);
                                            }
                                        }
                                        else
                                        {
                                            CheckState(context, State.ConnectedToSSID);

                                            AnalyticsProvider_Droid.TrackEventGA(context, TrackingCategory.Flow.ToString(), TrackingAction.BackgroundProcess.ToString(), EventLabel.HotspotHelperNoPackages.ToString());

                                            AlwaysOnNotifications.ShowNotification(context, new AlwaysOnNotificationParameters()
                                            {
                                                title = "AlwaysOn No Packages",
                                                subtitle = "You don't have any available packages to use.",
                                                body = "Buy a package in the app or online to connect to AlwaysOn",
                                                badge = 1,
                                                status = AlwaysOnNotificationStatus.NotConnected
                                            });
                                        }
                                    }
                                    else
                                    {
                                        CheckState(context, State.ConnectedToSSID);
                                    }
                                }
                                else
                                {
                                    CheckState(context, State.Authenticated);
                                }
                            }
                            else
                            {
                                CheckState(context, Wifi.IsWifiEnabled ? State.Enabled : State.Disabled);
                            }

                            OnAvailableLocked = false;
                        });
                    }
                    else
                    {
                        if (!networkInfo.IsConnectedOrConnecting)
                        {
                            if (HotspotHelperConsts.IsMarshmallow)
                            {
                                CManager.BindProcessToNetwork(null);
                            }
                            else
                            {
                                ConnectivityManager.SetProcessDefaultNetwork(null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}