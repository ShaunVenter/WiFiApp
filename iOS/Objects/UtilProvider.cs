using AlwaysOn;
using AlwaysOn.Objects;
using CoreAnimation;
using CoreGraphics;
using FloatLabeledEntry;
using Foundation;
using MaterialControls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using XamSvg;

namespace AlwaysOn_iOS.Objects
{
    public class UtilProvider
    {
        static UIView _borderBottom;
        static UIView _HeaderStrip;
        static UISvgImageView _NavLogo;
        static LoadingOverlay loadingOverlay;
        static MDButton _MenuTap = new MDButton();

        public static bool MenuIsOpen = false;
        public static nfloat ScreenHeight { get { return UIScreen.MainScreen.Bounds.Height; } }
        public static nfloat ScreenWidth { get { return UIScreen.MainScreen.Bounds.Width; } }
        public static CGRect ScreenBounds { get { return UIScreen.MainScreen.Bounds; } }
        public static nfloat ScreenHeight12th { get { return ScreenHeight / 12; } }
        public static nfloat SafeTop { get { return UIDevice.CurrentDevice.CheckSystemVersion(11, 0) ? UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Top : 0; } }
        public static nfloat SafeBottom { get { return UIDevice.CurrentDevice.CheckSystemVersion(11, 0) ? UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom : 0; } }

        public static UIView TextFieldBottomBorder(FloatLabeledTextField textField)
        {
            var yPosition = textField.Frame.Y;
            var xPosition = textField.Frame.X;

            _borderBottom = new UIView(new CGRect(xPosition, yPosition + 38, textField.Frame.Width, 2));
            _borderBottom.BackgroundColor = UIColor.White;


            return _borderBottom;
        }

        public static UIView TextFieldBottomBorderError(FloatLabeledTextField textField)
        {
            var yPosition = textField.Frame.Y;
            var xPosition = textField.Frame.X;

            _borderBottom = new UIView(new CGRect(xPosition, yPosition + 38, textField.Frame.Width, 2));
            _borderBottom.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);



            return _borderBottom;
        }

