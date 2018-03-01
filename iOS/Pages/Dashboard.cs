using AlwaysOn;
using AlwaysOn.Objects;
using AlwaysOn_iOS.Objects;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MaterialControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using XamSvg;

namespace AlwaysOn_iOS
{
    public partial class Dashboard : UIViewController
    {
        List<UserPackage> _Packages = new List<UserPackage>();
        LoadingOverlay loadingOverlay;
        UIView _noConnectionWrapper = new UIView();
        UIView _mainConnection = new UIView();
        UIView _labelWrapper = new UIView();
        UILabel _noConnection = new UILabel();
        UILabel _coolInternet = new UILabel();
        MDSwitch packageToggle;
        UIView _instructionHolder = null;
        MDButton _linkPackage = new MDButton();
        UIScrollView _scrollWrapper = new UIScrollView();

        bool allowAction = true;

        InfoButton _infoButton = new InfoButton();
        MDButton InfoBtn = new MDButton();

        UISvgImageView _orangeWarning = new UISvgImageView();

        bool _disableDragDrop = false;
        List<UIViewPosition> DragList = null;
        float DragSnapPosition = -1;
        List<float> TempCenters = new List<float>();
        static HotspotHelper.StatePackage hotspotHelperState = new HotspotHelper.StatePackage();

