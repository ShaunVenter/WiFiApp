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
using System.Threading;
using System.Timers;
using Valore.IOSDropDown;
using System.Threading.Tasks;
using TPKeyboardAvoiding;



namespace AlwaysOn_iOS
{
    public partial class RedeemVoucherPage : UIViewController
    {
        LoadingOverlay loadingOverlay;

        public List<ServiceProvider> _serviceProvider;

        public RedeemVoucherPage() : base("RedeemVoucherPage", null)
        {
            UtilProvider.MenuIsOpen = false;

            var sp = BackendProvider_iOS.GetStoredServiceProviders();
            if (sp.Result == OperationResult.Success && ((ServiceProviders)sp.Response).Items?.Count > 0)
            {
                _serviceProvider = ((ServiceProviders)sp.Response).Items;
            }
            else
            {
                _serviceProvider.Add(new ServiceProvider("1", "AlwaysOn") { });
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AnalyticsProvider_iOS.PageViewGA(PageName.LinkPackage.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var RedeemWrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight - UtilProvider.ScreenHeight12th));

            var PickerListWrapper = new UIView(new CGRect(0, 0, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight));
            var _PickerScrollView = new UIScrollView(new CGRect(0, -UtilProvider.ScreenHeight, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight))
            {
                ContentSize = new CGSize(UtilProvider.ScreenWidth, PickerListWrapper.Bounds.Height + 250)
            };
            _PickerScrollView.Add(PickerListWrapper);

            var instruction = new UILabel(new CGRect(0, 40, UtilProvider.ScreenWidth, 30))
            {
                Text = "Link your purchased package below",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14)
            };

