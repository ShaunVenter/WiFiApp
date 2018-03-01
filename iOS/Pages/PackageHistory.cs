using System;
using UIKit;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using FloatLabeledEntry;
using Foundation;
using MaterialControls;
using AlwaysOn;
using AlwaysOn.Objects;
using XamSvg;
using System.Collections.Generic;
using System.Globalization;
using CoreAnimation;
using System.Timers;
using System.Linq;

namespace AlwaysOn_iOS

{
    public partial class PackageHistory : UIViewController
    {
        List<UserPackage> _Packages = new List<UserPackage>();
        LoadingOverlay loadingOverlay;
        UIView _package = new UIView();
        UIScrollView _scrollWrapper = new UIScrollView();
        UILabel _inactiveAmount = new UILabel();

        public PackageHistory(object Packages) : base("PackageHistory", null)
        {
            UtilProvider.MenuIsOpen = false;
            _Packages = (List<UserPackage>)Packages;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var navController = base.NavigationController;
            navController.NavigationBarHidden = true;

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var listWrapper = new UIView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey)
            };

            int Index = -1;

            if (_Packages != null)
            {
                foreach (var package in _Packages)
                {
                    DateTime _ExpiryDate = DateTime.Now;
                    //var Date = DateTime.TryParse (date[0], out dateTime);
                    if (!string.IsNullOrWhiteSpace(package.ExpiryDate))
                    {
                        _ExpiryDate = Convert.ToDateTime(package.ExpiryDate);
                        _ExpiryDate.Date.ToString("dd MMM yyyy");
                    }

                    //var _userPackagePresentaion = new UserPackagePresentation (package)
                    if ((package.UsageLeftvalue) == "Package Used Up" || (_ExpiryDate.Date < DateTime.Now.Date))
                    {
                        Index++;
                        package.Sanitize();

                        _package = new UIView(new CGRect(0, Index * 90, UtilProvider.ScreenWidth, 90))
                        {
                            BackgroundColor = UIColor.Clear.FromHex(0x272727),
                            LayoutMargins = new UIEdgeInsets(0, 0, 5, 0),
                            Alpha = new nfloat(0.7)
                        };

                        _inactiveAmount = new UILabel(new CGRect(35, (_package.Bounds.Height / 2) - 15, 130, 30))
                        {
                            Text = package.GroupName,// dynamic
                            TextColor = UIColor.White,

                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20)
                        };

                        UILabel depleted = new UILabel(new CGRect((_package.Bounds.Width - 80) - 40, (_package.Bounds.Height / 2) - 15, 130, 30))
                        {
                            Text = "Depleted",// dynamic
                            TextColor = UIColor.White,

                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20)
                        };

                        UILabel expired = new UILabel(new CGRect((_package.Bounds.Width - 80) - 40, (_package.Bounds.Height / 2) - 15, 130, 30))
                        {
                            Text = "Expired",// dynamic
                            TextColor = UIColor.White,

                            Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 20)
                        };

                        _package.Add(_inactiveAmount);

                        if (_ExpiryDate.Date < DateTime.Now.Date)
                        {
                            _package.Add(expired);
                        }
                        else
                        {
                            _package.Add(depleted);
                        }
                        
                        _package.Add(UtilProvider.InactiveDurationIndicator(int.Parse(package.UsagePercentValue), _package));
                        _package.Add(UtilProvider.ConnectionSpacer(_package));

                        listWrapper.Add(_package);

                    }
                }
            }

            listWrapper.Frame = new CGRect(0, 0, UtilProvider.ScreenWidth, (_package.Bounds.Height * Index) + 80);
            
            _scrollWrapper = new UIScrollView(new CGRect(0, UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey),
                ContentSize = new CGSize(listWrapper.Bounds.Width, (listWrapper.Bounds.Height + 170))
            };

            var noHistory = new UILabel(new CGRect((UtilProvider.ScreenWidth / 2) - 150, 20, 300, 50))
            {
                Text = "You Do not have any Expired/Depleted Packages",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White
            };

            if (Index < 0)
            {
                listWrapper.Add(noHistory);
            }

            var DashBoard = new MDButton(new CGRect(35, listWrapper.Bounds.Height + 20, (UtilProvider.ScreenWidth - 80) / 2, 40), MDButtonType.Flat, UIColor.White);
            DashBoard.SetTitle("DASHBOARD", UIControlState.Normal);
            DashBoard.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            DashBoard.BackgroundColor = UIColor.Clear;
            DashBoard.Layer.BorderWidth = 1;
            DashBoard.Layer.BorderColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan).CGColor;

            DashBoard.TouchUpInside += async (object sender, EventArgs e) =>
            {
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                BackendProvider_iOS.UpdatePackagesFromServer();
                var _infoButton = await BackendProvider_iOS.GetInfoButton();

                UtilProvider.NavigationControllerWithCustomTransition(NavigationController).PushViewController(new Dashboard(_infoButton), false);
            };
            
            _scrollWrapper.AddSubview(listWrapper);
            _scrollWrapper.Add(DashBoard);

            View.Add(_scrollWrapper);
            View.Add(UtilProvider.HeaderStrip(true, true, this));

        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


