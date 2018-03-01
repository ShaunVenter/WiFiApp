using System;
using Alliance.Carousel;
using UIKit;
using AlwaysOn_iOS.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CoreGraphics;
using XamSvg;
using MaterialControls;
using FloatLabeledEntry;
using Foundation;
using AlwaysOn;
using AlwaysOn.Objects;
using System.Text.RegularExpressions;
using TPKeyboardAvoiding;
using Google.Analytics;
using Shimmer;
using Xamarin.Social;
using Xamarin.Social.Services;

namespace AlwaysOn_iOS
{
    public partial class SettingsPage : UIViewController
    {
        UISvgImageView chkNotificationsOff = null;
        UISvgImageView chkNotificationsOn = null;
        UILabel lblNotifications = null;

        UISvgImageView chkNotificationTextOff = null;
        UISvgImageView chkNotificationTextOn = null;
        UILabel lblNotificationText = null;

        UISvgImageView chkNotificationSoundOff = null;
        UISvgImageView chkNotificationSoundOn = null;
        UILabel lblNotificationSound = null;

        UISvgImageView chkNotificationAvailableOff = null;
        UISvgImageView chkNotificationAvailableOn = null;
        UILabel lblNotificationAvailable = null;

        public SettingsPage() : base("SettingsPage", null)
        {
            UtilProvider.MenuIsOpen = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            AnalyticsProvider_iOS.PageViewGA(PageName.Settings.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var userObject = BackendProvider_iOS.GetUser;
            var userSettings = BackendProvider_iOS.GetUserSettings;

            var Wrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight));
            var instruction = new UILabel(new CGRect(0, 10, Wrapper.Bounds.Width, 30))
            {
                Text = "Settings",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 23)
            };

