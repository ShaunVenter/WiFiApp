using System;
using UIKit;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using MaterialControls;
using FloatLabeledEntry;
using Foundation;
using AlwaysOn;
using AlwaysOn.Objects;
using System.Threading.Tasks;
using TPKeyboardAvoiding;

namespace AlwaysOn_iOS
{
    public partial class ResetPasswordPage : UIViewController
    {
        LoadingOverlay loadingOverlay;

        public ResetPasswordPage() : base("ResetPasswordPage", null)
        {
            UtilProvider.MenuIsOpen = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            
            AnalyticsProvider_iOS.PageViewGA(PageName.ForgotPassword.ToString());

            var navController = base.NavigationController;
            navController.NavigationBarHidden = true;
            
            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var ResetWrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight - UtilProvider.ScreenHeight12th - UtilProvider.SafeTop));

            var instruction = new UILabel(new CGRect(0, 10, 150, 30))
            {
                Text = "Forgot your Password?",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 14)
            };
            
            var info = new UILabel(new CGRect(0, 40, ResetWrapper.Bounds.Width, 80))
            {
                Text = "No problem. Just fill in the form below and we’ll send you a link which you can use to reset your password.",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                Lines = 3
            };

            var PhoneNumber = new FloatLabeledTextField(new CGRect(0, (UtilProvider.ScreenHeight / 4) - 5, ResetWrapper.Bounds.Width, 35))
            {
                Placeholder = "Phone number",
                AttributedPlaceholder = new NSAttributedString("Phone number", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 14),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                KeyboardType = UIKeyboardType.NumberPad,
            };
            PhoneNumber.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorderError(PhoneNumber));
                    PhoneNumber.AttributedPlaceholder = new NSAttributedString("Phone number", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    PhoneNumber.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorder(PhoneNumber));
                    PhoneNumber.AttributedPlaceholder = new NSAttributedString("Phone number", null, UIColor.White);
                    PhoneNumber.FloatingLabelActiveTextColor = UIColor.White;
                }
            };
            PhoneNumber.ShouldReturn += (textField) =>
            {
                PhoneNumber.ResignFirstResponder();
                return true;
            };

            var Email = new FloatLabeledTextField(new CGRect(0, (UtilProvider.ScreenHeight / 3) + 5, ResetWrapper.Bounds.Width, 35))
            {
                Placeholder = "Email address",
                AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 14),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                KeyboardType = UIKeyboardType.EmailAddress,
                AutocapitalizationType = UITextAutocapitalizationType.None
            };
            Email.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0 || !BackendProvider_iOS.IsValidEmail(((UITextField)sender).Text))
                {
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email)); ;
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    Email.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.White);
                    Email.FloatingLabelActiveTextColor = UIColor.White;
                }
            };
            Email.ShouldReturn += (textField) =>
            {
                Email.ResignFirstResponder();
                return true;
            };

            var Reset = new MDButton(new CGRect(ResetWrapper.Bounds.Width - (ResetWrapper.Bounds.Width / 2), ResetWrapper.Bounds.Height - 135, ResetWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            Reset.SetTitle("RECOVER NOW", UIControlState.Normal);
            Reset.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            Reset.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            Reset.TouchUpInside += async (object sender, EventArgs e) =>
            {
                // hide the keyboard
                Email.ResignFirstResponder();
                PhoneNumber.ResignFirstResponder();

                var email = string.IsNullOrWhiteSpace(Email.Text) ? string.Empty : Email.Text.Trim();
                var phoneNumber = string.IsNullOrWhiteSpace(PhoneNumber.Text) ? string.Empty : PhoneNumber.Text.Trim();


                if (email.Length <= 0)
                {
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    Email.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                    return;
                }

                if (!BackendProvider_iOS.IsValidEmail(email))
                {
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    Email.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                    View.AddSubview(UtilProvider.GenericError("Incorrect Email format entered, please try again"));
                    return;
                }

                Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                Email.FloatingLabelActiveTextColor = UIColor.White;
                ResetWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));

                if (phoneNumber.Length <= 0)
                {
                    ResetWrapper.Add(UtilProvider.TextFieldBottomBorderError(PhoneNumber));
                    PhoneNumber.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                ResetWrapper.Add(UtilProvider.TextFieldBottomBorder(PhoneNumber));
                PhoneNumber.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                Operation operation = await BackendProvider.ResetPassword(AppDelegate.ApiKey, email, phoneNumber);
                if (operation.Result == OperationResult.Failure)
                {
                    // Display error component with message

                    loadingOverlay.Hide();
                    View.AddSubview(UtilProvider.GenericError(operation.Message));

                    return;
                }

                //loadingOverlay.Hide();
                View.AddSubview(UtilProvider.SuccessMesage("A password link with your credentials has been emailed and sms'd to you"));

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ForgotPasswordRecover.ToString());

                await Task.Delay(2000);

                NavigationController.PushViewController(new LoginScreen(false), true);
                //BackendProvider_iOS.StoreUserData_iOS(operation.Response, RememberUser );

            };

            var Cancel = new MDButton(new CGRect(0, ResetWrapper.Bounds.Height - 135, ResetWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            Cancel.SetTitle("CANCEL", UIControlState.Normal);
            Cancel.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            Cancel.BackgroundColor = UIColor.Clear;
            Cancel.Layer.BorderWidth = 1;
            Cancel.Layer.BorderColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan).CGColor;
            Cancel.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ForgotPasswordCancel.ToString());

                NavigationController.PushViewController(new LoginScreen(false), true);
            };
            
            
            ResetWrapper.Add(instruction);
            ResetWrapper.Add(info);
            ResetWrapper.Add(PhoneNumber);
            ResetWrapper.Add(UtilProvider.TextFieldBottomBorder(PhoneNumber));
            ResetWrapper.Add(Email);
            ResetWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
            ResetWrapper.Add(Reset);
            ResetWrapper.Add(Cancel);
            ResetWrapper.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            
            var scroll = new TPKeyboardAvoidingScrollView()
            {
                Frame = new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight)
            };
            scroll.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            scroll.Add(ResetWrapper);

            View.Add(scroll);
            View.Add(UtilProvider.HeaderStrip(true, false, this));
        }
        
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}


