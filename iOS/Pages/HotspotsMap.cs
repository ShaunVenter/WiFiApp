using System;
using UIKit;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using AlwaysOn;
using Google.Maps;
using CoreLocation;
using System.Collections.Generic;
using System.Linq;
using AlwaysOn.Objects;

namespace AlwaysOn_iOS
{
    public partial class HotspotsMap : UIViewController
    {
        MapView mapView;
        List<Marker> Markers = new List<Marker>();
        bool ReRenderMap = true;
        public const int MAP_MARKER_LIMIT = 350;

        public HotspotsMap() : base("HotspotsMap", null)
        {
            UtilProvider.MenuIsOpen = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            AnalyticsProvider_iOS.PageViewGA(PageName.HotspotFinderMap.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            try
            {
                var camera = CameraPosition.FromCamera(HotspotTabs.ScrollCoords.CurrentLat, HotspotTabs.ScrollCoords.CurrentLong, HotspotTabs.ZoomLevel);
                mapView = MapView.FromCamera(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight), camera);

                UIButton myLocationButton = new UIButton(new CGRect(UtilProvider.ScreenWidth - 50, 10, 40, 40));
                myLocationButton.Add(new UIImageView(UIImage.FromFile("CenterLoc.png")) { Frame = new CGRect(0, 0, 40, 40) });
                myLocationButton.TouchUpInside += delegate
                {
                    mapView.Animate(new CLLocationCoordinate2D(HotspotTabs.RadiusCoords.CurrentLat, HotspotTabs.RadiusCoords.CurrentLong));
                    mapView.Animate(15f);
                };
                mapView.Add(myLocationButton);
                mapView.MyLocationEnabled = true;
                mapView.BuildingsEnabled = true;

                mapView.CameraPositionIdle += (object sender, GMSCameraEventArgs e) =>
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

                mapView.TappedMarker = (map, marker) =>
                {
                    map.Animate(marker.Position);
                    ReRenderMap = false;

                    return false;
                };

                RefreshMarkers();
            }
            catch { }

            View.Add(mapView);

            var _btn = UtilProvider.FloatingBtn("List");
            var dd = new DragDropGestureRecognizer();
            dd.Dragging += (object sender, DragDropEventArgs e) =>
            {
                var view = ((DragDropGestureRecognizer)sender).View;

                // Reposition box.
                var x = e.ViewWasAt.X + e.Delta.X;
                var y = e.ViewWasAt.Y + e.Delta.Y;
                view.Center = new CGPoint(x, y);
            };
            _btn.AddGestureRecognizer(dd);
            _btn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderMapList.ToString());

                NavigationController.PushViewController(new HotspotTabs(), true);
            };
            View.Add(_btn);

            View.Add(UtilProvider.HeaderStrip(true, false, this));
        }

        public void RefreshMarkers()
        {
            try
            {
                GeoDataProvider.GetCurrentPosition((e) =>
                {
                    HotspotTabs.RadiusCoords = e;

                    if (HotspotTabs.ScrollCoords == null)
                        HotspotTabs.ScrollCoords = HotspotTabs.RadiusCoords;
                });

                var bounds = new CoordinateBounds(mapView.Projection.VisibleRegion);
                var boundLatA = bounds.NorthEast.Latitude.ToString();
                var boundLatB = bounds.SouthWest.Latitude.ToString();
                var boundLngA = bounds.NorthEast.Longitude.ToString();
                var boundLngB = bounds.SouthWest.Longitude.ToString();
                
                var userLat = HotspotTabs.RadiusCoords.CurrentLat.ToString();
                var userLng = HotspotTabs.RadiusCoords.CurrentLong.ToString();
                var centerLat = mapView.Projection.CoordinateForPoint(mapView.Center).Latitude;
                var centerLng = mapView.Projection.CoordinateForPoint(mapView.Center).Longitude;

                HotspotTabs.ScrollCoords.CurrentLat = centerLat;
                HotspotTabs.ScrollCoords.CurrentLong = centerLng;
                HotspotTabs.ZoomLevel = mapView.Camera.Zoom;
                
                BackendProvider.HotspotsReceived += HotspotsReceived;
                BackendProvider.GetHotspotsByBoundsInternational(AppDelegate.ApiKey, boundLatA, boundLatB, boundLngA, boundLngB, userLat, userLng, centerLat.ToString(), centerLng.ToString(), MAP_MARKER_LIMIT);
            }
            catch { }
        }
        
        public void HotspotsReceived(object sender, HotspotResponse e)
        {
            BackendProvider.HotspotsReceived -= HotspotsReceived;

            InvokeOnMainThread(() =>
            {
                if (e.Success)
                {
                    HotspotTabs.Hotspots = (e.HotspotList ?? new List<AlwaysOn.Objects.Hotspot>()).Take(MAP_MARKER_LIMIT).ToList();
                    Markers.ForEach(n =>
                    {
                        n.Title = null;
                        n.AppearAnimation = MarkerAnimation.None;
                        n.Map = null;
                        n.Icon = null;
                        n = null;
                    });
                }

                Markers.Clear();

                Markers.AddRange(HotspotTabs.Hotspots.Select(h => new Marker()
                {
                    Map = mapView,
                    Position = new CLLocationCoordinate2D(double.Parse(h.lat), double.Parse(h.lng)),
                    Title = string.Format(h.data),
                    AppearAnimation = MarkerAnimation.Pop,
                    Icon = UIImage.FromFile(h.superwifi ? "S.png" : (h.international ? "I.png" : "B.png"))
                }));

                var count = Markers.Count;
                var hotspotLimit = count + 1 >= MAP_MARKER_LIMIT ? " Marker limit is " + MAP_MARKER_LIMIT + " on screen." : "";
                var Message = "Showing " + count + " hotspots." + hotspotLimit;

                View.AddSubview(UtilProvider.GenericError(Message));
            });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
