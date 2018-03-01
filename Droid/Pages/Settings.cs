using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using System;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Settings")]
    public class Settings : Activity
    {
        RelativeLayout lytActionBar;
        SvgImageView img;
        SvgImageView imgBack;
        SvgImageView imgMenu;
        LinearLayout lytContainer;
        CustomTextView txtTitle;
        AlwaysOnAlertDialog disableConnectionMgr;

        CustomCheckBox chkNotifications;
        CustomCheckBox chkNotificationSound;
        CustomCheckBox chkNotificationAvailable;
        CustomCheckBox chkDisableConnectionManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.Settings.ToString());

            XamSvg.Setup.InitSvgLib();
            SetContentView(Resource.Layout.Settings);

            var user = BackendProvider_Droid.GetUser;
            if (user == null)
            {
                var login = new Intent(this, typeof(Login));
                login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(login);
                Finish();
                return;
            }

            var userSettings = BackendProvider_Droid.GetUserSettings;

            lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            imgBack = FindViewById<SvgImageView>(Resource.Id.imgBack);
            imgBack.SetSvg(this, Resource.Raw.AO__arrow_back);
            imgBack.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(20));
            imgBack.LayoutParameters.Width = Utils.CalcDimension(130);
            imgBack.Click += btnCancel_Click;

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.Click += imgMenu_Click;
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);

            lytContainer = FindViewById<LinearLayout>(Resource.Id.lytForm);
            lytContainer.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), 0);

            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);
            txtTitle.SetPadding(Utils.CalcDimension(70), 0, 0, Utils.CalcDimension(50));

            chkNotifications = FindViewById<CustomCheckBox>(Resource.Id.chkNotifications);
            chkNotifications.Checked = userSettings.Notifications;
            chkNotifications.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                userSettings.Notifications = chkNotifications.Checked;
                BackendProvider_Droid.SetUserSettings(userSettings);

                chkNotificationSound.Visibility = userSettings.Notifications ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;

                GATick();
            };
            chkNotificationSound = FindViewById<CustomCheckBox>(Resource.Id.chkNotificationSound);
            chkNotificationSound.Visibility = userSettings.Notifications ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Gone;
            chkNotificationSound.Checked = userSettings.NotificationSound;
            chkNotificationSound.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                userSettings.NotificationSound = chkNotificationSound.Checked;
                BackendProvider_Droid.SetUserSettings(userSettings);

                GATick();
            };
            chkNotificationAvailable = FindViewById<CustomCheckBox>(Resource.Id.chkNotificationAvailable);
            chkNotificationAvailable.Checked = userSettings.NotificationAvailable;
            chkNotificationAvailable.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                userSettings.NotificationAvailable = chkNotificationAvailable.Checked;
                BackendProvider_Droid.SetUserSettings(userSettings);

                GATick();
            };
            if(Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var lblMarshmallowMessage = FindViewById<CustomTextView>(Resource.Id.lblMarshmallowMessage);
                lblMarshmallowMessage.Visibility = Android.Views.ViewStates.Visible;
            }
            chkDisableConnectionManager = FindViewById<CustomCheckBox>(Resource.Id.chkDisableConnectionManager);
            chkDisableConnectionManager.Checked = userSettings.DisableConnectionManager;
            chkDisableConnectionManager.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                if (chkDisableConnectionManager.Checked)
                {
                    var buttons = new string[] { "Disable", "Cancel" };
                    var message = "The following features will not work if you disable the Connection Manager:<br/><br/>&bull; Available Hotspot Notifications<br/>&bull; Auto Authenticate to Hotspot<br/>&bull; Authentication Notifications<br/>&bull; Pop-up to connect to AlwaysOn<br/>&bull; Pop-up to enable your WiFi<br/><br/>Are you sure you want to disable the Connection Manager?";
                    disableConnectionMgr = AndroidAlert.ShowAlert(this, "Disable Connection Manager", message, buttons);
                    disableConnectionMgr.ButtonClicked += (object sender1, AlwaysOnAlertDialogArgs e1) =>
                    {
                        if (e1.which == -1)
                        {
                            //backpressed
                            chkDisableConnectionManager.Checked = false;
                        }
                        else
                        {
                            switch (buttons[e1.which])
                            {
                                case "Disable":
                                    {
                                        userSettings.DisableConnectionManager = chkDisableConnectionManager.Checked;
                                        BackendProvider_Droid.SetUserSettings(userSettings);

                                        GATick();

                                        break;
                                    }
                                default:
                                    {
                                        chkDisableConnectionManager.Checked = false;
                                        break;
                                    }
                            }
                        }
                    };
                    disableConnectionMgr.Show();
                }
                else
                {
                    userSettings.DisableConnectionManager = chkDisableConnectionManager.Checked;
                    BackendProvider_Droid.SetUserSettings(userSettings);

                    GATick();
                }
            };
        }

        private void GATick()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.SettingsTick.ToString());
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "Settings");
            StartActivity(menu);
            Finish();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.SettingsBack.ToString());

            BackendProvider_Droid.UpdatePackagesFromServer();
            var intent = new Intent(this, typeof(Dashboard));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }
    }
}
