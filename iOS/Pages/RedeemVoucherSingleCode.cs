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
    public partial class RedeemVoucherSingleCode : UIViewController
    {
        LoadingOverlay loadingOverlay;

        public RedeemVoucherSingleCode() : base("RedeemVoucherSingleCode", null)
        {
            UtilProvider.MenuIsOpen = false;
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
                Text = "Link your Prepaid Pin below",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14)
            };

            var UserName = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(2), RedeemWrapper.Bounds.Width, 35))
            {
                Placeholder = "Prepaid Pin",
                AttributedPlaceholder = new NSAttributedString("0000000000", null, UIColor.White),
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
                    UserName.AttributedPlaceholder = new NSAttributedString("Prepaid Pin", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    UserName.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));
                    UserName.AttributedPlaceholder = new NSAttributedString("Prepaid Pin", null, UIColor.White);
                    UserName.FloatingLabelActiveTextColor = UIColor.White;
                }
            };
            UserName.ShouldReturn += (textField) =>
            {
                UserName.ResignFirstResponder();
                return true;
            };


           MDButton RedeemPackage = new MDButton(new CGRect(RedeemWrapper.Bounds.Width - (RedeemWrapper.Bounds.Width / 2), (UtilProvider.ScreenHeight) - 138 - UtilProvider.SafeBottom - UtilProvider.SafeTop, RedeemWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            RedeemPackage.SetTitle("LINK PIN", UIControlState.Normal);
            RedeemPackage.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 12.5f);
            RedeemPackage.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            RedeemPackage.TouchUpInside += async (object sender, EventArgs e) =>
            {
                // hide the keyboard
                UserName.ResignFirstResponder();
                UserName.ResignFirstResponder();

                var username = string.IsNullOrWhiteSpace(UserName.Text) ? string.Empty : UserName.Text.Trim();

                if (username.Length <= 0)
                {
                    RedeemWrapper.Add(UtilProvider.TextFieldBottomBorderError(UserName));
                    UserName.AttributedPlaceholder = new NSAttributedString("Prepaid Pin", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    UserName.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                    return;
                }

                UserName.AttributedPlaceholder = new NSAttributedString("Prepaid Pin", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                UserName.FloatingLabelActiveTextColor = UIColor.White;
                RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));



                RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));

                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                Operation operation = await BackendProvider.LinkAccessCode(AppDelegate.ApiKey, BackendProvider_iOS.GetUser.UserId, username);

                if (operation.Result == OperationResult.Failure)
                {
                    loadingOverlay.Hide();
                    View.Add(UtilProvider.GenericError(operation.Message));

                    return;
                }

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkAccessPackageLink.ToString());

                View.Add(UtilProvider.SuccessMesage("Prepaid Pin has been linked successfully"));
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
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkAccessPackageCancel.ToString());

                await UtilProvider.LoadDashBoard(this);//NavigationController.PushViewController(new LoginScreen(false), true);
            };

            RedeemWrapper.Add(instruction);
            //RedeemWrapper.Add (info);
            RedeemWrapper.Add(UserName);
            RedeemWrapper.Add(UtilProvider.TextFieldBottomBorder(UserName));
            RedeemWrapper.Add(RedeemPackage);
            RedeemWrapper.Add(Cancel);

            View.Add(UtilProvider.HeaderStrip(true, true, this));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}