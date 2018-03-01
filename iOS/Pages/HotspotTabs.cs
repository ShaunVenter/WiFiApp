using AlwaysOn;
using AlwaysOn.Objects;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace AlwaysOn_iOS
{
    public class HotspotTabs : UITabBarController
    {
        UIScrollView _scrollWrapper = new UIScrollView();

        public static RadiusCoordinates RadiusCoords = new RadiusCoordinates();
        public static RadiusCoordinates ScrollCoords;
        public static float ZoomLevel = 15;
        public static List<Hotspot> Hotspots;
        private LoadingOverlay loadingOverlay;

        UIView _hotSpot = new UIView();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TabBar.Hidden = true;

            UtilProvider.MenuIsOpen = false;

            loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
            View.Add(loadingOverlay);

            if (Hotspots != null && Hotspots.Count > 0)
            {
                Hotspots = Hotspots.Take(100).ToList();
                LoadTabs();
            }
            else
            {
                GeoDataProvider.GetCurrentPosition((e) =>
                {
                    RadiusCoords = e;

                    if (ScrollCoords == null)
                        ScrollCoords = RadiusCoords;

                    BackendProvider.HotspotsReceived += BackendProvider_HotspotsReceived;
                    BackendProvider.GetHotspotsInternational(AppDelegate.ApiKey, e);
                });
            }

            AnalyticsProvider_iOS.PageViewGA(PageName.HotspotFinderList.ToString());
        }

        private void BackendProvider_HotspotsReceived(object sender, HotspotResponse e)
        {
            BackendProvider.HotspotsReceived -= BackendProvider_HotspotsReceived;

            Hotspots = e.HotspotList ?? new List<Hotspot>();

            InvokeOnMainThread(() =>
            {
                LoadTabs();
            });
        }

        private void LoadTabs()
        {
            var tabs = new UIViewController[] { GenericHotspot(), SuperHotspot() };

            View.Add(UtilProvider.HeaderStrip(true, true, tabs[0]));
            ViewControllers = tabs;

            TabBar.TintColor = UIColor.White;
            TabBar.BarTintColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorHotspotBarGrey);

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorHotspotBgGrey);
            View.BackgroundColor = UIColor.Blue;

            TabBar.Hidden = false;
            loadingOverlay.Hide();
        }

        public UIViewController GenericHotspot()
        {
            var generic = new UIViewController()
            {
                Title = "Show all WiFi Hotspots"
            };
            generic.TabBarItem.Image = UIImage.FromFile("allhotspots.png");
            generic.View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var listWrapper = new UIView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey)
            };
            int Index = -1;

            foreach (var item in Hotspots)
            {
                Index++;

                _hotSpot = new UIView(new CGRect(0, Index * (UtilProvider.ScreenHeight / 6), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight / 6))
                {
                    BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey),
                    LayoutMargins = new UIEdgeInsets(0, 0, 5, 0),
                };

                UILabel HotspotName = new UILabel(new CGRect(20, 0, _hotSpot.Bounds.Width - 80, 35))
                {
                    Text = item.data,
                    Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14),
                    TextColor = UIColor.White
                };

                UILabel AddressLabel = new UILabel(new CGRect(20, 20, _hotSpot.Bounds.Width - 80, 35))
                {
                    Text = item.address ?? "",
                    Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                    TextColor = UIColor.White
                };
                //Async
                if (string.IsNullOrEmpty(item.address))
                {
                    ReverseGeocodeCurrentLocation(new CoreLocation.CLLocationCoordinate2D(Double.Parse(item.lat), Double.Parse(item.lng)), (str) =>
                    {
                        InvokeOnMainThread(() =>
                        {
                            AddressLabel.Text = str;
                            AddressLabel.SetNeedsDisplay();
                        });
                    });
                }

                UILabel HotspotDistance = new UILabel(new CGRect(20, 35, _hotSpot.Bounds.Width - 80, 35))
                {
                    Text = Math.Round(item.distance, 2) + " km",
                    Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                    TextColor = UIColor.White
                };
                //
                UIImageView Image = new UIImageView(UIImage.FromFile(item.superwifi ? "S.png" : (item.international ? "I.png" : "B.png")))
                {
                    Frame = new CGRect(UtilProvider.ScreenWidth - 65, 5, 30, 30),
                    ContentMode = UIViewContentMode.ScaleAspectFit
                };

                _hotSpot.Add(HotspotName);
                _hotSpot.Add(AddressLabel);
                _hotSpot.Add(HotspotDistance);
                _hotSpot.Add(Image);
                _hotSpot.Add(UtilProvider.ConnectionSpacer(_hotSpot));

                listWrapper.Add(_hotSpot);
            }

            _scrollWrapper = new UIScrollView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th + 10, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorHotspotBgGrey),
                ContentSize = new CGSize(listWrapper.Bounds.Width, (Index * (UtilProvider.ScreenHeight / 6)) + 200),
                ContentOffset = new CGPoint(0, 0)
            };

            _scrollWrapper.AddSubview(listWrapper);

            generic.Add(_scrollWrapper);

            var _btn = UtilProvider.FloatingBtn("Map");
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
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderListMap.ToString());

                NavigationController.PushViewController(new HotspotsMap(), true);
            };
            generic.Add(_btn);

            return generic;
        }

        public UIViewController SuperHotspot()
        {
            var super = new UIViewController()
            {
                Title = "Show Super WiFi Hotspots"
            };
            super.TabBarItem.Image = UIImage.FromFile("super.png");
            super.View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var listWrapper = new UIView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey)
            };
            int Index = -1;

            foreach (var item in Hotspots)
            {
                if (item.superwifi)
                {
                    Index++;

                    _hotSpot = new UIView(new CGRect(0, Index * (UtilProvider.ScreenHeight / 6), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight / 6))
                    {
                        BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey),
                        LayoutMargins = new UIEdgeInsets(0, 0, 5, 0),
                    };

                    UILabel HotspotName = new UILabel(new CGRect(20, 0, _hotSpot.Bounds.Width - 80, 35))
                    {
                        Text = item.data,
                        Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14),
                        TextColor = UIColor.White
                    };

                    UILabel AddressLabel = new UILabel(new CGRect(20, 20, _hotSpot.Bounds.Width - 80, 35))
                    {
                        Text = item.address ?? "",
                        Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                        TextColor = UIColor.White
                    };
                    //Async
                    if (string.IsNullOrEmpty(item.address))
                    {
                        ReverseGeocodeCurrentLocation(new CoreLocation.CLLocationCoordinate2D(Double.Parse(item.lat), Double.Parse(item.lng)), (str) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                AddressLabel.Text = str;
                                AddressLabel.SetNeedsDisplay();
                            });
                        });
                    }

                    UILabel HotspotDistance = new UILabel(new CGRect(20, 35, _hotSpot.Bounds.Width - 80, 35))
                    {
                        Text = Math.Round(item.distance, 2) + " km",
                        Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                        TextColor = UIColor.White
                    };

                    UIImageView Image = new UIImageView(UIImage.FromFile(item.superwifi ? "S.png" : (item.international ? "I.png" : "B.png")))
                    {
                        Frame = new CGRect(UtilProvider.ScreenWidth - 65, 5, 30, 30),
                        ContentMode = UIViewContentMode.ScaleAspectFit
                    };

                    _hotSpot.Add(HotspotName);
                    _hotSpot.Add(AddressLabel);
                    _hotSpot.Add(HotspotDistance);
                    _hotSpot.Add(Image);
                    _hotSpot.Add(UtilProvider.ConnectionSpacer(_hotSpot));

                    listWrapper.Add(_hotSpot);
                }
            }

            _scrollWrapper = new UIScrollView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th + 10, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorHotspotBgGrey),
                ContentSize = new CGSize(listWrapper.Bounds.Width, (Index * (UtilProvider.ScreenHeight / 6)) + 200),
                ContentOffset = new CGPoint(0, 0)
            };

            _scrollWrapper.AddSubview(listWrapper);

            super.Add(_scrollWrapper);

            var _btn = UtilProvider.FloatingBtn("Map");
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
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderListMap.ToString());

                NavigationController.PushViewController(new HotspotsMap(), true);
            };
            super.Add(_btn);

            return super;
        }

        public void ReverseGeocodeCurrentLocation(CoreLocation.CLLocationCoordinate2D location, Action<string> Callback)
        {
            try
            {
                new Google.Maps.Geocoder().ReverseGeocodeCord(location, new Google.Maps.ReverseGeocodeCallback((Google.Maps.ReverseGeocodeResponse re, NSError err) =>
                {
                    if (re != null && re.FirstResult != null)
                    {
                        Callback.Invoke(string.Join(" ", re.FirstResult.Lines));
                    }
                }));
            }
            catch (Exception ex)
            {
            }
        }
    }
}