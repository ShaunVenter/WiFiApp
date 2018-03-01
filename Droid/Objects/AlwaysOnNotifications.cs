using Android.App;
using Android.Content;
using Android.Media;
using Android.Text;
using Android.Text.Style;
using Android.Widget;
using System;

namespace AlwaysOn_Droid
{
    public enum AlwaysOnNotificationStatus { Connected = 0, NotConnected = 1, Available = 2, Received = 98, Other = 99 };

    public class AlwaysOnNotifications
    {
        const int color = 0x632e8e;
        const int connected = 10000;
        const int notconnected = 20000;
        const int available = 30000;

        private static bool IsNougat { get; set; } = Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N;

        static AlwaysOnNotificationStatus Status = AlwaysOnNotificationStatus.NotConnected;

        public static void ShowNotification(Context context, AlwaysOnNotificationParameters parameters)
        {
            if (Status != parameters.status || parameters.status == AlwaysOnNotificationStatus.Available)
            {
                Status = parameters.status;

                var settings = BackendProvider_Droid.GetUserSettings;
                var showNotification = false;

                switch (parameters.status)
                {
                    case AlwaysOnNotificationStatus.Connected:
                    case AlwaysOnNotificationStatus.NotConnected:
                        {
                            if (settings.Notifications)
                            {
                                showNotification = true;
                            }
                            break;
                        }
                    case AlwaysOnNotificationStatus.Available:
                        {
                            if (settings.NotificationAvailable)// && settings.LastAvailableNotification.AddMinutes(30) <= DateTime.Now)
                            {
                                showNotification = true;
                            }
                            break;
                        }
                }
                
                if (showNotification)
                {
                    Intent notificationIntent = new Intent(context, typeof(Dashboard));
                    notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    PendingIntent contentIntent = PendingIntent.GetActivity(context, 99, notificationIntent, PendingIntentFlags.UpdateCurrent);
                    
                    var builder = new Android.Support.V4.App.NotificationCompat.Builder(context);
                    builder.SetContentTitle(parameters.title);
                    builder.SetStyle(new Android.Support.V4.App.NotificationCompat.BigTextStyle().SetBigContentTitle(parameters.title)
                                                                                                 .SetSummaryText(parameters.subtitle)
                                                                                                 .BigText(parameters.subtitle + "\n" + parameters.body));
                    
                    builder.SetSmallIcon(Resource.Drawable.ic_notification_icon);
                    
                    builder.SetContentIntent(contentIntent);

                    builder.SetSound(Android.Net.Uri.Parse("android.resource://" + context.PackageName + "/" + (settings.NotificationSound ? Resource.Raw.wifi_avail : Resource.Raw.silent)));
                    builder.SetColor(color);
                    builder.SetDefaults((int)NotificationDefaults.Vibrate);
                    builder.SetLights(Android.Graphics.Color.Purple, 1500, 1000);

                    //builder.SetAutoCancel(false);
                    //builder.SetTicker("AlwaysOn Notification");
                    //builder.SetLargeIcon(Android.Graphics.BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.full_logo));
                    //builder.SetOngoing(false);
                    //builder.SetNumber(parameters.badge);

                    var notifyManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                    //int id = (int)parameters.status;
                    //switch (parameters.status)
                    //{
                    //    case AlwaysOnNotificationStatus.Connected:
                    //    case AlwaysOnNotificationStatus.NotConnected: { id += connected; break; }
                    //    case AlwaysOnNotificationStatus.Available: { id += available; break; }
                    //}

                    notifyManager.Notify(0, builder.Build());
                }
            }
        }
    }

    public class AlwaysOnNotificationParameters
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string body { get; set; }
        public int badge { get; set; }
        public AlwaysOnNotificationStatus status { get; set; }
    }
}