        public static UIView GenericError(string ErrorMesssage, Action Completed = null)
        {
            var errorHolder = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorErrorOrange)
            };

            var viewWrapper = new UIView(new CGRect(20, 0, UtilProvider.ScreenWidth - 40, UtilProvider.ScreenHeight12th));

            var Error = new UILabel(new CGRect(0, 0, UtilProvider.ScreenWidth - 40, UtilProvider.ScreenHeight12th))
            {
                Text = ErrorMesssage,
                TextColor = UIColor.White,
                Lines = ErrorMesssage.Length <= 94 ? 2 : 3,
                LineBreakMode = UILineBreakMode.WordWrap,

                //BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorErrorOrange),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12)
            };

            CGSize size = ((NSString)Error.Text).StringSize(Error.Font, constrainedToSize: new CGSize(Error.Frame.Width, 50f), lineBreakMode: UILineBreakMode.WordWrap);

            errorHolder.Frame = new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, size.Height + (ErrorMesssage.Length <= 94 ? 50 : 60));

            viewWrapper.Add(Error);

            errorHolder.Add(viewWrapper);

            UIView.Animate(1, 2, UIViewAnimationOptions.CurveEaseInOut, () => { errorHolder.Alpha = 0; }, Completed);

            return errorHolder;
        }

        public static UIView SuccessMesage(string Message, Action Completed = null)
        {
            var successHolder = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan)
            };

            var viewWrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 40, UtilProvider.ScreenHeight12th));

            var Error = new UILabel(new CGRect(0, (viewWrapper.Bounds.Height / 2) - 15, 230, 30))
            {
                Text = Message,
                TextColor = UIColor.White,
                Lines = 2,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12)
            };
            //viewWrapper.Add (errorImage);
            viewWrapper.Add(Error);

            successHolder.Add(viewWrapper);

            UIView.SetAnimationDuration(1);
            UIView.Animate(1, 2, UIViewAnimationOptions.CurveEaseInOut, () => { successHolder.Alpha = 0; }, Completed);

            return successHolder;
        }

        public static UIView ConnectionError(string ErrorMesssage)
        {
            var errorHolder = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorErrorOrange)
            };

            var viewWrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 40, UtilProvider.ScreenHeight12th));

            var errorImage = new UISvgImageView("svg/forbidden.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(0, (viewWrapper.Bounds.Height / 2) - 10, 20, 20)
            };

            var Error = new UILabel(new CGRect((viewWrapper.Bounds.Width - 250), (viewWrapper.Bounds.Height / 2) - 15, 230, 30))
            {
                Text = ErrorMesssage,
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 11)
            };
            viewWrapper.Add(errorImage);
            viewWrapper.Add(Error);


            errorHolder.Add(viewWrapper);



            return errorHolder;
        }

        public static UIView ButtonBottomBorder(UIButton Button)
        {
            var xPosition = Button.Frame.Y;

            _borderBottom = new UIView(new CGRect(35, xPosition + 30, Button.Frame.Width - 30, 1));
            _borderBottom.BackgroundColor = UIColor.White;

            return _borderBottom;
        }

        public static UISvgImageView OrangeExpiryWarning()
        {

            var warning = new UISvgImageView("svg/orange_warning.svg")
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            return warning;
        }

        public static UIView InactiveDurationIndicator(int Percentage, UIView Wrapper)
        {
            UIColor barColor = null;
            if (Percentage >= 75)
                barColor = UIColor.Clear.FromHex(0x905fb8); //Purple
            else if (Percentage >= 50)
                barColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan); //Cyan 0x4be1e3
            else if (Percentage >= 25)
                barColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorYellow); //Yellow 0xfbff80
            else
                barColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorErrorOrange); //Red 0xfc9674

            var bar = new UIView(new CGRect(0, Wrapper.Bounds.Height - 10, 0, 2))
            {
                BackgroundColor = barColor
            };

            var percentValue = ((nfloat)Percentage / 100) * (UtilProvider.ScreenWidth * 2);

            UIView.Animate(1, 0, UIViewAnimationOptions.CurveEaseInOut, () => { bar.Bounds = new CGRect(0, Wrapper.Bounds.Height - 10, percentValue, 2); }, () => { });

            return bar;
        }

        public static MDButton activeDurationIndicatorButton(int Percentage, UIView Wrapper)
        {
            var rippleColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorErrorOrange);
            if (Percentage >= 75)
            {
                rippleColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            }
            else if (Percentage >= 50)
            {
                rippleColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            }
            else if (Percentage >= 25)
            {
                rippleColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorYellow);
            }

            var percentValue = ((nfloat)Percentage / 100) * (UtilProvider.ScreenWidth * 2);
            var bar = new MDButton(new CGRect(0, Wrapper.Bounds.Height - 10, percentValue, Wrapper.Bounds.Height), MDButtonType.Flat, rippleColor);

            return bar;
        }

        public static UIView Menu(UIViewController activeView)
        {
            var menuWrapper = new UIView(new CGRect(UtilProvider.ScreenWidth, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorGrey)
            };

            var MenuStrip = new UIView(new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th));
            MenuStrip.BackgroundColor = UIColor.White;

            _NavLogo = new UISvgImageView("svg/AlwaysOn.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 40, (UtilProvider.ScreenHeight12th / 2) - 20, 80, 40),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            var closeBtn = new UISvgImageView("svg/close.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - 7.5, 30, 15),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            MDButton MenuTap = new MDButton(new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - (_HeaderStrip.Bounds.Height / 2), 40, _HeaderStrip.Bounds.Height), MDButtonType.FloatingAction, UIColor.Clear.FromHex(ConfigurationProvider.AppColorButtonTouch));
            MenuTap.BackgroundColor = UIColor.Clear;
            MenuTap.TouchUpInside += (object sender, EventArgs e) =>
            {
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut,
                    () =>
                    {
                        menuWrapper.Frame = new CGRect(UtilProvider.ScreenWidth, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight);
                    },
                    () =>
                    {
                        AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuClose.ToString());
                        MenuIsOpen = false;
                    }
                );
            };

            MenuStrip.Add(closeBtn);
            MenuStrip.Add(MenuTap);
            MenuStrip.Add(_NavLogo);

            var userDetailsWrapper = new UIView(new CGRect(0, MenuItemPosition(1), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey)
            };

            var signedInLabel = new UILabel(new CGRect(40, (UtilProvider.ScreenHeight12th / 2) - 17, 150, 20))
            {
                Text = "SIGNED IN AS",
                TextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorTextLightGrey),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 12)
            };

            var userName = new UILabel(new CGRect(40, (UtilProvider.ScreenHeight12th / 2), 150, 20))
            {
                Text = BackendProvider_iOS.GetUser.Name + " " + BackendProvider_iOS.GetUser.Surname,
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14)
            };
            var LogoutBtn = new MDButton(new CGRect(UtilProvider.ScreenWidth - 105, (UtilProvider.ScreenHeight12th / 2) - 15, 75, 30), MDButtonType.Raised, UIColor.White);
            LogoutBtn.SetTitle("SIGN OUT", UIControlState.Normal);
            LogoutBtn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            LogoutBtn.BackgroundColor = UIColor.Clear;
            LogoutBtn.SetTitleColor(UIColor.Clear.FromHex(ConfigurationProvider.AppColorTextGrey), UIControlState.Normal);
            LogoutBtn.Layer.BorderWidth = 1;
            LogoutBtn.Layer.BorderColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorTextGrey).CGColor;

            LogoutBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuSignout.ToString());

                BackendProvider_iOS.ClearSettings();
                activeView.NavigationController.PushViewController(new LoginScreen(false), true);
            };
            userDetailsWrapper.Add(signedInLabel);
            userDetailsWrapper.Add(userName);
            userDetailsWrapper.Add(LogoutBtn);


            // Links

            var DashBoardBtn = new MDButton(new CGRect(0, MenuItemPosition(2), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            DashBoardBtn.SetTitle("Dashboard", UIControlState.Normal);
            DashBoardBtn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            DashBoardBtn.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            DashBoardBtn.SetTitleColor(UIColor.White, UIControlState.Normal);
            DashBoardBtn.TouchUpInside += async (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuDashboard.ToString());

                await LoadDashBoard(activeView);
            };

            var RedeemVoucher = new MDButton(new CGRect(0, MenuItemPosition(3), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            RedeemVoucher.SetTitle("Link a Package", UIControlState.Normal);
            RedeemVoucher.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            RedeemVoucher.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            RedeemVoucher.SetTitleColor(UIColor.White, UIControlState.Normal);
            RedeemVoucher.TouchUpInside += (object sender, EventArgs e) =>
            {
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                activeView.Add(loadingOverlay);

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuLinkPackage.ToString());

                BackendProvider_iOS.GetServiceProviders();

                activeView.NavigationController.PushViewController(new RedeemVoucherPage(), true);
            };

            var RedeemVoucherSingleCode = new MDButton(new CGRect(0, MenuItemPosition(3), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            RedeemVoucherSingleCode.SetTitle("Link access code", UIControlState.Normal);
            RedeemVoucherSingleCode.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            RedeemVoucherSingleCode.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            RedeemVoucherSingleCode.SetTitleColor(UIColor.White, UIControlState.Normal);
            RedeemVoucherSingleCode.TouchUpInside += (object sender, EventArgs e) =>
            {
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                activeView.Add(loadingOverlay);

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuLinkAccessPackage.ToString());

                BackendProvider_iOS.GetServiceProviders();

                activeView.NavigationController.PushViewController(new RedeemVoucherSingleCode(), true);
            };

            var HotSpotFinder = new MDButton(new CGRect(0, MenuItemPosition(4), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            HotSpotFinder.SetTitle("Hotspot finder", UIControlState.Normal);
            HotSpotFinder.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            HotSpotFinder.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            HotSpotFinder.SetTitleColor(UIColor.White, UIControlState.Normal);
            HotSpotFinder.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuHotspotFinder.ToString());

                activeView.NavigationController.PushViewController(new HotspotTabs(), true);
            };

            var ProfileDetails = new MDButton(new CGRect(0, MenuItemPosition(5), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            ProfileDetails.SetTitle("Profile Details", UIControlState.Normal);
            ProfileDetails.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            ProfileDetails.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            ProfileDetails.SetTitleColor(UIColor.White, UIControlState.Normal);
            ProfileDetails.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuProfileDetails.ToString());

                activeView.NavigationController.PushViewController(new ProfileDetailsPage(), true);
            };

            var Settings = new MDButton(new CGRect(0, MenuItemPosition(6), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            Settings.SetTitle("Settings", UIControlState.Normal);
            Settings.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            Settings.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            Settings.SetTitleColor(UIColor.White, UIControlState.Normal);
            Settings.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuSettings.ToString());

                activeView.NavigationController.PushViewController(new SettingsPage(), true);
            };

            var Connect = new MDButton(new CGRect(0, MenuItemPosition(7), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            Connect.SetTitle("How to connect", UIControlState.Normal);
            Connect.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            Connect.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            Connect.SetTitleColor(UIColor.White, UIControlState.Normal);
            Connect.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuHowToConnect.ToString());

                activeView.NavigationController.PushViewController(new HowToConnectCarousel(), true);
            };

            var TechSupport = new MDButton(new CGRect(0, MenuItemPosition(8), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            TechSupport.SetTitle("Technical Support", UIControlState.Normal);
            TechSupport.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            TechSupport.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            TechSupport.SetTitleColor(UIColor.White, UIControlState.Normal);
            TechSupport.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuTechnicalSupport.ToString());

                activeView.NavigationController.PushViewController(new TechnicalSupportScreen(), true);
            };

            var Terms = new MDButton(new CGRect(0, MenuItemPosition(9), UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th), MDButtonType.Flat, UIColor.White);
            Terms.SetTitle("Terms & Conditions", UIControlState.Normal);
            Terms.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 20f);
            Terms.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);
            Terms.SetTitleColor(UIColor.White, UIControlState.Normal);
            Terms.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.PageViewGA(PageName.TermsAndConditions.ToString());
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuTermsConditions.ToString());

                var url = "https://www.alwayson.co.za/terms.aspx";
                UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
            };

            menuWrapper.Add(userDetailsWrapper);

            menuWrapper.Add(DashBoardBtn);
            menuWrapper.Add(RedeemVoucher);
            menuWrapper.Add(HotSpotFinder);
            menuWrapper.Add(ProfileDetails);
            menuWrapper.Add(Settings);
            menuWrapper.Add(Connect);
            menuWrapper.Add(TechSupport);
            menuWrapper.Add(Terms);

            menuWrapper.Add(MenuStrip);

            UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () => { menuWrapper.Frame = new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight); }, () => { });

            return menuWrapper;
        }

        public static nfloat MenuItemPosition(float item)
        {
            return (UtilProvider.SafeTop + (item * 5)) + UtilProvider.ScreenHeight12th * item;
        }

        public static nfloat ItemPosition(float item)
        {
            try
            {
                if (item == 0) return 5;

                return (item * 5) + (UtilProvider.ScreenHeight12th * item);
            }
            catch
            {
                return 0;
            }
        }

        public static UIView HeaderStrip(bool BackButton, bool MenuButton, UIViewController activeView)
        {
            _HeaderStrip = new UIView(new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.White
            };
            _NavLogo = new UISvgImageView("svg/AlwaysOn.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 40, (UtilProvider.ScreenHeight12th / 2) - 20, 80, 40),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            _HeaderStrip.Add(_NavLogo);

            if (BackButton)
            {
                var backBtn = new UISvgImageView("svg/backBtn.svg")
                {
                    Frame = new CGRect(25, (UtilProvider.ScreenHeight12th / 2) - 7.5, 30, 15),
                    ContentMode = UIViewContentMode.ScaleAspectFit
                };
                MDButton backTap = new MDButton(new CGRect(25, (UtilProvider.ScreenHeight12th / 2) - (_HeaderStrip.Bounds.Height / 2), 40, _HeaderStrip.Bounds.Height), MDButtonType.FloatingAction, UIColor.Clear.FromHex(ConfigurationProvider.AppColorButtonTouch));
                backTap.BackgroundColor = UIColor.Clear;
                backTap.TouchUpInside += async (object sender, EventArgs e) =>
                {
                    switch (activeView.NibName)
                    {
                        case "Dashboard": break;
                        case "RegistrationScreen":
                        case "ResetPasswordPage": activeView.NavigationController.PushViewController(new LoginScreen(false), true); break;
                        case "ProfileDetails":
                            {
                                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsBack.ToString());
                                await LoadDashBoard(activeView);
                                break;
                            }
                        case "SettingsPage":
                            {
                                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.SettingsBack.ToString());
                                await LoadDashBoard(activeView);
                                break;
                            }
                        case "UpdateUserProfile":
                            {
                                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsEditBack.ToString());
                                activeView.NavigationController.PushViewController(new ProfileDetailsPage(), true);
                                break;
                            }
                        case "TechnicalSupportScreen": await LoadDashBoard(activeView); break;
                        case "HowToConnectCarousel": await LoadDashBoard(activeView); break;
                        case "RedeemVoucherPage": await LoadDashBoard(activeView); break;
                        case "HotspotTabs":
                            {
                                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderListBack.ToString());
                                await LoadDashBoard(activeView);
                                break;
                            }
                        case "HotspotsMap":
                            {
                                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.HotspotFinderMapBack.ToString());
                                activeView.NavigationController.PushViewController(new HotspotTabs(), true);
                                break;
                            }
                        case "PackageHistory": await LoadDashBoard(activeView); break;
                        default: await LoadDashBoard(activeView); break;
                    }
                };

                _HeaderStrip.Add(backBtn);
                _HeaderStrip.Add(backTap);
            }

            if (MenuButton)
            {
                var menuBtn = new UISvgImageView("svg/menuBtn.svg")
                {
                    Frame = new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - 12.5, 40, 25),
                    ContentMode = UIViewContentMode.ScaleAspectFit
                };
                _MenuTap = new MDButton(new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - (_HeaderStrip.Bounds.Height / 2), 70, _HeaderStrip.Bounds.Height), MDButtonType.FloatingAction, UIColor.Clear.FromHex(ConfigurationProvider.AppColorButtonTouch));
                _MenuTap.BackgroundColor = UIColor.Clear;
                _MenuTap.TouchUpInside += (object sender, EventArgs e) =>
                {
                    if (!MenuIsOpen)
                    {
                        activeView.View.Add(Menu(activeView));

                        MenuIsOpen = true;
                    }
                };

                _HeaderStrip.Add(menuBtn);
                _HeaderStrip.Add(_MenuTap);
            }

            return _HeaderStrip;
        }

        public static async Task LoadDashBoard(UIViewController activeView)
        {
            loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
            activeView.Add(loadingOverlay);

            BackendProvider_iOS.UpdatePackagesFromServer();
            var _infoButton = await BackendProvider_iOS.GetInfoButton();

            NavigationControllerWithCustomTransition(activeView.NavigationController).PushViewController(new Dashboard(_infoButton), false);
        }

        public static UIView LoadInfoWebView(UIViewController activeView, string url)
        {
            var WebViewParent = new UIView(new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                BackgroundColor = UIColor.White
            };

            var MenuStrip = new UIView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight12th))
            {
                BackgroundColor = UIColor.White
            };

            var webView = new UIWebView(new CGRect(0, UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                ScalesPageToFit = true
            };
            webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
            webView.LoadStarted += delegate
            {
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                webView.Add(loadingOverlay);
                webView.BringSubviewToFront(MenuStrip);
            };
            webView.LoadFinished += delegate
            {
                loadingOverlay.Hide();
            };
            webView.LoadError += async delegate
            {
                await LoadDashBoard(activeView);
            };

            _NavLogo = new UISvgImageView("svg/AlwaysOn.svg")
            {
                Frame = new CGRect((UtilProvider.ScreenWidth / 2) - 40, (UtilProvider.ScreenHeight12th / 2) - 20, 80, 40),
                ContentMode = UIViewContentMode.ScaleAspectFit,
            };

            var backBtn = new UISvgImageView("svg/backBtn.svg")
            {
                Frame = new CGRect(25, (UtilProvider.ScreenHeight12th / 2) - 7.5, 30, 15),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            MDButton backTap = new MDButton(new CGRect(25, (UtilProvider.ScreenHeight12th / 2) - (_HeaderStrip.Bounds.Height / 2), 40, _HeaderStrip.Bounds.Height), MDButtonType.FloatingAction, UIColor.Clear.FromHex(ConfigurationProvider.AppColorButtonTouch));
            backTap.TouchUpInside += async (object sender, EventArgs e) =>
            {
                if (webView.CanGoBack)
                {
                    webView.GoBack();
                }
                else
                {
                    await LoadDashBoard(activeView);
                }
            };
            backTap.BackgroundColor = UIColor.Clear;

            var closeBtn = new UISvgImageView("svg/close.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - 7.5, 30, 15),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            var menuBtn = new UISvgImageView("svg/menuBtn.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - 12.5, 40, 25),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            _MenuTap = new MDButton(new CGRect(UtilProvider.ScreenWidth - 45, (UtilProvider.ScreenHeight12th / 2) - (_HeaderStrip.Bounds.Height / 2), 70, _HeaderStrip.Bounds.Height), MDButtonType.FloatingAction, UIColor.Clear.FromHex(ConfigurationProvider.AppColorButtonTouch));
            _MenuTap.BackgroundColor = UIColor.Clear;

            _MenuTap.TouchUpInside += (object sender, EventArgs e) =>
            {
                if (!MenuIsOpen)
                {
                    activeView.Add(Menu(activeView));

                    MenuIsOpen = true;
                }
            };

            MenuStrip.Add(menuBtn);
            MenuStrip.Add(_MenuTap);
            MenuStrip.Add(_NavLogo);
            MenuStrip.Add(backBtn);
            MenuStrip.Add(backTap);

            //webView.Add(MenuStrip);

            WebViewParent.Add(MenuStrip);
            WebViewParent.Add(webView);

            UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () => { WebViewParent.Frame = new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight); }, () => { });

            return WebViewParent;
        }

        public static UIView ConnectionSpacer(UIView Wrapper)
        {
            UIView spacer = new UIView(new CGRect(0, Wrapper.Bounds.Height - 8, Wrapper.Bounds.Width, 10))
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey)
            };

            return spacer;
        }

        public static MDButton FloatingBtn(string Title)
        {
            MDButton Btn = new MDButton(new CGRect(UtilProvider.ScreenWidth - 100, UtilProvider.ScreenHeight - 150, 55, 55), MDButtonType.FloatingAction, UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 11),
                BackgroundColor = UIColor.White
            };
            Btn.SetTitle(Title, UIControlState.Normal);
            Btn.SetTitleColor(UIColor.Clear.FromHex(ConfigurationProvider.AppColorDataTabPurple), UIControlState.Normal);
            Btn.SetTitleColor(UIColor.White, UIControlState.Selected);

            var lines = Title.Length - Title.Replace("\r\n", "").Length;
            Btn.TitleLabel.Lines = lines < 1 ? 1 : lines;
            Btn.TitleLabel.TextAlignment = UITextAlignment.Center;

            return Btn;
        }

        public static UISvgImageView WifiSettingsImages(UIView wrapper)
        {
            var settings = new UISvgImageView("svg/settingsIcon.svg")
            {
                Frame = new CGRect(UtilProvider.ScreenWidth - 100, (wrapper.Bounds.Height / 2) - 12.5, 65, 25),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            return settings;
        }

        public static UISvgImageView WifiDivider(UIView wrapper)
        {
            var line = new UISvgImageView("svg/wifi-divider.svg")
            {
                Frame = new CGRect(40, (wrapper.Bounds.Height - 2), wrapper.Bounds.Width - 80, 2),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            return line;
        }

        public static UINavigationController NavigationControllerWithCustomTransition(UINavigationController Controller)
        {
            var transition = new CATransition()
            {
                Duration = 0.3,
                TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn),
                Type = CAAnimation.TransitionFade,
                Subtype = CAAnimation.TransitionFade//,
                //FillMode = CAFillMode.Both,
                //FadeInDuration = new nfloat(0.5),
                //FadeOutDuration = new nfloat(0.5)
            };
            Controller.View.Layer.AddAnimation(transition, null);
            return Controller;
        }

        public enum FontShape { Regular = 0, LightItalic = 1, Light = 2, Italic = 3, BoldItalic = 4, Bold = 5 }
        public static UIFont GetAppFont(FontShape shape, nfloat size)
        {
            /*
            var fontlist = new System.Collections.Generic.List<string>();
            foreach (string sName in UIFont.FamilyNames)
            {
                foreach (string sFontName in UIFont.FontNamesForFamilyName(sName))
                {
                    fontlist.Add(string.Format("FamilyName:\t\t\t{0} \t\t\tFontName:\t\t\t\t{1}", sName, sFontName));
                }
            }
            */
            switch (shape)
            {
                default:
                case FontShape.Regular: return UIFont.FromName(ConfigurationProvider.FocoRegular, size) ?? UIFont.FromName("HelveticaNeue", size);
                case FontShape.LightItalic: return UIFont.FromName(ConfigurationProvider.FocoLightItalic, size) ?? UIFont.FromName("HelveticaNeue-LightItalic", size);
                case FontShape.Light: return UIFont.FromName(ConfigurationProvider.FocoLight, size) ?? UIFont.FromName("HelveticaNeue-Light", size);
                case FontShape.Italic: return UIFont.FromName(ConfigurationProvider.FocoItalic, size) ?? UIFont.FromName("HelveticaNeue-Italic", size);
                case FontShape.BoldItalic: return UIFont.FromName(ConfigurationProvider.FocoBoldItalic, size) ?? UIFont.FromName("HelveticaNeue-BoldItalic", size);
                case FontShape.Bold: return UIFont.FromName(ConfigurationProvider.FocoBold, size) ?? UIFont.FromName("HelveticaNeue-Bold", size);
            }
        }
    }

    public class HotspotHelperWebClient : System.Net.WebClient
    {
        public int Timeout { get; set; } = 3000;

        protected override System.Net.WebRequest GetWebRequest(System.Uri uri)
        {
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = Timeout;
            return w;
        }
    }

    public class TimeoutWebClient : System.Net.WebClient
    {
        public int Timeout { get; set; } = 1000;
        public System.Net.HttpStatusCode? ResponseStatusCode { get; set; }

        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            var w = (System.Net.HttpWebRequest)base.GetWebRequest(uri);
            w.AllowAutoRedirect = false;
            w.Timeout = Timeout;
            w.Host = uri.Host;
            w.KeepAlive = false;

            return w;
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request)
        {
            try
            {
                System.Net.WebResponse resp = base.GetWebResponse(request);
                ResponseStatusCode = (resp is System.Net.HttpWebResponse) ? ((System.Net.HttpWebResponse)resp).StatusCode : (System.Net.HttpStatusCode?)null;
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override System.Net.WebResponse GetWebResponse(System.Net.WebRequest request, IAsyncResult result)
        {
            try
            {
                System.Net.WebResponse resp = base.GetWebResponse(request, result);
                ResponseStatusCode = (resp is System.Net.HttpWebResponse) ? ((System.Net.HttpWebResponse)resp).StatusCode : (System.Net.HttpStatusCode?)null;
                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