            //UserSettings Notifications/AutoAuthenticate
            var NotificationsTop = UtilProvider.ItemPosition(1);
            chkNotificationsOff = new UISvgImageView("svg/Check_off.svg")
            {
                Frame = new CGRect(0, NotificationsTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true
            };
            chkNotificationsOff.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.Notifications = true;
                chkNotificationsOn.Alpha = 1;

                BackendProvider_iOS.SetUserSettings(userSettings);

                ShowHideNotificationSettings(!userSettings.Notifications);

                GATick();
            }));
            chkNotificationsOn = new UISvgImageView("svg/Check_on.svg")
            {
                Frame = new CGRect(0, NotificationsTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true,
                Alpha = userSettings.Notifications ? 1 : 0
            };
            chkNotificationsOn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.Notifications = false;
                chkNotificationsOn.Alpha = 0;

                BackendProvider_iOS.SetUserSettings(userSettings);

                ShowHideNotificationSettings(!userSettings.Notifications);

                GATick();
            }));
            lblNotifications = new UILabel(new CGRect(25, NotificationsTop, UtilProvider.ScreenWidth - 100, 30))
            {
                Text = "Notifications (Auto Authenticate)",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Left,
                UserInteractionEnabled = true
            };
            lblNotifications.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                chkNotificationsOn.Alpha = userSettings.Notifications ? 0 : 1;
                userSettings.Notifications = !userSettings.Notifications;

                BackendProvider_iOS.SetUserSettings(userSettings);
                
                ShowHideNotificationSettings(!userSettings.Notifications);

                GATick();
            }));

            //UserSettings NotificationText
            var NotificationTextTop = UtilProvider.ItemPosition(2);
            chkNotificationTextOff = new UISvgImageView("svg/Check_off.svg")
            {
                Frame = new CGRect(20, NotificationTextTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true
            };
            chkNotificationTextOff.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.NotificationText = true;
                chkNotificationTextOn.Alpha = 1;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));
            chkNotificationTextOn = new UISvgImageView("svg/Check_on.svg")
            {
                Frame = new CGRect(20, NotificationTextTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true,
                Alpha = userSettings.NotificationText ? 1 : 0
            };
            chkNotificationTextOn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.NotificationText = false;
                chkNotificationTextOn.Alpha = 0;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));
            lblNotificationText = new UILabel(new CGRect(45, NotificationTextTop, UtilProvider.ScreenWidth - 100, 30))
            {
                Text = "Notification Pop-up",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Left,
                UserInteractionEnabled = true
            };
            lblNotificationText.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                chkNotificationTextOn.Alpha = userSettings.NotificationText ? 0 : 1;
                userSettings.NotificationText = !userSettings.NotificationText;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));

            //UserSettings NotificationSound
            var NotificationSoundTop = UtilProvider.ItemPosition(3);
            chkNotificationSoundOff = new UISvgImageView("svg/Check_off.svg")
            {
                Frame = new CGRect(20, NotificationSoundTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true
            };
            chkNotificationSoundOff.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.NotificationSound = true;
                chkNotificationSoundOn.Alpha = 1;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));
            chkNotificationSoundOn = new UISvgImageView("svg/Check_on.svg")
            {
                Frame = new CGRect(20, NotificationSoundTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true,
                Alpha = userSettings.NotificationSound ? 1 : 0
            };
            chkNotificationSoundOn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.NotificationSound = false;
                chkNotificationSoundOn.Alpha = 0;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));
            lblNotificationSound = new UILabel(new CGRect(45, NotificationSoundTop, UtilProvider.ScreenWidth - 100, 30))
            {
                Text = "Notification Sound",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Left,
                UserInteractionEnabled = true
            };
            lblNotificationSound.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                chkNotificationSoundOn.Alpha = userSettings.NotificationSound ? 0 : 1;
                userSettings.NotificationSound = !userSettings.NotificationSound;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));

            //UserSettings NotificationAvailable
            var NotificationAvailableTop = UtilProvider.ItemPosition(4);
            chkNotificationAvailableOff = new UISvgImageView("svg/Check_off.svg")
            {
                Frame = new CGRect(20, NotificationAvailableTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true
            };
            chkNotificationAvailableOff.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.NotificationAvailable = true;
                chkNotificationAvailableOn.Alpha = 1;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));
            chkNotificationAvailableOn = new UISvgImageView("svg/Check_on.svg")
            {
                Frame = new CGRect(20, NotificationAvailableTop, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                UserInteractionEnabled = true,
                Alpha = userSettings.NotificationAvailable ? 1 : 0
            };
            chkNotificationAvailableOn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                userSettings.NotificationAvailable = false;
                chkNotificationAvailableOn.Alpha = 0;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));
            lblNotificationAvailable = new UILabel(new CGRect(45, NotificationAvailableTop, UtilProvider.ScreenWidth - 100, 30))
            {
                Text = "Notification On Availability",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Left,
                UserInteractionEnabled = true
            };
            lblNotificationAvailable.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                chkNotificationAvailableOn.Alpha = userSettings.NotificationAvailable ? 0 : 1;
                userSettings.NotificationAvailable = !userSettings.NotificationAvailable;

                BackendProvider_iOS.SetUserSettings(userSettings);

                GATick();
            }));

            Wrapper.Add(instruction);

            Wrapper.Add(chkNotificationsOff);
            Wrapper.Add(chkNotificationsOn);
            Wrapper.Add(lblNotifications);

            Wrapper.Add(chkNotificationTextOff);
            Wrapper.Add(chkNotificationTextOn);
            Wrapper.Add(lblNotificationText);

            Wrapper.Add(chkNotificationSoundOff);
            Wrapper.Add(chkNotificationSoundOn);
            Wrapper.Add(lblNotificationSound);

            Wrapper.Add(chkNotificationAvailableOff);
            Wrapper.Add(chkNotificationAvailableOn);
            Wrapper.Add(lblNotificationAvailable);

            ShowHideNotificationSettings(!userSettings.Notifications);

            var parent = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight - UtilProvider.SafeTop - UtilProvider.ScreenHeight12th));
            parent.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            parent.Add(Wrapper);

            View.Add(parent);
            View.Add(UtilProvider.HeaderStrip(true, true, this));
        }

        private void ShowHideNotificationSettings(bool hidden)
        {
            chkNotificationTextOff.Hidden = hidden;
            chkNotificationTextOn.Hidden = hidden;
            lblNotificationText.Hidden = hidden;
            chkNotificationSoundOff.Hidden = hidden;
            chkNotificationSoundOn.Hidden = hidden;
            lblNotificationSound.Hidden = hidden;
            chkNotificationAvailableOff.Hidden = hidden;
            chkNotificationAvailableOn.Hidden = hidden;
            lblNotificationAvailable.Hidden = hidden;
        }

        private void GATick()
        {
            AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.SettingsTick.ToString());
        }
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


