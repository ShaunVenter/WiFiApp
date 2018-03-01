using System;
using UserNotifications;

namespace AlwaysOn_iOS.Objects
{
    public enum AlwaysOnNotificationStatus { Connected = 0, NotConnected = 1, Available = 2 };

    public class AlwaysOnNotifications
    {
        private static double DELAY_NOTIFICATION_SECONDS = 0.01;
        private static string[] NOTIFICATION_STATUS = { "AlwaysOnConnected", "AlwaysOnNotConnected", "AlwaysOnAvailable" };

        public static void ShowNotification(AlwaysOnNotificationParameters parameters)
        {
            var settings = BackendProvider_iOS.GetUserSettings;
            var showNotification = false;

            switch (parameters.status)
            {
                case AlwaysOnNotificationStatus.Connected:
                case AlwaysOnNotificationStatus.NotConnected:
                    {
                        if (settings.Notifications)// && settings.LastConnectionNotification.AddMinutes(30) <= DateTime.Now)
                        {
                            showNotification = true;

                            //settings.LastConnectionNotification = DateTime.Now;
                            //BackendProvider_iOS.SetUserSettings(settings);
                        }
                        break;
                    }
                case AlwaysOnNotificationStatus.Available:
                    {
                        if (settings.Notifications && settings.NotificationAvailable && settings.LastAvailableNotification.AddMinutes(30) <= DateTime.Now)
                        {
                            showNotification = true;

                            settings.LastAvailableNotification = DateTime.Now;
                            BackendProvider_iOS.SetUserSettings(settings);
                        }
                        break;
                    }
            }

            if (showNotification)
            {
                UNUserNotificationCenter.Current.AddNotificationRequest(UNNotificationRequest.FromIdentifier(NOTIFICATION_STATUS[(int)parameters.status], new UNMutableNotificationContent()
                {
                    Title = parameters.title,
                    Subtitle = settings.NotificationText ? parameters.subtitle : "",
                    Body = settings.NotificationText ? parameters.body : "",
                    Badge = parameters.badge,
                    Sound = settings.NotificationSound ? UNNotificationSound.GetSound("wifi_avail.mp3") : UNNotificationSound.GetSound("silent.mp3")
                }, UNTimeIntervalNotificationTrigger.CreateTrigger(DELAY_NOTIFICATION_SECONDS, false)), null);
            }
        }

        public static void RemoveAll(UNUserNotificationCenter center)
        {
            if (center != null)
            {
                center.RemoveDeliveredNotifications(NOTIFICATION_STATUS);
                center.RemoveAllDeliveredNotifications();

                center.RemovePendingNotificationRequests(NOTIFICATION_STATUS);
                center.RemoveAllPendingNotificationRequests();
            }

            // reset our badge
            UIKit.UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
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

    public class AlwaysOnNotificationDelegate : UNUserNotificationCenterDelegate
    {
        public event EventHandler<bool> Update;
        protected void OnUpdate(object sender, bool e)
        {
            if (Update != null)
                Update(sender, e);
        }

        public AlwaysOnNotificationDelegate(EventHandler<bool> Update)
        {
            this.Update += Update;
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            AlwaysOnNotifications.RemoveAll(center);

            OnUpdate(this, true);

            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            AlwaysOnNotifications.RemoveAll(center);
            completionHandler();
        }
    }
}