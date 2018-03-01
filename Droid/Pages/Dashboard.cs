using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Dashboard")]
    public class Dashboard : Activity
    {
        RelativeLayout lytActionBar;
        SvgImageView img;
        SvgImageView imgMenu;

        LinearLayout lytContainer;
        LinearLayout lytPackages;
        Android.Support.V4.Widget.SwipeRefreshLayout packageSwipeRefreshLayout;
        CustomScrollView packageScrollView;

        LinearLayout lytEmptyDash;
        List<UserPackage> _packages;
        UserSettings userSettings;
        CustomTextView txtTitle;
        CustomTextView txtSubTitle;
        CustomButton btnLinkPackages;
        LinearLayout lytButtons;
        LinearLayout lytNotification;
        CustomTextView txtNotification;
        LinearLayout lytNotificationSuccess;
        CustomTextView txtNotificationSuccess;
        SvgImageView imgInstructionIcon;
        Android.Support.V7.Widget.SwitchCompat instructionSwitch;
        RelativeLayout lytInstruction;
        ProgressBar connectivityProgressBar;
        CustomTextView txtInstruction;
        Animation fadeOutAnimation;
        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;
        CustomButton fabPay;
        FrameLayout frameLayout;
        AlwaysOnAlertDialog enableWiFiAlert,
                            connectAlwaysOnAlert,
                            packageAlert,
                            packageDetailsAlert,
                            unlinkPackageAlert,
                            unlinkPackagePasswordAlert,
                            unlinkedPackageAlert,
                            closeApplicationAlert;

        UIObjectMove floatButton = new UIObjectMove();
        bool allowAction = true;

        bool isNewAndroid = Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
        List<RelativeLayoutPosition> DragList = new List<RelativeLayoutPosition>();
        float DragSnapPosition = -1;
        List<float> TempTops = new List<float>();

        static State WiFiState = State.None;
        static HotspotHelper.StatePackage hotspotHelperState = new HotspotHelper.StatePackage();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.Dashboard.ToString());

            SetContentView(Resource.Layout.Dashboard);

            OnResume();
        }

        protected override void OnPause()
        {
            HotspotHelper.WiFiChanged = null;
            base.OnPause();
        }

        protected override void OnResume()
        {
            HotspotHelper.WiFiChanged += WiFiChanged_Received;
            base.OnResume();

            try
            {
                if (BackendProvider_Droid.GetUser == null)
                {
                    var login = new Intent(this, typeof(Login));
                    login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    Finish();
                    StartActivity(login);
                    return;
                }

                userSettings = BackendProvider_Droid.GetUserSettings;

                IntializeLayout();
                InitializePackages(true);

                HideLoader();
            }
            catch (Exception ex)
            {
                RefreshDash();
            }

            GetConnectivityStatus();
        }

        protected override void OnDestroy()
        {
            HotspotHelper.WiFiChanged = null;
            base.OnDestroy();
        }

        private void GetConnectivityStatus(object sender = null, EventArgs e = null)
        {
            if (sender != null || e != null)
            {
                lytInstruction.Clickable = false;
                lytInstruction.Click -= GetConnectivityStatus;
            }

            txtInstruction.Text = "      Getting connectivity status";
            connectivityProgressBar.Visibility = ViewStates.Visible;
            imgInstructionIcon.SetSvg(this, null);

            HotspotHelper.GetConnectivityStatePackage((sp) =>
            {
                hotspotHelperState = sp;

                connectAlwaysOnAlert = null;

                RunOnUiThread(() => InitializePackages(false));
            });
        }

        private void IntializeLayout()
        {
            lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);
            imgMenu.Click += imgMenu_Click;

            lytContainer = FindViewById<LinearLayout>(Resource.Id.lytContainer);
            lytPackages = FindViewById<LinearLayout>(Resource.Id.lytPackages);
            packageSwipeRefreshLayout = FindViewById<Android.Support.V4.Widget.SwipeRefreshLayout>(Resource.Id.packageSwipeRefreshLayout);
            packageSwipeRefreshLayout.SetColorSchemeColors(Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.my_purple),
                                                           Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.my_yellow),
                                                           Android.Support.V4.Content.ContextCompat.GetColor(this, Resource.Color.my_red));
            packageSwipeRefreshLayout.Refresh += (object sender, EventArgs e) =>
            {
                packageSwipeRefreshLayout.Refreshing = true;

                Task.Run(() =>
                {
                    var sp = BackendProvider_Droid.GetStoredServiceProviders();
                    if (sp.Result == OperationResult.Failure || (sp.Result == OperationResult.Success && ((ServiceProviders)sp.Response).Items?.Count == 0))
                    {
                        BackendProvider_Droid.GetServiceProviders();
                    }

                    BackendProvider_Droid.UpdatePackagesFromServer();
                    //Make Callback -> then update packages on dashboard with pull down refresh
                    var packages = BackendProvider_Droid.GetStoredPackages();

                    RunOnUiThread(() =>
                    {
                        _packages = packages;

                        packageSwipeRefreshLayout.Refreshing = false;

                        InitializePackages(true);
                    });
                });
            };
            packageScrollView = FindViewById<CustomScrollView>(Resource.Id.packageScrollView);

            lytEmptyDash = FindViewById<LinearLayout>(Resource.Id.lytEmptyDash);
            lytEmptyDash.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(50), 0);
            lytEmptyDash.TextAlignment = TextAlignment.Center;
            lytEmptyDash.Visibility = ViewStates.Gone;

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.TranslationY = Utils.CalcDimension(100);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), 0);
            lytNotification.Visibility = ViewStates.Gone;
            txtNotification = FindViewById<CustomTextView>(Resource.Id.txtNotification);

            lytNotificationSuccess = FindViewById<LinearLayout>(Resource.Id.lytNotificationSuccess);
            lytNotificationSuccess.TranslationY = Utils.CalcDimension(100);
            lytNotificationSuccess.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), 0);
            lytNotificationSuccess.Visibility = ViewStates.Gone;
            txtNotificationSuccess = FindViewById<CustomTextView>(Resource.Id.txtNotificationSuccess);

            lytInstruction = FindViewById<RelativeLayout>(Resource.Id.lytInstruction);
            lytInstruction.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(30), 0);
            lytInstruction.Visibility = ViewStates.Gone;
            connectivityProgressBar = FindViewById<ProgressBar>(Resource.Id.connectivityProgressBar);
            txtInstruction = FindViewById<CustomTextView>(Resource.Id.txtInstruction);
            imgInstructionIcon = FindViewById<SvgImageView>(Resource.Id.imgInstructionIcon);
            imgInstructionIcon.SetSvg(this, Resource.Raw.no_sign);
            imgInstructionIcon.LayoutParameters.Width = Utils.CalcDimension(60);
            imgInstructionIcon.SetPadding(0, 0, Utils.CalcDimension(20), 0);

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            fadeOutAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeOutAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                txtNotification.Text = "";
                lytNotification.Visibility = ViewStates.Gone;

                txtNotificationSuccess.Text = "";
                lytNotificationSuccess.Visibility = ViewStates.Gone;

                HideLoader();
            };

            frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout);
            frameLayout.SetPadding((Resources.DisplayMetrics.WidthPixels / 2) - Utils.CalcDimension(70), 0, 0, Utils.CalcDimension(10));

            fabPay = FindViewById<CustomButton>(Resource.Id.btnRoundBuy);
            fabPay.SetBackgroundResource(Resource.Drawable.Buy);

            fabPay.Touch += (v, me) =>
            {
                int X = (int)me.Event.RawX;
                int Y = (int)me.Event.RawY;
                switch (me.Event.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        floatButton.xPad = frameLayout.PaddingLeft;
                        floatButton.yPad = frameLayout.PaddingBottom;
                        floatButton.xDelta = X;
                        floatButton.yDelta = Y;
                        floatButton.Moved = false;
                        break;
                    case MotionEventActions.Up:
                        if (!floatButton.Moved)
                        {
                            btnBuyPackage_Click(null, null);
                        }
                        break;
                    case MotionEventActions.PointerDown:
                        break;
                    case MotionEventActions.PointerUp:
                        break;
                    case MotionEventActions.Move:
                        var _x = X - floatButton.xDelta;
                        var _y = Y - floatButton.yDelta;
                        floatButton.Moved = floatButton.Moved || Math.Abs(_x) > Utils.CalcDimension(100) || Math.Abs(_y) > Utils.CalcDimension(100);
                        if (floatButton.Moved)
                        {
                            var padleft = _x + floatButton.xPad;
                            padleft = padleft + fabPay.Width > Resources.DisplayMetrics.WidthPixels ? Resources.DisplayMetrics.WidthPixels - fabPay.Width : padleft;
                            var padbottom = (_y * -1) + floatButton.yPad;
                            padbottom = padbottom + fabPay.Height > Resources.DisplayMetrics.HeightPixels - lytActionBar.LayoutParameters.Height - Utils.CalcDimension(45) ? Resources.DisplayMetrics.HeightPixels - lytActionBar.LayoutParameters.Height - Utils.CalcDimension(45) - fabPay.Height : padbottom;
                            frameLayout.SetPadding(padleft, 0, 0, padbottom);
                        }
                        break;
                }
                frameLayout.Invalidate();
            };

            BackendProvider_Droid.GetServerSSIDs();
        }

        private void InitializePackages(bool PulledRefresh)
        {
            connectivityProgressBar.Visibility = ViewStates.Invisible;

            if (hotspotHelperState.ConnectedToAlwaysOn)
            {
                lytInstruction.Visibility = ViewStates.Gone;

                if (hotspotHelperState.AuthenticatedUsername.Length == 0)
                {
                    txtInstruction.Text = "Tap the switch to enable a package.";
                    txtInstruction.SetPadding(0, Utils.CalcDimension(15), Utils.CalcDimension(20), 0);

                    if (instructionSwitch == null)
                    {
                        var instructionSwitchParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
                        instructionSwitchParams.AddRule(LayoutRules.AlignParentRight);

                        instructionSwitch = new Android.Support.V7.Widget.SwitchCompat(this)
                        {
                            LayoutParameters = instructionSwitchParams,
                            SwitchMinWidth = Utils.CalcDimension(50),
                            TextOff = "",
                            TextOn = ""
                        };
                        instructionSwitch.SetMinimumWidth(Utils.CalcDimension(20));

                        lytInstruction.AddView(instructionSwitch);

                        Task.Run(() =>
                        {
                            while (true)
                            {
                                Thread.Sleep(2000);
                                RunOnUiThread(() => instructionSwitch.Checked = true);
                                Thread.Sleep(500);
                                RunOnUiThread(() => instructionSwitch.Checked = false);
                                Thread.Sleep(500);
                                RunOnUiThread(() => instructionSwitch.Checked = true);
                                Thread.Sleep(500);
                                RunOnUiThread(() => instructionSwitch.Checked = false);
                            }
                        });
                    }

                    lytInstruction.Visibility = ViewStates.Visible;
                    lytInstruction.LayoutParameters.Height = Utils.CalcDimension(100);
                    lytInstruction.SetBackgroundColor(Color.ParseColor(GetString(Resource.Color.notifiaction_success)));

                    imgInstructionIcon.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                txtInstruction.Text = "You are not connected to an AlwaysOn Hotspot";
                txtInstruction.SetPadding(0, Utils.CalcDimension(15), 0, 0);
                imgInstructionIcon.SetSvg(this, Resource.Raw.no_sign);

                lytInstruction.RemoveView(instructionSwitch);

                lytInstruction.Visibility = ViewStates.Visible;
                lytInstruction.LayoutParameters.Height = Utils.CalcDimension(100);
                lytInstruction.SetBackgroundColor(Color.ParseColor(GetString(Resource.Color.notification_fail)));

                lytInstruction.Clickable = true;
                lytInstruction.Click -= GetConnectivityStatus;
                lytInstruction.Click += GetConnectivityStatus;

                imgInstructionIcon.Visibility = ViewStates.Visible;
            }

            _packages = BackendProvider_Droid.GetStoredPackages();
            if (_packages == null || _packages.Count == 0)
            {
                _packages = BackendProvider_Droid.GetPackagesFromServerSync();
                if (_packages == null || _packages.Count == 0)
                {
                    DisplayEmptyDashboard();
                }
                else
                {
                    UsablePackages();
                }
            }
            else
            {
                UsablePackages();
            }

            if (!PulledRefresh)
                WiFiEnableAsync(); //Checks if WiFi is switched on or not
        }

        private void WiFiChanged_Received(HotspotHelper.StatePackage packageState)
        {
            //if (ActivityPaused) return;

            hotspotHelperState = packageState;

            if (WiFiState != hotspotHelperState.State)
            {
                WiFiState = hotspotHelperState.State;
                RefreshDash();
            }
        }

        private void WiFiEnableAsync()
        {
            if (hotspotHelperState.Disabled)
            {
                if (!userSettings.DisableConnectionManager)
                {
                    //Show Alert
                    if (enableWiFiAlert == null)
                    {
                        var buttons = new string[] { "Yes", "No" };
                        var title = "Your WiFi is disabled";
                        var message = "Do you want to enable your WiFi?";

                        RunOnUiThread(() =>
                        {
                            enableWiFiAlert = AndroidAlert.ShowAlert(this, title, message, buttons);
                            enableWiFiAlert.ButtonClicked += (object sender, AlwaysOnAlertDialogArgs e) =>
                            {
                                enableWiFiAlert = null;

                                if (e.which == 0)
                                {
                                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardTurnOnWifi.ToString());

                                    HotspotHelper.EnableWiFi();
                                }
                            };
                            enableWiFiAlert.Show();
                        });
                    }
                }
            }
            else
            {
                //if (hotspotHelperState.ShowSSIDAvailable)
                //{
                var SSIDList = hotspotHelperState.AvailableSSIDs;

                if (!hotspotHelperState.ConnectedToAlwaysOn && SSIDList.Count > 0) //hotspotHelperState.IsConnectedToWiFi && 
                {
                    if (!userSettings.DisableConnectionManager)
                    {
                        //Alert or AutoConnect
                        if (connectAlwaysOnAlert == null)
                        {
                            var onlyOne = false;
                            var title = "AlwaysOn WiFi Available";
                            var message = "Please select an Available AlwaysOn Hotspot to connect to";

                            if (!SSIDList.Contains("Cancel"))
                                SSIDList.Add("Cancel");

                            var buttons = SSIDList.ToArray();

                            //Show Alert
                            if (SSIDList.Count == 2)
                            {
                                onlyOne = true;
                                message = "Do you want to connect to the AlwaysOn hotspot \"" + SSIDList.FirstOrDefault() + "\"?";
                                buttons = new string[] { "Yes", "No" };
                            }

                            RunOnUiThread(() =>
                            {
                                connectAlwaysOnAlert = AndroidAlert.ShowAlert(this, title, message, buttons);
                                if (connectAlwaysOnAlert != null)
                                {
                                    connectAlwaysOnAlert.ButtonClicked += (object sender, AlwaysOnAlertDialogArgs e) =>
                                    {
                                        connectAlwaysOnAlert = null;

                                        if (buttons[e.which] != "Cancel" && buttons[e.which] != "No" && !hotspotHelperState.ConnectedToAlwaysOn) //hotspotHelperState.IsConnectedToWiFi &&
                                        {
                                            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardConnectAlwaysOn.ToString());

                                            HotspotHelper.ConnectToSSID(this, onlyOne ? SSIDList.FirstOrDefault() : buttons[e.which]);
                                        }
                                    };
                                    connectAlwaysOnAlert.Show();
                                }
                            });
                        }
                    }
                }

                if (hotspotHelperState.State == State.None && SSIDList.Count == 0)
                {
                    SendBroadcast(new Intent("com.is.alwayson.hotspotservice.StartScan"));
                }
            }
        }

        private void DisplayEmptyDashboard()
        {
            lytEmptyDash.Visibility = ViewStates.Visible;
            packageSwipeRefreshLayout.Visibility = ViewStates.Gone;

            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);
            txtTitle.SetPadding(0, Utils.CalcDimension(80), 0, 0);
            txtTitle.TextAlignment = TextAlignment.Center;

            txtSubTitle = FindViewById<CustomTextView>(Resource.Id.txtSubTitle);
            txtSubTitle.SetPadding(0, Utils.CalcDimension(100), 0, 0);
            txtSubTitle.TextAlignment = TextAlignment.Center;

            btnLinkPackages = FindViewById<CustomButton>(Resource.Id.btnLinkPackage);
            btnLinkPackages.LayoutParameters.Width = Utils.CalcDimension(245);
            btnLinkPackages.LayoutParameters.Height = Utils.CalcDimension(75);

            lytButtons = FindViewById<LinearLayout>(Resource.Id.lytButtons);
            lytButtons.LayoutParameters.Height = Utils.CalcDimension(300);

            btnLinkPackages.Click += delegate
            {
                var i = new Intent(this, typeof(LinkPackage));
                i.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                Finish();
                StartActivity(i);
            };
        }

        private void UsablePackages()
        {
            try
            {
                DragList = new List<RelativeLayoutPosition>();
                lytPackages.RemoveAllViews();

                DragList = null;
                TempTops = new List<float>();

                foreach (var package in _packages)
                {
                    package.Sanitize();

                    /* Create all Controls and Views */
                    #region Create all Controls and Views

                    var lytListItemWrapper = new RelativeLayout(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, Utils.CalcDimension(180))
                        {
                            LeftMargin = 0,
                            TopMargin = 0,
                            RightMargin = 0,
                            BottomMargin = 0
                        }
                    };
                    lytListItemWrapper.SetBackgroundColor(Color.ParseColor("#3a3a3a"));

                    RelativeLayout lytProgressBackground = new RelativeLayout(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent)
                    };
                    lytProgressBackground.SetBackgroundColor(Color.ParseColor("#3a3a3a"));

                    LinearLayout lytDataContainer = new LinearLayout(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, Utils.CalcDimension(175)),
                        Orientation = Orientation.Horizontal
                    };
                    lytDataContainer.SetPadding(Utils.CalcDimension(30), 0, 0, Utils.CalcDimension(10));

                    var txtTitle = new CustomTextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(Utils.CalcDimension(200), LinearLayout.LayoutParams.MatchParent),
                        TextSize = 20,
                        Gravity = GravityFlags.Center,
                        TextAlignment = TextAlignment.Center
                    };
                    txtTitle.SetTextColor(Color.White);
                    txtTitle.SetCustomFont(GetString(Resource.String.fontBold));
                    lytDataContainer.AddView(txtTitle);

                    LinearLayout lytTextContainer = new LinearLayout(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(Utils.CalcDimension(360), LinearLayout.LayoutParams.MatchParent),
                        Orientation = Orientation.Vertical
                    };

                    var txtLeftValue = new CustomTextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent),
                        Gravity = GravityFlags.Left,
                        TextSize = 13
                    };
                    txtLeftValue.SetPadding(0, Utils.CalcDimension(15), 0, Utils.CalcDimension(10));
                    txtLeftValue.SetTextColor(Color.White);
                    txtLeftValue.SetCustomFont(GetString(Resource.String.fontRegular));
                    lytTextContainer.AddView(txtLeftValue);

                    var txtExpiryDate = new CustomTextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent),
                        Gravity = GravityFlags.Left,
                        TextSize = 13
                    };
                    txtExpiryDate.SetPadding(0, Utils.CalcDimension(7), 0, Utils.CalcDimension(10));
                    txtExpiryDate.SetTextColor(Color.White);
                    txtExpiryDate.SetCustomFont(GetString(Resource.String.fontRegular));
                    lytTextContainer.AddView(txtExpiryDate);

                    var txtActive = new CustomTextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent),
                        Gravity = GravityFlags.Left,
                        Text = "",
                        TextSize = 13
                    };
                    txtActive.SetPadding(0, Utils.CalcDimension(7), 0, 0);
                    txtActive.SetTextColor(Color.White);
                    txtActive.SetCustomFont(GetString(Resource.String.fontBold));
                    lytTextContainer.AddView(txtActive);

                    lytDataContainer.AddView(lytTextContainer);

                    var packageSwitchParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent) { RightMargin = Utils.CalcDimension(5) };
                    packageSwitchParams.AddRule(LayoutRules.AlignParentRight | LayoutRules.AlignParentTop);
                    var packageSwitch = new Android.Support.V7.Widget.SwitchCompat(this)
                    {
                        LayoutParameters = packageSwitchParams,
                        TextOff = "0",
                        TextOn = "1",
                        SwitchMinWidth = Utils.CalcDimension(20)
                    };
                    packageSwitch.TranslationY = Utils.CalcDimension(60);
                    //if (Resources.DisplayMetrics.WidthPixels < 801)
                    //{
                    //    packageSwitch.SetPadding(Utils.CalcDimension(20), Utils.CalcDimension(40), 0, Utils.CalcDimension(10));
                    //    packageSwitch.SwitchMinWidth = Utils.CalcDimension(50);
                    //}
                    //else
                    //    packageSwitch.SetPadding(Utils.CalcDimension(20), Utils.CalcDimension(40), Utils.CalcDimension(40), Utils.CalcDimension(10));

                    //lytDataContainer.AddView(packageSwitch);

                    var lineParams = new RelativeLayout.LayoutParams(0, Utils.CalcDimension(20));
                    lineParams.AddRule(LayoutRules.AlignParentBottom);
                    LinearLayout Line = new LinearLayout(this)
                    {
                        LayoutParameters = lineParams
                    };

                    var expiresSoon = new CustomTextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(Utils.CalcDimension(170), Utils.CalcDimension(35))
                        {
                            LeftMargin = Utils.CalcDimension(10)
                        },
                        Gravity = GravityFlags.Center,
                        Text = "Expires soon!",
                        TextSize = 12,
                        TranslationY = Utils.CalcDimension(120),
                        Background = Android.Support.V4.Content.ContextCompat.GetDrawable(this, Resource.Drawable.orange_warning),//new XamSvg.SvgImageView(this, GetString(Resource.Raw.AO__Orange_warn)).ImageDrawable,
                        Visibility = ViewStates.Gone
                    };
                    expiresSoon.SetTextColor(Color.ParseColor("#ffffff"));
                    expiresSoon.SetPadding(Utils.CalcDimension(5), 0, Utils.CalcDimension(5), Utils.CalcDimension(5));
                    expiresSoon.SetCustomFont(GetString(Resource.String.fontRegular));

                    lytListItemWrapper.AddView(lytProgressBackground);
                    lytListItemWrapper.AddView(lytDataContainer);
                    lytListItemWrapper.AddView(packageSwitch);
                    lytListItemWrapper.AddView(Line);
                    lytListItemWrapper.AddView(expiresSoon);

                    //bottom pad
                    var bottomPadParams = new RelativeLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, Utils.CalcDimension(15));
                    bottomPadParams.AddRule(LayoutRules.AlignParentBottom);
                    var bottomPad = new LinearLayout(this) { LayoutParameters = bottomPadParams };
                    bottomPad.SetBackgroundColor(Color.ParseColor("#333333"));
                    lytListItemWrapper.AddView(bottomPad);

                    #endregion Create all Controls and Views

                    /* Set Values to Controls and Views */
                    #region Set Values to Controls and Views


                    var spid = Convert.ToInt32(string.IsNullOrEmpty(package.ServiceProviderId) ? "0" : package.ServiceProviderId);
                    package.GroupName = spid > 1 ? "Service Provider" : package.GroupName;
                    txtTitle.Text = string.IsNullOrWhiteSpace(package.GroupNumber) ? package.GroupName : package.GroupNumber + "\n" + package.GroupUnit;

                    var percentage = Convert.ToDouble(package.UsageLeftPercentage.Split(',')[0]);
                    txtLeftValue.Text = package.UsageValue + " " + package.Unit + "  (" + percentage + "%) Left";

                    var newWidth = (percentage / 100.0) * Resources.DisplayMetrics.WidthPixels;

                    var lineColor = Color.ParseColor("#f0664d");
                    lineColor = percentage >= 25 ? Color.ParseColor("#f4cc5d") : lineColor;
                    lineColor = percentage >= 50 ? Color.ParseColor("#33babc") : lineColor;
                    lineColor = percentage >= 75 ? Color.ParseColor("#905fb8") : lineColor;

                    Line.LayoutParameters.Width = Convert.ToInt32(newWidth);
                    Line.SetBackgroundColor(lineColor);
                    //Line.StartAnimation(new WidthAnimation(Line, "Width", Convert.ToInt32(newWidth)) { Duration = 1000 });
                    Line.StartAnimation(new NewWidthAnimation(Line, Line.Width, (int)newWidth) { Duration = 1000 });

                    txtExpiryDate.Text = !string.IsNullOrEmpty(package.ExpiryDate) ? "Expires " + Convert.ToDateTime(package.ExpiryDate).Date.ToString("dd MMM yyyy") : "";

                    if (package.LoginUserName == hotspotHelperState.AuthenticatedUsername)
                    {
                        txtActive.Text = "Online with this Package";
                        lytProgressBackground.SetBackgroundColor(Color.ParseColor("#632e8e"));
                        packageSwitch.Checked = true;
                    }

                    if (!string.IsNullOrEmpty(package.ExpiryDate) && (Convert.ToDateTime(package.ExpiryDate) - DateTime.Today).TotalDays < 4)
                    {
                        txtTitle.SetPadding(0, 0, 0, Utils.CalcDimension(25));
                        expiresSoon.Visibility = ViewStates.Visible;
                    }

                    if (package.UsageLeftvalue == "Uncapped")
                    {
                        txtLeftValue.Text = "";
                        txtExpiryDate.Text = "";
                        package.GroupName = "Uncapped";
                        txtTitle.Text = package.GroupName;
                    }
                    else if (package.GroupName == "Service Provider")
                    {
                        txtLeftValue.Text = BackendProvider_Droid.GetServiceProviderName(spid.ToString());
                        txtExpiryDate.Text = "Rate Limited";
                    }

                    package.toggle = packageSwitch;
                    (package.toggle as Android.Support.V7.Widget.SwitchCompat).CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
                    {
                        ShowLoader();

                        new Handler().PostDelayed(() =>
                        {
                            ConnectDisconnectPackage(package, false);
                        }, 50);
                    };

                    var ddtl = new DragDropTouchListener();
                    ddtl.LongClicked += (object sender, View view) =>
                    {
                        try
                        {
                            if (isNewAndroid)
                                view.SetZ(2);
                            else
                                view.BringToFront();

                            DragList.ForEach(v =>
                            {
                                if (isNewAndroid)
                                {
                                    if (v.ItemWrapper != view) v.ItemWrapper.SetZ(1);
                                }
                                else
                                    v.ItemWrapper.Invalidate();
                            });

                            if (isNewAndroid)
                                lytPackages.SetZ(1);
                            else
                                lytPackages.Invalidate();

                            if (TempTops.Count == 0)
                            {
                                DragList.ForEach(n =>
                                {
                                    var y = n.ItemWrapper.GetY();
                                    TempTops.Add(y);
                                    n.originalY = y;
                                    n.newY = y;
                                });
                            }

                            packageScrollView.ScrollEnabled = false;
                            packageSwipeRefreshLayout.Enabled = false;
                            ddtl.ClickedLongDone = true;
                        }
                        catch { }
                    };
                    ddtl.Dragging += (object sender, DragDropTouchEventArgs e) =>
                    {
                        try
                        {
                            if (ddtl.ClickedLongDone)
                            {
                                var CenterX = e.ViewWasAt.X;
                                var OldY = e.ViewWasAt.Y;
                                var NewY = OldY + e.Delta.Y;

                                //Move Current Dragging Item
                                e.View.SetY(NewY);

                                var currentIndex = TempTops.IndexOf(DragSnapPosition > -1 ? DragSnapPosition : OldY);

                                var currentTop = TempTops[currentIndex];
                                var beforeTop = currentIndex == 0 ? -1 : TempTops[currentIndex - 1];
                                var afterTop = currentIndex + 1 > TempTops.Count - 1 ? -1 : TempTops[currentIndex + 1];

                                var current = DragList.Where(n => n.newY == currentTop).FirstOrDefault();

                                //Swop with item below
                                if (afterTop != -1 && NewY > afterTop)
                                {
                                    var after = DragList.Where(n => n.newY == afterTop).FirstOrDefault();

                                    DragSnapPosition = after.newY;
                                    after.newY = current.newY;
                                    current.newY = DragSnapPosition;

                                    after.ItemWrapper.SetY(after.newY);
                                }

                                //Swop with item above
                                if (beforeTop != -1 && NewY < beforeTop)
                                {
                                    var before = DragList.Where(n => n.newY == beforeTop).FirstOrDefault();

                                    DragSnapPosition = before.newY;
                                    before.newY = current.newY;
                                    current.newY = DragSnapPosition;

                                    before.ItemWrapper.SetY(before.newY);
                                }
                            }
                        }
                        catch { }
                    };
                    ddtl.Dropped += (object sender, DragDropTouchEventArgs e) =>
                    {
                        try
                        {
                            DragSnapPosition = -1;

                            DragList = DragList.OrderBy(v => v.newY).ToList();
                            for (int i = 0; i < DragList.Count; i++)
                            {
                                DragList[i].newY = TempTops[i];
                                DragList[i].originalY = TempTops[i];
                                DragList[i].ItemWrapper.SetY(TempTops[i]);
                            }

                            DragList = DragList.OrderBy(n => n.originalY).ToList();

                            Task.Factory.StartNew(() =>
                            {
                                var orderedPackages = (from dl in DragList.Select(n => (int)n.ItemWrapper.Tag).ToList()
                                                       join p in _packages on dl equals p.Id
                                                       select p).ToList();

                                BackendProvider_Droid.SetPackages(orderedPackages);
                            }, TaskCreationOptions.LongRunning);

                            packageScrollView.ScrollEnabled = true;
                            packageSwipeRefreshLayout.Enabled = true;
                            ddtl.ClickedLongDone = false;
                        }
                        catch { }
                    };
                    ddtl.Click += delegate
                    {
                        ShowLoader();

                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageSelect.ToString());

                        var title = package.GroupName + " (" + package.LoginUserName + ")";
                        var message = "Select an action below";
                        var buttons = new string[0];

                        HotspotHelper.WisprGatewayStatus((SessionInfo) =>
                        {
                            if (hotspotHelperState.AuthenticatedUsername == package.LoginUserName)
                            {
                                buttons = new string[] { "Disconnect", "Cancel" };
                                title = "You are connected to the internet";

                                var fields = new List<string>();
                                if (!string.IsNullOrEmpty(hotspotHelperState.ConnectedSSID)) fields.Add("<b>SSID: " + hotspotHelperState.ConnectedSSID + "</b>");
                                if (!string.IsNullOrEmpty(package.GroupName)) fields.Add("<b>Package:</b> " + package.GroupName);
                                if (!string.IsNullOrEmpty(hotspotHelperState.AuthenticatedUsername)) fields.Add("<b>Username:</b> " + hotspotHelperState.AuthenticatedUsername);

                                if (SessionInfo != null && SessionInfo.Username == package.LoginUserName)
                                {
                                    if (!string.IsNullOrEmpty(SessionInfo.TimeRemaining)) fields.Add("<b>Time Remaining:</b> " + SessionInfo.TimeRemaining);
                                    if (!string.IsNullOrEmpty(SessionInfo.DataRemaining)) fields.Add("<b>Data Remaining:</b> " + SessionInfo.DataRemaining ?? "");
                                    if (!string.IsNullOrEmpty(SessionInfo.ConnectedDuration)) fields.Add("<b>Duration:</b> " + SessionInfo.ConnectedDuration ?? "");
                                    if (!string.IsNullOrEmpty(SessionInfo.CurrentTotalUsed)) fields.Add("<b>Data Used:</b> " + SessionInfo.CurrentTotalUsed ?? "");
                                }

                                message = string.Join("<br/>", fields) + "<br/>";
                            }
                            else
                            {
                                buttons = new string[] { "Connect", "View Details", "Unlink from Account", "Cancel" };
                            }

                            RunOnUiThread(() =>
                            {
                                packageAlert = AndroidAlert.ShowAlert(this, title, message, buttons);
                                packageAlert.ButtonClicked += (object sender, AlwaysOnAlertDialogArgs e) =>
                                {
                                    //packageDetailsAlert.Hide();

                                    switch (buttons[e.which])
                                    {
                                        case "Disconnect":
                                            {
                                                (package.toggle as Android.Support.V7.Widget.SwitchCompat).Checked = false;
                                                break;
                                            }
                                        case "Connect":
                                            {
                                                (package.toggle as Android.Support.V7.Widget.SwitchCompat).Checked = true;
                                                break;
                                            }
                                        case "View Details":
                                            {
                                                ShowLoader();

                                                AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageViewDetails.ToString());

                                                var fields = new List<string>();
                                                fields.Add("<b>Package:</b> " + package.GroupName);
                                                fields.Add("<b>Username:</b> " + package.LoginUserName);
                                                fields.Add("<b>Expiration:</b> " + (!string.IsNullOrWhiteSpace(package.ExpiryDate) ? Convert.ToDateTime(package.ExpiryDate).ToString("dd MMM yyyy") : "No Expiry"));
                                                fields.Add("<b>Type:</b> " + package.AccountTypeDesc);
                                                var providerName = BackendProvider_Droid.GetServiceProviderName(package.ServiceProviderId);
                                                if (providerName != "Unknown")
                                                {
                                                    fields.Add("<b>Provider:</b> " + providerName);
                                                }
                                                if (package.UsageLeftvalue != "Service Provider Rate Limited")
                                                {
                                                    fields.Add("<b>Usage Left:</b> " + package.UsageLeftvalue);
                                                }

                                                var msg = string.Join("<br/>", fields) + "<br/>";

                                                packageDetailsAlert = AndroidAlert.ShowAlert(this, "Package Details", msg, "Close");
                                                HideLoader();
                                                packageDetailsAlert.Show();

                                                break;
                                            }
                                        case "Unlink from Account":
                                            {
                                                ShowLoader();

                                                var user = BackendProvider_Droid.GetUser;
                                                var msg = "Are you sure you want to unlink package \"" + package.GroupName + " (" + package.LoginUserName + ")\" from your account?";
                                                msg += "<br/><br/>You will be required to enter your AlwaysOn account password for \"" + user.LoginCredential + "\".";
                                                msg += "<br/><br/>The package username and password will be displayed onscreen for you to save.";

                                                unlinkPackageAlert = AndroidAlert.ShowAlert(this, "Unlink from Account", msg, "Yes", "No");
                                                unlinkPackageAlert.ButtonClicked += (object s2, AlwaysOnAlertDialogArgs e2) =>
                                                {
                                                    if (e2.which == 0)
                                                    {
                                                        var errorMsg = "";
                                                        var pwtitle = "Unlink Package";
                                                        var pwmessage = errorMsg + "Please enter your AlwaysOn account password for \"" + user.LoginCredential + "\" to unlink package \"" + package.GroupName + " (" + package.LoginUserName + ")\"";
                                                        var pwButtons = new string[] { "Unlink Package", "Cancel" };

                                                        unlinkPackagePasswordAlert = AndroidAlert.ShowAlertWithTextField(this, pwtitle, pwmessage, true, pwButtons);

                                                        EventHandler<AlwaysOnAlertDialogArgs> passwordClick = null;
                                                        passwordClick = (object s3, AlwaysOnAlertDialogArgs e3) =>
                                                        {
                                                            if (e3.TextFieldText.Length > 0 && e3.TextFieldText == user.Password)
                                                            {
                                                                var unlinkOperation = BackendProvider.UnlinkPackageSync(MainApplication.ApiKey, package.Id);
                                                                if (unlinkOperation.Success)
                                                                {
                                                                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageUnlink.ToString());

                                                                    var unlinked = " Package was successfully unlinked from your account.<br/><br/><b>Package:</b> " + package.GroupName + "<br/><b>Username:</b> " + package.LoginUserName + "<br/><b>Password:</b> " + package.LoginPassword + "<br/>";
                                                                    unlinkedPackageAlert = AndroidAlert.ShowAlert(this, "Success", unlinked, "Close");
                                                                    unlinkedPackageAlert.ButtonClicked += (object sender3, AlwaysOnAlertDialogArgs e4) =>
                                                                    {
                                                                        RefreshDash();
                                                                    };
                                                                    unlinkedPackageAlert.Show();
                                                                }
                                                                else
                                                                {
                                                                    txtNotification.Text = unlinkOperation.Message;
                                                                    lytNotification.Visibility = ViewStates.Visible;
                                                                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(110);
                                                                    lytNotification.StartAnimation(fadeOutAnimation);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (e3.which == 0)
                                                                {
                                                                    if (errorMsg == "")
                                                                    {
                                                                        errorMsg = "Incorrect password<br/><br/>";
                                                                        pwmessage = errorMsg + pwmessage;
                                                                    }
                                                                    unlinkPackagePasswordAlert = AndroidAlert.ShowAlertWithTextField(this, pwtitle, pwmessage, true, pwButtons);
                                                                    unlinkPackagePasswordAlert.ButtonClicked += passwordClick;
                                                                    unlinkPackagePasswordAlert.Show();
                                                                }
                                                                else
                                                                {
                                                                    //Cancel Clicked
                                                                }
                                                            }
                                                        };

                                                        unlinkPackagePasswordAlert.ButtonClicked += passwordClick;
                                                        unlinkPackagePasswordAlert.Show();
                                                    }
                                                };
                                                HideLoader();
                                                unlinkPackageAlert.Show();

                                                break;
                                            }
                                        case "Cancel": { break; }
                                    }
                                };
                                HideLoader();
                                packageAlert.Show();
                            });
                        });
                    };
                    lytListItemWrapper.SetOnTouchListener(ddtl);

                    lytListItemWrapper.Tag = package.Id;

                    lytPackages.AddView(lytListItemWrapper);

                    DragList = DragList ?? new List<RelativeLayoutPosition>();

                    if (!DragList.Where(n => n.ItemWrapper.Tag == lytListItemWrapper.Tag).Any())
                        DragList.Add(new RelativeLayoutPosition() { ItemWrapper = lytListItemWrapper, originalY = 0, newY = 0 });

                    #endregion Set Values to Controls and Views
                }

                lytPackages.SetPadding(0, 0, 0, Utils.CalcDimension(180));
            }
            catch (Exception ex)
            {

            }
        }

        private void ConnectDisconnectPackage(UserPackage package, bool FirstConnnect)
        {
            if (!allowAction) return;
            allowAction = false;

            var toggle = (Android.Support.V7.Widget.SwitchCompat)package.toggle;
            package = !toggle.Checked && !FirstConnnect ? null : package;

            var Success = false;
            var Message = "";
            WiFiState = State.None;

            //1. Disconnect any connected packages
            if (hotspotHelperState.ConnectedToAlwaysOn)
            {
                if (hotspotHelperState.Authenticated)
                {
                    var packageDesc = (_packages.Where(n => n.LoginUserName == hotspotHelperState.AuthenticatedUsername).Select(n => n.GroupName + " (" + n.LoginUserName + ")").FirstOrDefault() ?? "").ToString().Trim();

                    if (!HotspotHelper.DisconnectPackage())
                    {
                        Message = "Could not Disable Package" + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "");
                        toggle.Checked = false;
                        Success = false;
                    }
                    else
                    {
                        Message = "Package " + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "") + "was disabled successfully.";
                        Success = true;

                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageDisconnect.ToString());
                    }
                }
                else
                {
                    Success = true;
                }
            }
            else
            {
                Message = "You are not connected to an AlwaysOn Hotspot";
                toggle.Checked = false;
                Success = false;
            }

            //2. If package parameter is not null, try and connect that package.
            if (Success && package != null)
            {
                var packageDesc = package.GroupName + " (" + package.LoginUserName + ")";

                var connectPackage = HotspotHelper.ConnectPackage(this, package.LoginUserName, package.LoginPassword, false);
                if (!connectPackage.Success)
                {
                    Message = "Could not Enable Package" + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " + connectPackage.ResultMessage : "");
                    toggle.Checked = false;
                    Success = false;
                }
                else
                {
                    Message = "Package " + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "") + "was enabled successfully.";
                    toggle.Checked = true;
                    Success = true;

                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageConnect.ToString());
                }
            }

            fadeOutAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                allowAction = true;
                HideLoader();
            };

            //3. If success, reload dashboard
            if (Success)
            {
                txtNotificationSuccess.Text = Message;
                lytNotificationSuccess.Visibility = ViewStates.Visible;
                lytNotificationSuccess.LayoutParameters.Height = Utils.CalcDimension(110);
                lytNotificationSuccess.StartAnimation(fadeOutAnimation);
            }
            else
            {
                txtNotification.Text = Message;
                lytNotification.Visibility = ViewStates.Visible;
                lytNotification.LayoutParameters.Height = Utils.CalcDimension(110);
                lytNotification.StartAnimation(fadeOutAnimation);
            }

            //hotspotHelperState = new HotspotHelper.StatePackage();
            //InitializePackages(false);

            HotspotHelper.GetConnectivityStatePackage((sp) =>
            {
                hotspotHelperState = sp;

                RunOnUiThread(() => InitializePackages(false));
            });
        }

        void btnBuyPackage_Click(object sender, EventArgs e)
        {
            var buyPackage = new Intent(this, typeof(BuyPackage));
            buyPackage.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            Finish();
            StartActivity(buyPackage);
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "Dashboard");
            Finish();
            StartActivity(menu);
        }

        public override void OnBackPressed()
        {
            var buttons = new string[] { "OK", "Cancel" };
            closeApplicationAlert = AndroidAlert.ShowAlert(this, "Close application", "Are you sure you want to exit?", buttons);
            closeApplicationAlert.ButtonClicked += (object sender, AlwaysOnAlertDialogArgs e) =>
            {
                //closeApplicationAlert.Hide();

                switch (buttons[e.which])
                {
                    case "OK":
                        {
                            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.CloseApplication.ToString());

                            Finish();
                            break;
                        }
                }
            };
            closeApplicationAlert.Show();
        }

        public void RefreshDash()
        {
            BackendProvider_Droid.UpdatePackagesFromServer();
            var intent = new Intent(this, typeof(Dashboard));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            Finish();
            StartActivity(intent);
            OverridePendingTransition(0, 0);
        }

        public void ShowLoader()
        {
            myProgressBar.Visibility = ViewStates.Visible;
            myProgressBar.Clickable = true;
            lytprogressBar.Visibility = ViewStates.Visible;
            lytprogressBar.Clickable = true;
        }

        public void HideLoader()
        {
            myProgressBar.Visibility = ViewStates.Invisible;
            myProgressBar.Clickable = false;
            lytprogressBar.Visibility = ViewStates.Invisible;
            lytprogressBar.Clickable = false;
        }
    }

    public class RelativeLayoutPosition : Java.Lang.Object
    {
        public RelativeLayout ItemWrapper { get; set; }
        public float originalY { get; set; }
        public float newY { get; set; }
    }
}