            var UserName = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(2), RedeemWrapper.Bounds.Width, 35))
            {
                Placeholder = "Username",
                AttributedPlaceholder = new NSAttributedString("Username", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 14),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                AutocapitalizationType = UITextAutocapitalizationType.None
            };
            UserName.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorderError(UserName));
                    UserName.AttributedPlaceholder = new NSAttributedString("Username", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    UserName.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));
                    UserName.AttributedPlaceholder = new NSAttributedString("Username", null, UIColor.White);
                    UserName.FloatingLabelActiveTextColor = UIColor.White;
                }
            };
            UserName.ShouldReturn += (textField) =>
            {
                UserName.ResignFirstResponder();
                return true;
            };

            var Password = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(3), RedeemWrapper.Bounds.Width, 35))
            {
                Placeholder = "Password",
                AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 14),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                KeyboardType = UIKeyboardType.EmailAddress,
                AutocapitalizationType = UITextAutocapitalizationType.None

            };
            Password.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0 || !BackendProvider_iOS.IsValidEmail(((UITextField)sender).Text))
                {
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password)); ;
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    Password.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.White);
                    Password.FloatingLabelActiveTextColor = UIColor.White;
                }
            };
            Password.ShouldReturn += (textField) =>
            {
                Password.ResignFirstResponder();
                return true;
            };

            var ServiceProvider = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(4), RedeemWrapper.Bounds.Width, 35))
            {
                Placeholder = "Service Provider",
                AttributedPlaceholder = new NSAttributedString("Service Provider", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 14),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                Enabled = false,
                Text = "AlwaysOn"
            };
            var ServiceProviderTap = new MDButton(new CGRect(0, UtilProvider.ItemPosition(4), RedeemWrapper.Bounds.Width, 35), MDButtonType.Flat, UIColor.White)
            {
                BackgroundColor = UIColor.Clear
            };
            
            MDButton RedeemPackage = new MDButton(new CGRect(RedeemWrapper.Bounds.Width - (RedeemWrapper.Bounds.Width / 2), (UtilProvider.ScreenHeight) - 138 - UtilProvider.SafeBottom - UtilProvider.SafeTop, RedeemWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            RedeemPackage.SetTitle("LINK PACKAGE", UIControlState.Normal);
            RedeemPackage.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 12.5f);
            RedeemPackage.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            RedeemPackage.TouchUpInside += async (object sender, EventArgs e) =>
            {
                // hide the keyboard
                UserName.ResignFirstResponder();
                UserName.ResignFirstResponder();

                var username = string.IsNullOrWhiteSpace(UserName.Text) ? string.Empty : UserName.Text.Trim();
                var password = string.IsNullOrWhiteSpace(Password.Text) ? string.Empty : Password.Text.Trim();
                
                if (username.Length <= 0)
                {
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorderError(UserName));
                    UserName.AttributedPlaceholder = new NSAttributedString("Username", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    UserName.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                    return;
                }

                UserName.AttributedPlaceholder = new NSAttributedString("Username", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                UserName.FloatingLabelActiveTextColor = UIColor.White;
                RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));

                if (password.Length <= 0)
                {
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }


                RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));
                UserName.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                Operation operation = await BackendProvider.LinkPurchasedPackage(AppDelegate.ApiKey, BackendProvider_iOS.GetUser.UserId, username, password, ServiceProvider.Tag.ToString());
                if (operation.Result == OperationResult.Failure)
                {
                    loadingOverlay.Hide();
                    View.Add(UtilProvider.GenericError(operation.Message));

                    return;
                }

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkPackageLink.ToString());

                View.Add(UtilProvider.SuccessMesage("Package has been linked successfully"));
                //loadingOverlay.Hide();
                await Task.Delay(2000);
                await UtilProvider.LoadDashBoard(this);
            };

            MDButton Cancel = new MDButton(new CGRect(0, (UtilProvider.ScreenHeight) - 138 - UtilProvider.SafeBottom - UtilProvider.SafeTop, RedeemWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            Cancel.SetTitle("CANCEL", UIControlState.Normal);
            Cancel.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            Cancel.BackgroundColor = UIColor.Clear;
            Cancel.Layer.BorderWidth = 1;
            Cancel.Layer.BorderColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan).CGColor;
            Cancel.TouchUpInside += async (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkPackageCancel.ToString());

                await UtilProvider.LoadDashBoard(this);//NavigationController.PushViewController(new LoginScreen(false), true);
            };
            
            RedeemWrapper.Add(instruction);
            //RedeemWrapper.Add (info);
            RedeemWrapper.Add(UserName);
            RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));
            RedeemWrapper.Add(Password);
            RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
            RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(ServiceProvider));
            RedeemWrapper.Add(ServiceProvider);
            RedeemWrapper.Add(ServiceProviderTap);
            RedeemWrapper.Add(RedeemPackage);
            RedeemWrapper.Add(Cancel);
            
            var scroll = new TPKeyboardAvoidingScrollView
            {
                Frame = new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight - UtilProvider.SafeTop - UtilProvider.ScreenHeight12th)
            };
            scroll.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            scroll.Add(RedeemWrapper);
            scroll.Add(CreateDropDownList(ServiceProviderTap, PickerListWrapper, ServiceProvider, _PickerScrollView));
            scroll.Add(_PickerScrollView);

            View.Add(scroll);
            View.Add(UtilProvider.HeaderStrip(true, true, this));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private DropDownList CreateDropDownList(MDButton toggleList, UIView RedeemWrapper, FloatLabeledTextField serviceProvider, UIScrollView scrollWrapper)
        {
            var list = new List<DropDownListItem>();

            int Index = 1;
            foreach (var item in _serviceProvider)
            {

                Index++;
                list.Add(new DropDownListItem()
                {
                    Id = item.Id,
                    DisplayText = item.Name,
                    IsSelected = item.Name == "AlwaysOn" ? true : false,

                });
            }
            scrollWrapper.ContentSize = new CGSize(UtilProvider.ScreenWidth, (Index * 45));
            RedeemWrapper.Frame = new CGRect(0, 0, UtilProvider.ScreenWidth, (Index * 45));
            var ddl = new DropDownList(RedeemWrapper, list.ToArray())
            {
                BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkPurple),
                TextColor = UIColor.White,
                Opacity = 0.95f,
                TintColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan),
                ImageColor = UIColor.Blue,
                BorderColor = UIColor.Clear.CGColor
            };

            ddl.DropDownListChanged += (e, a) =>
            {
                //var index = e; // e is the index selected
                serviceProvider.Text = a.DisplayText;
                serviceProvider.Tag = Convert.ToInt32(string.IsNullOrEmpty(a.Id) ? "1" : a.Id.ToString().Trim());

                UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    ddl.Toggle();
                }, () =>
                {
                    scrollWrapper.Frame = new CGRect(0, -UtilProvider.ScreenHeight, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight);
                });
            };

            toggleList.TouchUpInside += (object sender, EventArgs e) =>
            {
                UIView.Animate(0.2, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    scrollWrapper.Frame = new CGRect(0, UtilProvider.SafeTop, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight);
                }, () =>
                {
                    ddl.Toggle();
                });
            };

            return ddl;
        }
    }
}