using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using XamSvg;

namespace AlwaysOn_Droid
{
    public class Utils
    {
        public static int CalcDimension(float dp)
        {
            return (int)(TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.System.DisplayMetrics) / 2.1);
        }

        public static int CalcDimensionRatio(float dp)
        {
            dp = dp > 360 ? 360 : dp;

            var percentage = (dp * 100 / 360);
            
            return (int)((percentage / 100) * Resources.System.DisplayMetrics.WidthPixels);
        }

        public static float CalcFontRatio(float sp)
        {
            var percentage = sp * 100 / 480;

            return (percentage / 100) * (Resources.System.DisplayMetrics.HeightPixels / Resources.System.DisplayMetrics.Density);
        }

        public static void ShowNotification(Context context, string Title, string Subtitle, string Body, bool Vibrate, bool Sound)
        {
            Intent notificationIntent = new Intent(context, typeof(MainActivity));
            notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            PendingIntent contentIntent = PendingIntent.GetActivity(context, 99, notificationIntent, PendingIntentFlags.UpdateCurrent);

            var builder = new Android.Support.V4.App.NotificationCompat.Builder(context);
            builder.SetContentTitle(Title);
            builder.SetStyle(new Android.Support.V4.App.NotificationCompat.BigTextStyle().SetBigContentTitle(Title)
                                                                                         .SetSummaryText(Subtitle)
                                                                                         .BigText(Subtitle + "\n" + Body));

            builder.SetSmallIcon(Resource.Drawable.ic_notification_icon);

            builder.SetContentIntent(contentIntent);

            builder.SetSound(Sound ? Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification) : Android.Net.Uri.Empty);
            builder.SetColor(0x14a6ff);
            builder.SetDefaults(Vibrate ? (int)NotificationDefaults.Vibrate : (int)NotificationDefaults.Lights);
            builder.SetLights(Android.Graphics.Color.Purple, 1500, 1000);

            var notifyManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

            notifyManager.Notify(GetIntFromStringAlphas(Title + Subtitle), builder.Build());
        }

        public static int GetIntFromStringAlphas(string str) => str.Select(c => (int)c).Sum(c => c);

        public static bool IsOnline(Context context)
        {
            try
            {
                var cman = (Android.Net.ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
                //var wifiNetwork = cman.GetNetworkInfo(ConnectivityType.Wifi);
                //var mobiNetwork = cman.GetNetworkInfo(ConnectivityType.Mobile);
                var networkInfo = cman.ActiveNetworkInfo;

                //return (wifiNetwork != null && wifiNetwork.IsConnectedOrConnecting) || (mobiNetwork != null && mobiNetwork.IsConnectedOrConnecting);
                return networkInfo != null && networkInfo.IsConnectedOrConnecting;
            }
            catch
            {
                return false;
            }
        }
    }
    public class HotspotHelperWebClient : System.Net.WebClient
    {
        public int Timeout { get; set; } = 1500;
        public System.Net.HttpStatusCode? ResponseStatusCode { get; set; }

        protected override System.Net.WebRequest GetWebRequest(System.Uri uri)
        {
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = Timeout;
            return w;
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
        {
            try
            {
                System.Net.WebResponse resp = base.GetWebResponse(request);
                ResponseStatusCode = (resp is System.Net.HttpWebResponse) ? ((System.Net.HttpWebResponse)resp).StatusCode : (System.Net.HttpStatusCode?)null;
                return resp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request, IAsyncResult result)
        {
            try
            {
                System.Net.WebResponse resp = base.GetWebResponse(request, result);
                ResponseStatusCode = (resp is System.Net.HttpWebResponse) ? ((System.Net.HttpWebResponse)resp).StatusCode : (System.Net.HttpStatusCode?)null;
                return resp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class TimeoutWebClient : System.Net.WebClient
    {
        public int Timeout { get; set; } = 1000;
        public System.Net.HttpStatusCode? ResponseStatusCode { get; set; }

        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            var w = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
            w.AllowAutoRedirect = false;
            w.Timeout = Timeout;
            return w;
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
        {
            try
            {
                if(request.RequestUri.Scheme == Uri.UriSchemeHttps)
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
                }
                System.Net.WebResponse resp = base.GetWebResponse(request);
                ResponseStatusCode = (resp is System.Net.HttpWebResponse) ? ((System.Net.HttpWebResponse)resp).StatusCode : (System.Net.HttpStatusCode?)null;
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request, IAsyncResult result)
        {
            try
            {
                System.Net.WebResponse resp = base.GetWebResponse(request, result);
                ResponseStatusCode = (resp is System.Net.HttpWebResponse) ? ((System.Net.HttpWebResponse)resp).StatusCode : (System.Net.HttpStatusCode?)null;
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}