        public Dashboard(InfoButton infoButton) : base("Dashboard", null)
        {
            UtilProvider.MenuIsOpen = false;
            _infoButton = infoButton;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            allowAction = true;

            AnalyticsProvider_iOS.PageViewGA(PageName.Dashboard.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            _labelWrapper = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th + _mainConnection.Bounds.Height, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th - 5))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey)
            };
            View.Add(_labelWrapper);

            //InitializePackages();

            BackendProvider_iOS.GetServerSSIDs();

            View.Add(UtilProvider.ConnectionError("Getting connectivity status"));

            HotspotHelper.GetConnectivityStatePackage((sp) =>
            {
                hotspotHelperState = sp;

                InvokeOnMainThread(() => InitializePackages());
            });
        }

        private void InitializePackages()
        {
            if (!hotspotHelperState.ConnectedToAlwaysOn)
            {
                View.Add(UtilProvider.ConnectionError("You are not connected to an AlwaysOn Hotspot"));
            }
            else
            {
                if (!hotspotHelperState.Authenticated)
                {
                    CreateInstructionsSubview();
                }
            }

            _Packages = BackendProvider_iOS.GetStoredPackages();
            if (_Packages == null || _Packages.Count == 0)
            {
                _Packages = BackendProvider_iOS.GetPackagesFromServerSync();
                if (_Packages == null || _Packages.Count == 0)
                {
                    View.Add(NoPackageOverlay());
                }
                else
                {
                    CreatePackageList();
                }
            }
            else
            {
                CreatePackageList();
            }

            View.Add(UtilProvider.HeaderStrip(false, true, this));

            InfoBtn = UtilProvider.FloatingBtn(_infoButton.Title.Replace(@"\r\n", "\r\n"));
            InfoBtn.AddGestureRecognizer(new DragDropGestureRecognizer(null, (object sender, DragDropEventArgs e) =>
            {
                (sender as DragDropGestureRecognizer).View.Center = new CGPoint(e.ViewWasAt.X + e.Delta.X, e.ViewWasAt.Y + e.Delta.Y);
            }, null));
            InfoBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.PageViewGA(PageName.InfoButton.ToString());
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardInfoButton.ToString());

                View.Add(UtilProvider.LoadInfoWebView(this, _infoButton.Url + "?aoid=" + BackendProvider_iOS.GetUser.UserId + "&t=" + (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) + "&p=nopad"));
            };
            InfoBtn.Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 22.5, UtilProvider.ScreenHeight - 65 - UtilProvider.SafeBottom, 55, 55);
            View.Add(InfoBtn);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void CreateInstructionsSubview()
        {
            if (_instructionHolder == null)
            {
                _instructionHolder = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight / 12))
                {
                    BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan)
                };

                var viewWrapper = new UIView(new CGRect(40, UtilProvider.SafeTop, UtilProvider.ScreenWidth - 40, UtilProvider.ScreenHeight / 12));

                var Message = new UILabel(new CGRect(0, UtilProvider.SafeTop + (viewWrapper.Bounds.Height / 2) - 15, 230, 30))
                {
                    Text = "Switch to browse with package",
                    TextColor = UIColor.White,
                    Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12)
                };

                packageToggle = new MDSwitch()
                {
                    Frame = new CGRect(UtilProvider.ScreenWidth - 140, UtilProvider.SafeTop + (UtilProvider.ScreenHeight12th / 2) - 20, 100, 40),
                    //BackgroundColor = UIColor.DarkGray,
                    ThumbOn = UIColor.White,
                    ThumbOff = UIColor.White,
                    TrackOff = UIColor.Clear.FromHex(ConfigurationProvider.AppColorSwicthTrack),
                    TrackOn = UIColor.Clear.FromHex(ConfigurationProvider.AppColorSwicthTrack)
                };

                viewWrapper.Add(packageToggle);
                viewWrapper.Add(Message);

                _instructionHolder.Add(viewWrapper);

                View.Add(_instructionHolder);

                Task.Run(() =>
                {
                    while (true)
                    {
                        NSThread.SleepFor(2);
                        InvokeOnMainThread(() => packageToggle.On = true);
                        NSThread.SleepFor(0.5);
                        InvokeOnMainThread(() => packageToggle.On = false);
                        NSThread.SleepFor(0.5);
                        InvokeOnMainThread(() => packageToggle.On = true);
                        NSThread.SleepFor(0.5);
                        InvokeOnMainThread(() => packageToggle.On = false);
                    }
                });
            }
            else
            {
                View.BringSubviewToFront(_instructionHolder);
            }
        }

        private UIView NoPackageOverlay()
        {
            //View.BackgroundColor = UIColor.Clear.FromHex (ConfigurationProvider.AppColorPurple);
            _noConnectionWrapper = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                //BackgroundColor =
            };

            var gradient = new CAGradientLayer();
            gradient.Frame = _noConnectionWrapper.Layer.Bounds;
            gradient.Colors = new CoreGraphics.CGColor[]
            {
                UIColor.FromRGB (221, 94, 91).CGColor,
                UIColor.FromRGB (224, 95, 89).CGColor
            };
            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            _noConnectionWrapper.Layer.AddSublayer(gradient);

            _noConnection = new UILabel(new CGRect((UtilProvider.ScreenWidth / 2) - 100, (UtilProvider.ScreenHeight / 12) * 3, 200, 50))
            {
                Lines = 2,
                Text = "You dont have any packages linked to this account",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };

            _coolInternet = new UILabel(new CGRect((UtilProvider.ScreenWidth / 2) - 100, (UtilProvider.ScreenHeight / 12) * 5, 200, 50))
            {
                Lines = 2,
                Text = "The Internet is pretty cool. Lets get you online.",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 14),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };

            _linkPackage = new MDButton(new CGRect((UtilProvider.ScreenWidth / 2) - (((UtilProvider.ScreenWidth / 2) - 45) / 2), (UtilProvider.ScreenHeight / 2) + 65, (UtilProvider.ScreenWidth / 2) - 45, 35), MDButtonType.Flat, UIColor.White);
            _linkPackage.SetTitle("LINK A PACKAGE", UIControlState.Normal);
            _linkPackage.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            _linkPackage.BackgroundColor = UIColor.Clear;
            _linkPackage.Layer.BorderWidth = 1;
            _linkPackage.Layer.BorderColor = UIColor.White.CGColor;
            _linkPackage.TouchUpInside += (object sender, EventArgs e) =>
            {
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuLinkPackage.ToString());

                BackendProvider_iOS.GetServiceProviders();

                NavigationController.PushViewController(new RedeemVoucherPage(), true);
            };

            _mainConnection = new UIView(new CGRect(0, UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight / 1.8))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan)
            };

            _noConnectionWrapper.Add(_noConnection);
            _noConnectionWrapper.Add(_coolInternet);
            _noConnectionWrapper.Add(_linkPackage);

            return _noConnectionWrapper;
        }

        private void CreatePackageList()
        {
            if (_scrollWrapper.IsDescendantOfView(View))
            {
                _scrollWrapper.RemoveFromSuperview();
            }

            var listWrapper = new UITableView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey),
                SeparatorColor = UIColor.Clear
            };

            var _refreshLayout = new UIRefreshControl()
            {
                TintColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan)
            };
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                _refreshLayout.ValueChanged += (object sender, EventArgs e) =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        var sp = BackendProvider_iOS.GetStoredServiceProviders();
                        if (sp.Result == OperationResult.Failure || (sp.Result == OperationResult.Success && ((ServiceProviders)sp.Response).Items?.Count == 0))
                        {
                            BackendProvider_iOS.GetServiceProviders();
                        }

                        BackendProvider_iOS.UpdatePackagesFromServer();

                        InvokeOnMainThread(() =>
                        {
                            _refreshLayout.EndRefreshing();

                            ViewDidLoad();
                        });
                    }, TaskCreationOptions.LongRunning);
                };

                listWrapper.RefreshControl = _refreshLayout;
            }

            DragList = null;
            TempCenters = new List<float>();
            int Index = -1;

            if (_Packages != null)
            {
                try
                {
                    foreach (var package in _Packages)
                    {
                        Index++;

                        //var _userPackagePresentaion = new UserPackagePresentation (package)
                        package.Sanitize();

                        var _lytListItemWrapper = new UIButton(new CGRect(0, Index * 90, UtilProvider.ScreenWidth, 90))
                        {
                            BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey),
                            LayoutMargins = new UIEdgeInsets(0, 0, 5, 0),
                            Alpha = new nfloat(0.7),
                            Tag = package.Id
                        };

                        var _Title = new UILabel(new CGRect(22, (_lytListItemWrapper.Bounds.Height / 2) - 33, 60, 30))
                        {
                            TextColor = UIColor.White,
                            TextAlignment = UITextAlignment.Center,
                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 20),
                            Lines = 2,
                            LineBreakMode = UILineBreakMode.Clip
                        };

                        var _usageUnit = new UILabel(new CGRect(20, (_lytListItemWrapper.Bounds.Height / 2) - 14, 60, 30))
                        {
                            Text = package.GroupUnit,// Dynamic
                            TextColor = UIColor.White,
                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 15),
                            TextAlignment = UITextAlignment.Center
                        };

                        _orangeWarning = UtilProvider.OrangeExpiryWarning();
                        _orangeWarning.Frame = new CGRect(0, (_lytListItemWrapper.Bounds.Height / 2) + 5, 90, 40);

                        var _expiresSoon = new UILabel(new CGRect(0, (_lytListItemWrapper.Bounds.Height / 2) + 15, 90, 20))
                        {
                            Text = "Expires soon!",
                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12),
                            TextColor = UIColor.White,
                            TextAlignment = UITextAlignment.Center
                        };

                        var txtLeftValue = new UILabel(new CGRect(100, 13, 180, 20))
                        {
                            Text = package.UsageValue + " " + package.Unit + "  (" + package.UsageLeftPercentage.Split(',')[0] + "%) Left",
                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 13),
                            TextColor = UIColor.White
                        };

                        DateTime? _ExpiryDate = !string.IsNullOrWhiteSpace(package.ExpiryDate) ? Convert.ToDateTime(package.ExpiryDate) : (DateTime?)null;
                        var _inactiveExpiry = new UILabel(new CGRect(100, (_lytListItemWrapper.Bounds.Height / 2) - 10, 140, 20))
                        {
                            Text = _ExpiryDate != null ? "Expires " + _ExpiryDate.Value.ToString("dd MMM yyyy") : "No Expiry",
                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 11),
                            TextColor = UIColor.White
                        };

                        package.toggle = new MDSwitch()
                        {
                            Frame = new CGRect(UtilProvider.ScreenWidth - 100, (_lytListItemWrapper.Bounds.Height / 2) - 20, 100, 40),
                            //BackgroundColor = UIColor.DarkGray,
                            ThumbOn = UIColor.White,
                            ThumbOff = UIColor.White,
                            TrackOff = UIColor.Clear.FromHex(ConfigurationProvider.AppColorSwicthTrack),
                            TrackOn = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan),
                            Tag = package.Id,
                            On = false
                        };

                        if (package.LoginUserName == hotspotHelperState.AuthenticatedUsername)
                        {

                            //if (int.Parse(package.UsagePercentValue) >= 75)
                            //{
                            _lytListItemWrapper.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
                            //}
                            //else if (int.Parse(package.UsagePercentValue) >= 50)
                            //{
                            //    _package.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
                            //}
                            //else if (int.Parse(package.UsagePercentValue) >= 25)
                            //{
                            //    _package.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorYellow);
                            //}
                            //else
                            //{
                            //    _package.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorErrorOrange);
                            //}

                            var connectedBtn = UtilProvider.activeDurationIndicatorButton(int.Parse(package.UsagePercentValue), _lytListItemWrapper);
                            connectedBtn.TouchUpInside += (object sender, EventArgs e) =>
                            {
                                _lytListItemWrapper.SendActionForControlEvents(UIControlEvent.TouchUpInside);
                            };
                            _lytListItemWrapper.AddSubview(connectedBtn);

                            UILabel isOnline = new UILabel(new CGRect(0, (_lytListItemWrapper.Bounds.Height / 2) + 5, UtilProvider.ScreenWidth, 30))
                            {
                                Text = "Online with this Package",
                                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12),
                                TextColor = UIColor.White,
                                TextAlignment = UITextAlignment.Center
                            };
                            _lytListItemWrapper.Add(isOnline);
                            (package.toggle as MDSwitch).On = true;
                        }

                        if (package.UsageLeftvalue != "Package Used Up" && (_ExpiryDate == null || DateTime.Today.Date <= _ExpiryDate.Value) && package.UsageLeftPercentage != "0")
                        {
                            _lytListItemWrapper.Add(package.toggle as MDSwitch);
                        }

                        _lytListItemWrapper.Add(UtilProvider.InactiveDurationIndicator(int.Parse(package.UsagePercentValue), _lytListItemWrapper));
                        _lytListItemWrapper.Add(UtilProvider.ConnectionSpacer(_lytListItemWrapper));

                        if ((_ExpiryDate != null && DateTime.Today.AddDays(3).Date >= _ExpiryDate.Value) && package.ExpiryDate != null)
                        {
                            _lytListItemWrapper.Add(_orangeWarning);
                            _lytListItemWrapper.Add(_expiresSoon);
                        }

                        var packageStatus = new UILabel(new CGRect((_lytListItemWrapper.Bounds.Width / 2) - 50, (_lytListItemWrapper.Bounds.Height / 2) - 14, 160, 30))
                        {
                            Text = "",// Dynamic
                            TextColor = UIColor.White,
                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 13),
                            TextAlignment = UITextAlignment.Center
                        };

                        var spid = Convert.ToInt32(string.IsNullOrEmpty(package.ServiceProviderId) ? "0" : package.ServiceProviderId);
                        package.GroupName = spid > 1 ? "Service Provider" : package.GroupName;
                        _Title.Text = string.IsNullOrWhiteSpace(package.GroupNumber) ? package.GroupName : package.GroupNumber + "\n" + package.GroupUnit;

                        if (package.UsageLeftvalue == "Uncapped" || package.UsageLeftvalue == "Package Used Up" || package.GroupName == "Service Provider")
                        {
                            _Title.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 15);
                            _Title.Frame = new CGRect(20, (_lytListItemWrapper.Bounds.Height / 2) - 15, 130, 30);
                            _Title.TextAlignment = UITextAlignment.Left;

                            if (package.UsageLeftvalue == "Package Used Up")
                            {
                                packageStatus.Text = (_ExpiryDate != null && DateTime.Today.Date > _ExpiryDate.Value) ? "Package Expired on: " + _ExpiryDate.Value.ToString("dd MMM yyyy") : "Package Depleted";
                            }

                            if (package.GroupName == "Service Provider")
                            {
                                _Title.Text = "Service\nProvider";
                                _Title.Frame = new CGRect(20, (_lytListItemWrapper.Bounds.Height / 2) - 30, 130, 60);

                                packageStatus.Text = "Rate limited";

                                if (spid > 1)
                                {
                                    txtLeftValue.Text = BackendProvider_iOS.GetServiceProviderName(spid.ToString());
                                    _lytListItemWrapper.Add(txtLeftValue);

                                    _inactiveExpiry.Text = "Rate Limited";
                                    _lytListItemWrapper.Add(_inactiveExpiry);
                                }
                            }
                            else if (package.UsageLeftvalue == "Uncapped")
                            {
                                _Title.Text = "Uncapped";
                            }
                            else
                            {
                                _lytListItemWrapper.Add(packageStatus);
                            }
                        }
                        else
                        {
                            _lytListItemWrapper.Add(_usageUnit);
                            _lytListItemWrapper.Add(txtLeftValue);
                            _lytListItemWrapper.Add(_inactiveExpiry);
                        }

                        _lytListItemWrapper.Add(_Title);

                        var ddgr = new DragDropGestureRecognizer();
                        ddgr.Held += (object sender, UIView e) =>
                        {
                            listWrapper.BringSubviewToFront(e);
                        };
                        ddgr.Dragging += (object sender, DragDropEventArgs e) =>
                        {
                            if (!_disableDragDrop)
                            {
                                var view = ((DragDropGestureRecognizer)sender).View;

                                var CenterX = e.ViewWasAt.X;
                                var OldY = e.ViewWasAt.Y;
                                var NewY = OldY + e.Delta.Y;

                                //Move Current Dragging Item
                                view.Center = new CGPoint(CenterX, NewY);

                                var currentIndex = TempCenters.IndexOf(DragSnapPosition > -1 ? DragSnapPosition : OldY);

                                var currentCenter = TempCenters[currentIndex];
                                var beforeCenter = currentIndex == 0 ? -1 : TempCenters[currentIndex - 1];
                                var afterCenter = currentIndex + 1 > TempCenters.Count - 1 ? -1 : TempCenters[currentIndex + 1];

                                var current = DragList.Where(n => n.newY == currentCenter).FirstOrDefault();

                                //Swop with item below
                                if (afterCenter != -1 && NewY > afterCenter)
                                {
                                    var after = DragList.Where(n => n.newY == afterCenter).FirstOrDefault();

                                    DragSnapPosition = after.newY;
                                    after.newY = current.newY;
                                    current.newY = DragSnapPosition;

                                    MDButton.Animate(0.333, 0, UIViewAnimationOptions.CurveEaseInOut, () => { after.uiView.Center = new CGPoint(CenterX, after.newY); }, () => { });
                                }

                                //Swop with item above
                                if (beforeCenter != -1 && NewY < beforeCenter)
                                {
                                    var before = DragList.Where(n => n.newY == beforeCenter).FirstOrDefault();

                                    DragSnapPosition = before.newY;
                                    before.newY = current.newY;
                                    current.newY = DragSnapPosition;

                                    MDButton.Animate(0.333, 0, UIViewAnimationOptions.CurveEaseInOut, () => { before.uiView.Center = new CGPoint(CenterX, before.newY); }, () => { });
                                }
                            }
                        };
                        ddgr.Dropped += (object sender, DragDropEventArgs e) =>
                        {
                            if (!_disableDragDrop)
                            {
                                DragSnapPosition = -1;

                                DragList = DragList.OrderBy(v => v.newY).ToList();
                                for (int i = 0; i < DragList.Count; i++)
                                {
                                    DragList[i].newY = TempCenters[i];
                                    DragList[i].originalY = TempCenters[i];
                                    UIView.Animate(0.333, 0, UIViewAnimationOptions.CurveEaseInOut, () => { DragList[i].uiView.Center = new CGPoint(e.ViewWasAt.X, TempCenters[i]); }, () => { });
                                }

                                DragList = DragList.OrderBy(n => n.originalY).ToList();

                                var orderedPackages = (from dl in DragList.Select(n => (int)n.uiView.Tag).ToList()
                                                       join p in _Packages on dl equals p.Id
                                                       select p).ToList();

                                BackendProvider_iOS.SetPackages(orderedPackages);
                            }
                        };
                        _lytListItemWrapper.AddGestureRecognizer(ddgr);
                        _lytListItemWrapper.TouchUpInside += async (object sender, EventArgs e) =>
                        {
                            AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageSelect.ToString());

                            _refreshLayout.EndRefreshing();

                            bool connectionStatusAlert = false;
                            var title = package.GroupName + " (" + package.LoginUserName + ")";
                            var message = "Select an action below";
                            var buttons = new string[0];

                            HotspotHelper.WisprGatewayStatus((SessionInfo) =>
                            {
                                if (hotspotHelperState.AuthenticatedUsername == package.LoginUserName)
                                {
                                    connectionStatusAlert = true;
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

                                    message = string.Join("\n", fields) + "\n";
                                }
                                else
                                {
                                    buttons = new string[] { "Connect", "View Details", "Unlink from Account", "Cancel" };
                                }

                                InvokeOnMainThread(async () =>
                                {
                                    var action = await ShowAlert(this, title, message, connectionStatusAlert, buttons);
                                    switch (buttons[action])
                                    {
                                        case "Disconnect":
                                            {
                                                (package.toggle as MDSwitch).On = false;
                                                break;
                                            }
                                        case "Connect":
                                            {
                                                (package.toggle as MDSwitch).On = true;
                                                break;
                                            }
                                        case "View Details":
                                            {
                                                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageViewDetails.ToString());

                                                var fields = new List<string>();
                                                fields.Add("<b>Package:</b> " + package.GroupName);
                                                fields.Add("<b>Username:</b> " + package.LoginUserName);
                                                fields.Add("<b>Expiration:</b> " + (!string.IsNullOrWhiteSpace(package.ExpiryDate) ? Convert.ToDateTime(package.ExpiryDate).ToString("dd MMM yyyy") : "No Expiry"));
                                                fields.Add("<b>Type:</b> " + package.AccountTypeDesc);
                                                var providerName = BackendProvider_iOS.GetServiceProviderName(package.ServiceProviderId);
                                                if (providerName != "Unknown")
                                                {
                                                    fields.Add("<b>Provider:</b> " + providerName);
                                                }
                                                if (package.UsageLeftvalue != "Service Provider Rate Limited")
                                                {
                                                    fields.Add("<b>Usage Left:</b> " + package.UsageLeftvalue);
                                                }

                                                var msg = string.Join("\n", fields) + "\n";

                                                await ShowAlert(this, "Package Details", msg, true, "Close");
                                                break;
                                            }
                                        case "Unlink from Account":
                                            {
                                                var user = BackendProvider_iOS.GetUser;
                                                var msg = "Are you sure you want to unlink package \"" + package.GroupName + " (" + package.LoginUserName + ")\" from your account?";
                                                msg += "\n\nYou will be required to enter your AlwaysOn account password for \"" + user.LoginCredential + "\".";
                                                msg += "\n\nThe package username and password will be displayed onscreen for you to save.";
                                                var unlinkaction = await ShowAlert(this, "Unlink from Account", msg, false, "Yes", "No");
                                                if (unlinkaction == 0)
                                                {
                                                    var errorMsg = "";
                                                    AlertTextfieldFeedback unlinkpasswordaction = new AlertTextfieldFeedback() { ButtonIndex = 0, TextFieldText = "" };
                                                    while (unlinkpasswordaction.ButtonIndex == 0 && unlinkpasswordaction.TextFieldText != user.Password)
                                                    {
                                                        unlinkpasswordaction = await ShowAlertWithTextField(this, "Unlink Package", errorMsg + "Please enter your AlwaysOn account password for \"" + user.LoginCredential + "\" to unlink package \"" + package.GroupName + " (" + package.LoginUserName + ")\"", true, "Unlink Package", "Cancel");
                                                        errorMsg = "Incorrect password\n\n";
                                                    }
                                                    if (unlinkpasswordaction.ButtonIndex == 0)
                                                    {
                                                        loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                                                        View.Add(loadingOverlay);

                                                        var unlinkOperation = BackendProvider.UnlinkPackageSync(AppDelegate.ApiKey, package.Id);
                                                        if (unlinkOperation.Success)
                                                        {
                                                            AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageUnlink.ToString());

                                                            if (loadingOverlay != null) loadingOverlay.Hide();

                                                            var unlinked = " Package was successfully unlinked from your account.\n\n<b>Package:</b> " + package.GroupName + "\n<b>Username:</b> " + package.LoginUserName + "\n<b>Password:</b> " + package.LoginPassword + "\n";
                                                            var packageUsernamePassword = await ShowAlert(this, "Success", unlinked, true, "Close");

                                                            //Reload Dashboard
                                                            loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                                                            View.Add(loadingOverlay);

                                                            BackendProvider_iOS.UpdatePackagesFromServer();

                                                            var _infoButton = await BackendProvider_iOS.GetInfoButton();

                                                            UtilProvider.NavigationControllerWithCustomTransition(NavigationController).PushViewController(new Dashboard(_infoButton), false);
                                                        }
                                                        else
                                                        {
                                                            View.AddSubview(UtilProvider.GenericError(unlinkOperation.Message));
                                                            if (loadingOverlay != null) loadingOverlay.Hide();
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case "Cancel": { break; }
                                    }
                                });
                            });
                        };

                        (package.toggle as MDSwitch).ValueChanged += (object sender, EventArgs e) =>
                        {
                            ConnectDisconnectPackage(package);
                        };

                        listWrapper.Add(_lytListItemWrapper);

                        DragList = DragList ?? new List<UIViewPosition>();

                        if (!DragList.Where(n => n.uiView.Tag == _lytListItemWrapper.Tag).Any())
                            DragList.Add(new UIViewPosition() { uiView = _lytListItemWrapper, originalY = (float)_lytListItemWrapper.Center.Y, newY = (float)_lytListItemWrapper.Center.Y });

                        TempCenters.Add((float)_lytListItemWrapper.Center.Y);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            var listWrapperHeight = (nfloat)(90 * Index) + 90;
            listWrapperHeight = listWrapperHeight < UtilProvider.ScreenHeight - UtilProvider.ScreenHeight12th ? UtilProvider.ScreenHeight - UtilProvider.ScreenHeight12th : listWrapperHeight;
            listWrapper.Frame = new CGRect(0, 0, UtilProvider.ScreenWidth, listWrapperHeight);

            //disable package drag and drop when scrollview is scrolling
            listWrapper.DraggingStarted += delegate { _disableDragDrop = true; };
            listWrapper.DraggingEnded += delegate { _disableDragDrop = false; };

            _scrollWrapper = new UIScrollView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th * (!hotspotHelperState.Authenticated ? 2 : 1), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey),
                ContentSize = new CGSize(UtilProvider.ScreenWidth, listWrapperHeight),
            };

            //disable package drag and drop when scrollview is scrolling
            _scrollWrapper.DraggingStarted += delegate { _disableDragDrop = true; };
            _scrollWrapper.DraggingEnded += delegate { _disableDragDrop = false; };

            _scrollWrapper.Add(listWrapper);

            View.Add(_scrollWrapper);
        }

        private void ConnectDisconnectPackage(UserPackage package)
        {
            if (!allowAction)
            {
                allowAction = true;
                return;
            }

            allowAction = false;

            loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
            View.Add(loadingOverlay);

            var toggle = (MDSwitch)package.toggle;
            package = !toggle.On ? null : package;

            var Success = false;
            var Message = "";

            //1. Disconnect any connected packages
            if (hotspotHelperState.ConnectedToAlwaysOn)
            {
                if (hotspotHelperState.Authenticated)
                {
                    var packageDesc = (_Packages.Where(n => n.LoginUserName == hotspotHelperState.AuthenticatedUsername).Select(n => n.GroupName + " (" + n.LoginUserName + ")").FirstOrDefault() ?? "").ToString().Trim();

                    if (!HotspotHelper.DisconnectPackage())
                    {
                        Message = "Could not Disable Package" + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "");
                        toggle.On = false;
                        Success = false;
                    }
                    else
                    {
                        Message = "Package " + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "") + "was disabled successfully.";
                        Success = true;

                        AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageDisconnect.ToString());
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
                toggle.On = false;
                Success = false;
            }

            //2. If package parameter is not null, try and connect that package.
            if (Success && package != null)
            {
                var packageDesc = package.GroupName + " (" + package.LoginUserName + ")";

                var connectPackage = HotspotHelper.ConnectPackage(package.LoginUserName, package.LoginPassword, false);
                if (!connectPackage.Success)
                {
                    Message = "Could not Enable Package" + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "");
                    toggle.On = false;
                    Success = false;
                }
                else
                {
                    Message = "Package " + (packageDesc.Length > 0 ? " \"" + packageDesc + "\" " : "") + "was enabled successfully.";
                    toggle.On = true;
                    Success = true;

                    AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.DashboardPackageConnect.ToString());
                }
            }

            //3. If success, reload dashboard
            if (Success)
            {
                View.AddSubview(UtilProvider.SuccessMesage(Message, () =>
                {
                    ViewDidLoad();
                }));
            }
            else
            {
                View.AddSubview(UtilProvider.GenericError(Message, () =>
                {
                    allowAction = true;
                    loadingOverlay.Hide();
                }));
            }
        }

        public static Task<nint> ShowAlert(UIViewController viewController, string title, string message, bool connectionStatusAlert, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<nint>();
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            if (connectionStatusAlert)
            {
                alert.Message = "";

                var b_open = "<b>";
                var b_close = "</b>";
                var boldSpots = new List<NSRange>();
                var boldCount = (message.Length - message.Replace(b_open, "").Length) / b_open.Length;
                for (int i = 0; i < boldCount; i++)
                {
                    var startBold = message.IndexOf(b_open);
                    var endBold = message.IndexOf(b_close);
                    message = message.Remove(startBold, b_open.Length).Remove(endBold - b_close.Length + 1, b_close.Length);
                    boldSpots.Add(new NSRange(startBold, endBold - startBold - b_close.Length + 1));
                }
                var attrString = new NSMutableAttributedString(message);
                boldSpots.ForEach(nsRange => attrString.AddAttribute(UIStringAttributeKey.Font, UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 16), nsRange));

                alert.SetValueForKey(attrString, new NSString("attributedMessage"));
            }

            foreach (var button in buttons)
                alert.AddAction(UIAlertAction.Create(button, UIAlertActionStyle.Default, (a) => tcs.TrySetResult(Array.IndexOf(buttons, button))));

            viewController.PresentViewController(alert, true, null);

            return tcs.Task;
        }

        public static Task<AlertTextfieldFeedback> ShowAlertWithTextField(UIViewController viewController, string title, string message, bool passwordTextfield, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<AlertTextfieldFeedback>();
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            UITextField field = null;

            alert.AddTextField((tf) =>
            {
                //Save the field
                field = tf;

                //Initialize
                field.Placeholder = passwordTextfield ? "Password" : "";
                field.Text = "";
                field.AutocorrectionType = UITextAutocorrectionType.No;
                field.KeyboardType = UIKeyboardType.Default;
                field.ReturnKeyType = UIReturnKeyType.Done;
                field.ClearButtonMode = UITextFieldViewMode.WhileEditing;
                field.SecureTextEntry = passwordTextfield;
            });

            foreach (var button in buttons)
                alert.AddAction(UIAlertAction.Create(button, UIAlertActionStyle.Default, (a) =>
                {
                    tcs.TrySetResult(new AlertTextfieldFeedback() { ButtonIndex = Array.IndexOf(buttons, button), TextFieldText = field.Text });
                }));

            viewController.PresentViewController(alert, true, null);

            return tcs.Task;
        }
    }
}