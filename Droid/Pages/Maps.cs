using AlwaysOn;
using Android.App;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Maps")]
    public class Maps : FragmentActivity, ILocationListener
    {
        GoogleMap Map;
        List<Marker> Markers = new List<Marker>();
        bool ReRenderMap = true;
        public const int MAP_MARKER_LIMIT = 350;

        LinearLayout lytNotification;
        CustomTextView txtNotification;
        Animation fadeOutAnimation;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        FrameLayout frameLayout;
        CustomButton fabList;
        UIObjectMove floatButton = new UIObjectMove();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.HotspotFinderMap.ToString());

            SetContentView(Resource.Layout.Maps);

            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            var img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            var imgBack = FindViewById<SvgImageView>(Resource.Id.imgBack);
            imgBack.SetSvg(this, Resource.Raw.AO__arrow_back);
            imgBack.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(20));
            imgBack.LayoutParameters.Width = Utils.CalcDimension(130);
            imgBack.Click += (object sender, EventArgs e) =>
            {
                OnBackPressed();
            };

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout);
            frameLayout.SetPadding(Resources.DisplayMetrics.WidthPixels - Utils.CalcDimension(190), 0, 0, Utils.CalcDimension(100));
            fabList = FindViewById<CustomButton>(Resource.Id.fabList);
            fabList.SetBackgroundResource(Resource.Drawable.List);
            fabList.Touch += (v, me) =>
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
                            ShowLoader();

                            OnBackPressed();
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
                            padleft = padleft + fabList.Width > Resources.DisplayMetrics.WidthPixels ? Resources.DisplayMetrics.WidthPixels - fabList.Width : padleft;
                            var padbottom = (_y * -1) + floatButton.yPad;
                            padbottom = padbottom + fabList.Height > Resources.DisplayMetrics.HeightPixels - lytActionBar.LayoutParameters.Height - Utils.CalcDimension(45) ? Resources.DisplayMetrics.HeightPixels - lytActionBar.LayoutParameters.Height - Utils.CalcDimension(45) - fabList.Height : padbottom;
                            frameLayout.SetPadding(padleft, 0, 0, padbottom);
                        }
                        break;
                }
                frameLayout.Invalidate();
            };

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), 0);
            lytNotification.Visibility = ViewStates.Gone;
            txtNotification = FindViewById<CustomTextView>(Resource.Id.txtNotification);
            fadeOutAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeOutAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                txtNotification.Text = "";
                lytNotification.Visibility = ViewStates.Gone;
            };

            int status = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(this);

            // Showing status
            if (status != ConnectionResult.Success)
            {
                int requestCode = 10;
                Dialog dialog = GooglePlayServicesUtil.GetErrorDialog(status, this, requestCode);
                dialog.Show();
                return;
            }
            //((SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapView))
            var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.mapView);
            mapFragment.GetMapAsync(new MapCallback((object sender, GoogleMap mapReady) =>
            {
                Map = mapReady;
                //Map.StopAnimation();
                Map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(HotspotFinder.ScrollCoords.CurrentLat, HotspotFinder.ScrollCoords.CurrentLong), HotspotFinder.ZoomLevel));
                Map.MyLocationEnabled = true;
                Map.BuildingsEnabled = true;

                Map.CameraIdle += (object sender2, EventArgs e2) =>
                {
                    if (ReRenderMap)
                    {
                        RefreshMarkers();
                    }
                    else
                    {
                        ReRenderMap = true;
                    }
                };

                Map.MarkerClick += (object sender2, GoogleMap.MarkerClickEventArgs e2) =>
                {
                    e2.Marker.ShowInfoWindow();
                    Map.StopAnimation();
                    Map.AnimateCamera(CameraUpdateFactory.NewLatLng(e2.Marker.Position));
                    ReRenderMap = false;
                };

                RefreshMarkers();
            }));

            HideLoader();
        }

        public class MapCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            private EventHandler<GoogleMap> MapReady;
            public MapCallback(EventHandler<GoogleMap> mapReady)
            {
                MapReady += mapReady;
            }
            public void OnMapReady(GoogleMap googleMap)
            {
                if (MapReady != null)
                    MapReady(this, googleMap);
            }
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderMapBack.ToString());

            HotspotFinder.Hotspots = HotspotFinder.Hotspots.Take(100).ToList();

            Finish();
        }

        public void RefreshMarkers()
        {
            try
            {
                GeoDataProvider.GetCurrentPosition((e) =>
                {
                    HotspotFinder.RadiusCoords = e;

                    if (HotspotFinder.ScrollCoords == null)
                        HotspotFinder.ScrollCoords = HotspotFinder.RadiusCoords;
                });
                
                var bounds = Map.Projection.VisibleRegion.LatLngBounds;
                var boundLatA = bounds.Northeast.Latitude.ToString();
                var boundLatB = bounds.Southwest.Latitude.ToString();
                var boundLngA = bounds.Northeast.Longitude.ToString();
                var boundLngB = bounds.Southwest.Longitude.ToString();

                var userLat = HotspotFinder.RadiusCoords.CurrentLat.ToString();
                var userLng = HotspotFinder.RadiusCoords.CurrentLong.ToString();
                var centerLat = Map.Projection.VisibleRegion.LatLngBounds.Center.Latitude;
                var centerLng = Map.Projection.VisibleRegion.LatLngBounds.Center.Longitude;

                HotspotFinder.ScrollCoords.CurrentLat = centerLat;
                HotspotFinder.ScrollCoords.CurrentLong = centerLng;
                HotspotFinder.ZoomLevel = Map.CameraPosition.Zoom;

                BackendProvider.HotspotsReceived += HotspotsReceived;
                BackendProvider.GetHotspotsByBoundsInternational(MainApplication.ApiKey, boundLatA, boundLatB, boundLngA, boundLngB, userLat, userLng, centerLat.ToString(), centerLng.ToString(), MAP_MARKER_LIMIT);
            }
            catch { }
        }
        
        public void HotspotsReceived(object sender, HotspotResponse e)
        {
            BackendProvider.HotspotsReceived -= HotspotsReceived;

            if (e.Success)
            {
                HotspotFinder.Hotspots = (e.HotspotList ?? new List<AlwaysOn.Objects.Hotspot>()).Take(MAP_MARKER_LIMIT).ToList();

                RunOnUiThread(() =>
                {
                    Map.Clear();
                    Markers.Clear();
                });
            }
            
            if (Map != null)
            {
                RunOnUiThread(() =>
                {
                    var count = 0;
                    var error = false;
                    HotspotFinder.Hotspots.ForEach(n =>
                    {
                        try
                        {
                            MarkerOptions marker = new MarkerOptions();
                            marker.SetPosition(new LatLng(double.Parse(n.lat), double.Parse(n.lng)));
                            marker.SetTitle(n.data);
                            marker.SetIcon(BitmapDescriptorFactory.FromResource(n.superwifi ? Resource.Drawable.S_30 : (n.international ? Resource.Drawable.I_30 : Resource.Drawable.B_30)));
                            var sMarker = Map.AddMarker(marker);
                            
                            Markers.Add(sMarker);
                            count++;
                        }
                        catch (Exception ex)
                        {
                            error = error || true;
                        }
                    });

                    if (!error)
                    {
                        var hotspotLimit = count + 1 >= MAP_MARKER_LIMIT ? " Marker limit is " + MAP_MARKER_LIMIT + " on screen." : "";
                        txtNotification.Text = "Showing " + count + " hotspots." + hotspotLimit;
                        lytNotification.Visibility = ViewStates.Visible;
                        lytNotification.LayoutParameters.Height = Utils.CalcDimension(110);
                        lytNotification.StartAnimation(fadeOutAnimation);
                    }
                });
            }
        }
        
        public void OnLocationChanged(Location location)
        {
            HotspotFinder.RadiusCoords.CurrentLat = location.Latitude;
            HotspotFinder.RadiusCoords.CurrentLong = location.Longitude;

            if (HotspotFinder.ScrollCoords == null)
                HotspotFinder.ScrollCoords = HotspotFinder.RadiusCoords;
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

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
}