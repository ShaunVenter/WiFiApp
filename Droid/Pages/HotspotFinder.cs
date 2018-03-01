using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.Net.Wifi;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.HotspotFinder")]
    public class HotspotFinder : FragmentActivity, Android.Locations.ILocationListener
    {
        public const int REQUEST_CHECK_SETTINGS = 0x1;

        bool ExecuteOnResume;
        SvgImageView img;
        SvgImageView imgMenu;
        SvgImageView imgBack;

        public static RadiusCoordinates RadiusCoords = new RadiusCoordinates();
        public static RadiusCoordinates ScrollCoords;
        public static float ZoomLevel = 15;
        public static List<Hotspot> Hotspots;

        protected HotspotFragmentAdapter adapter;
        protected ViewPager pager;
        protected TabLayout tabs;
        LinearLayout lytNotification;
        CustomTextView txtNotification;
        AlwaysOnAlertDialog locationAlert;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        LocationManager _locationManager;
        string _locationProvider;
        CustomButton fabMap;
        UIObjectMove floatButton = new UIObjectMove();
        FrameLayout frameLayout;

        bool hasLocations = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ExecuteOnResume = false;

            AnalyticsProvider_Droid.PageViewGA(this, PageName.HotspotFinderList.ToString());

            SetContentView(Resource.Layout.HotspotFinder);

            var userObj = BackendProvider_Droid.GetUser;
            if (userObj == null)
            {
                var login = new Intent(this, typeof(Login));
                login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(login);
                Finish();
                return;
            }

            InitializeLayout();

            ShowLoader();

            GetLots();
        }

        private void InitializeLayout()
        {
            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
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

            imgBack = FindViewById<SvgImageView>(Resource.Id.imgBack);
            imgBack.SetSvg(this, Resource.Raw.AO__arrow_back);
            imgBack.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(20));
            imgBack.LayoutParameters.Width = Utils.CalcDimension(130);
            imgBack.Click += imgBack_Click;

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            pager = FindViewById<ViewPager>(Resource.Id.viewpager);
            tabs = FindViewById<TabLayout>(Resource.Id.tabs);

            frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout);
            frameLayout.SetPadding(Resources.DisplayMetrics.WidthPixels - Utils.CalcDimension(190), 0, 0, Utils.CalcDimension(100));
            fabMap = FindViewById<CustomButton>(Resource.Id.fabMap);
            fabMap.SetBackgroundResource(Resource.Drawable.Map);
            fabMap.Touch += (v, me) =>
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
                            fabMap_Click(null, null);
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
                            padleft = padleft + fabMap.Width > Resources.DisplayMetrics.WidthPixels ? Resources.DisplayMetrics.WidthPixels - fabMap.Width : padleft;
                            var padbottom = (_y * -1) + floatButton.yPad;
                            padbottom = padbottom + fabMap.Height > Resources.DisplayMetrics.HeightPixels - lytActionBar.LayoutParameters.Height - Utils.CalcDimension(45) ? Resources.DisplayMetrics.HeightPixels - lytActionBar.LayoutParameters.Height - Utils.CalcDimension(45) - fabMap.Height : padbottom;
                            frameLayout.SetPadding(padleft, 0, 0, padbottom);
                        }
                        break;
                }
                frameLayout.Invalidate();
            };

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(10), Utils.CalcDimension(70), Utils.CalcDimension(10));
            lytNotification.Visibility = ViewStates.Gone;
            txtNotification = new CustomTextView(this)
            {
                Gravity = GravityFlags.Left,
                Text = "",
                TextSize = 14
            };
            txtNotification.SetCustomFont(GetString(Resource.String.fontRegular));
            lytNotification.TranslationY = Utils.CalcDimension(100);
            lytNotification.AddView(txtNotification);
            lytNotification.LayoutParameters.Height = Utils.CalcDimension(100);
        }

        private bool HasLocationServiceOn
        {
            get
            {
                _locationManager = (LocationManager)GetSystemService(LocationService);

                var acceptableLocationProviders = _locationManager.GetProviders(new Criteria { Accuracy = Accuracy.Fine }, true);
                _locationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;

                return !string.IsNullOrEmpty(_locationProvider);
            }
        }

        private Location LastKnownLocation
        {
            get
            {
                IList<string> providers = _locationManager.GetProviders(false);
                Location bestLocation = null;
                foreach (string provider in providers)
                {
                    Location l = _locationManager.GetLastKnownLocation(provider);
                    if (l != null && (bestLocation == null || l.Accuracy < bestLocation.Accuracy))
                    {
                        bestLocation = l;
                    }
                }
                return bestLocation;
            }
        }

        private void GetLots()
        {
            try
            {
                if (!HasLocationServiceOn)
                {
                    var buttons = new string[] { "Turn On", "Cancel" };
                    locationAlert = AndroidAlert.ShowAlert(this, "Location services", "Please switch on location services and try again.", buttons);
                    locationAlert.ButtonClicked += (object sender, AlwaysOnAlertDialogArgs e) =>
                    {
                        switch (buttons[e.which])
                        {
                            case "Turn On":
                                {
                                    //StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                                    DisplayLocationSettingsRequest();
                                    HideLoader();
                                    break;
                                }
                            case "Cancel":
                                {
                                    BackPress();
                                    break;
                                }
                        }
                    };
                    locationAlert.Show();
                }
                else
                {
                    if (Hotspots != null && Hotspots.Count > 0)
                    {
                        SetAdapter();
                    }
                    else
                    {
                        string locationProvider = LocationManager.NetworkProvider;

                        _locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

                        var coords = LastKnownLocation;
                        if (coords != null)
                        {
                            RadiusCoords.CurrentLat = coords.Latitude;
                            RadiusCoords.CurrentLong = coords.Longitude;

                            if (ScrollCoords == null)
                                ScrollCoords = RadiusCoords;

                            BackendProvider.HotspotsReceived -= BackendProvider_HotspotsReceived;
                            BackendProvider.HotspotsReceived -= BackendProvider_HotspotsReceived;

                            BackendProvider.HotspotsReceived += BackendProvider_HotspotsReceived;
                            BackendProvider.GetHotspotsInternational(MainApplication.ApiKey, RadiusCoords);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BackPress();
                return;
            }
        }

        private void BackendProvider_HotspotsReceived(object sender, HotspotResponse e)
        {
            BackendProvider.HotspotsReceived -= BackendProvider_HotspotsReceived;

            Hotspots = e.HotspotList ?? new List<Hotspot>();

            RunOnUiThread(() =>
            {
                SetAdapter();
            });
        }

        void SetAdapter()
        {
            hasLocations = Hotspots.Count > 0;
            Task.Run(() =>
            {
                adapter = new HotspotFragmentAdapter(SupportFragmentManager);
                RunOnUiThread(() =>
                {
                    pager.Adapter = adapter;
                    tabs.SetupWithViewPager(pager);
                    tabs.LayoutParameters.Height = Utils.CalcDimension(95);
                    HideLoader();
                });
            });
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "HotspotFinder");
            StartActivity(menu);
            Finish();
        }

        void imgBack_Click(object sender, EventArgs e)
        {
            BackPress();
        }

        public void OnLocationChanged(Location location)
        {
            if (hasLocations) return;

            RadiusCoords.CurrentLat = location.Latitude;
            RadiusCoords.CurrentLong = location.Longitude;

            if (ScrollCoords == null)
                ScrollCoords = RadiusCoords;

            BackendProvider.HotspotsReceived -= BackendProvider_HotspotsReceived;
            BackendProvider.HotspotsReceived -= BackendProvider_HotspotsReceived;

            BackendProvider.HotspotsReceived += BackendProvider_HotspotsReceived;
            BackendProvider.GetHotspotsInternational(MainApplication.ApiKey, RadiusCoords);
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        protected override void OnResume()
        {
            base.OnResume();
            if (ExecuteOnResume)
            {
                ShowLoader();
                GetLots();
            }
            else
            {
                ExecuteOnResume = true;
            }
        }

        void fabMap_Click(object sender, EventArgs e)
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderListMap.ToString());

            StartActivity(new Intent(this, typeof(Maps)));
        }

        public void BackPress()
        {
            BackendProvider_Droid.UpdatePackagesFromServer();
            var intent = new Intent(this, typeof(Dashboard));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderListBack.ToString());

            BackPress();
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

        private void DisplayLocationSettingsRequest()
        {
            var googleApiClient = new GoogleApiClient.Builder(this).AddApi(LocationServices.API).Build();
            googleApiClient.Connect();

            var locationRequest = LocationRequest.Create();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(10000);
            locationRequest.SetFastestInterval(10000 / 2);

            var builder = new LocationSettingsRequest.Builder().AddLocationRequest(locationRequest);
            builder.SetAlwaysShow(true);

            var result = LocationServices.SettingsApi.CheckLocationSettings(googleApiClient, builder.Build());
            result.SetResultCallback((LocationSettingsResult callback) =>
            {
                switch (callback.Status.StatusCode)
                {
                    case LocationSettingsStatusCodes.Success:
                        {
                            GetLots();
                            break;
                        }
                    case LocationSettingsStatusCodes.ResolutionRequired:
                        {
                            try
                            {
                                // Show the dialog by calling startResolutionForResult(), and check the result
                                // in onActivityResult().
                                callback.Status.StartResolutionForResult(this, REQUEST_CHECK_SETTINGS);
                            }
                            catch (IntentSender.SendIntentException e)
                            {
                            }

                            break;
                        }
                    default:
                        {
                            // If all else fails, take the user to the android location settings
                            StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                            break;
                        }
                }
            });
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case REQUEST_CHECK_SETTINGS:
                    {
                        switch (resultCode)
                        {
                            case Android.App.Result.Ok:
                                {
                                    GetLots();
                                    break;
                                }
                            case Android.App.Result.Canceled:
                                {
                                    //No location
                                    BackPress();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}