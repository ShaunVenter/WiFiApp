using System;
using UIKit;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using FloatLabeledEntry;
using Foundation;
using MaterialControls;
using AlwaysOn;
using AlwaysOn.Objects;
using System.Text.RegularExpressions;
using System.Drawing;
using TPKeyboardAvoiding;

namespace AlwaysOn_iOS
{
    public partial class RegistrationScreen : UIViewController
    {
        string PackageUsername = "";
        string PackagePassword = "";

        LoadingOverlay loadingOverlay;

        public RegistrationScreen(string PackageUsername, string PackagePassword) : base("RegistrationScreen", null)
        {
            UtilProvider.MenuIsOpen = false;

            this.PackageUsername = PackageUsername;
            this.PackagePassword = PackagePassword;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            AnalyticsProvider_iOS.PageViewGA(PageName.Registration.ToString());

            var navController = base.NavigationController;
            navController.NavigationBarHidden = true;

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var RegistrationWrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight));

            var instruction = new UILabel(new CGRect(0, 10, 200, 30))
            {
                Text = "New to AlwaysOn? Register below.",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14)
            };

            var Name = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(1), RegistrationWrapper.Bounds.Width, 35))
            {
                Placeholder = "Name  ",
                AttributedPlaceholder = new NSAttributedString("Name  ", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White
            };
            Name.ShouldReturn += (textField) =>
            {
                Name.ResignFirstResponder();
                return true;
            };
            Name.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Name));
                    Name.AttributedPlaceholder = new NSAttributedString("Name", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Name));
                    Name.AttributedPlaceholder = new NSAttributedString("Name", null, UIColor.White);
                }
            };

            var Surname = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(2), RegistrationWrapper.Bounds.Width, 35))
            {
                Placeholder = "Surname",
                AttributedPlaceholder = new NSAttributedString("Surname", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,

            };
            Surname.ShouldReturn += (textField) =>
            {
                Surname.ResignFirstResponder();
                return true;
            };
            Surname.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Surname));
                    Surname.AttributedPlaceholder = new NSAttributedString("Surname", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Surname));
                    Surname.AttributedPlaceholder = new NSAttributedString("Surname", null, UIColor.White);
                }
            };

            var Email = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(3), RegistrationWrapper.Bounds.Width, 35))
            {
                Placeholder = "Email",
                AttributedPlaceholder = new NSAttributedString("Email", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                KeyboardType = UIKeyboardType.EmailAddress,
                AutocapitalizationType = UITextAutocapitalizationType.None
            };
            Email.ShouldReturn += (textField) =>
            {
                Email.ResignFirstResponder();
                return true;
            };
            Email.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0 || !BackendProvider_iOS.IsValidEmail(((UITextField)sender).Text))
                {
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email)); ;
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email", null, UIColor.White);
                }
            };

            var MobileNumber = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(4), RegistrationWrapper.Bounds.Width, 35))
            {
                Placeholder = "Mobile No.",
                AttributedPlaceholder = new NSAttributedString("Mobile No.", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                KeyboardType = UIKeyboardType.NumberPad
            };
            MobileNumber.ShouldReturn += (textField) =>
            {
                MobileNumber.ResignFirstResponder();
                return true;
            };
            MobileNumber.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(MobileNumber));
                    MobileNumber.AttributedPlaceholder = new NSAttributedString("Mobile No.", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(MobileNumber));
                    MobileNumber.AttributedPlaceholder = new NSAttributedString("Mobile No.", null, UIColor.White);
                }
            };

            var Password = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(5), RegistrationWrapper.Bounds.Width, 35))
            {
                Placeholder = "Password",
                AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                SecureTextEntry = true
            };
            Password.ShouldReturn += (textField) =>
            {
                Password.ResignFirstResponder();
                return true;
            };
            Password.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.White);
                }
            };

            var ConfirmPassword = new FloatLabeledTextField(new CGRect(0, UtilProvider.ItemPosition(6), RegistrationWrapper.Bounds.Width, 35))
            {
                Placeholder = "Confirm Password",
                AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.White),
                FloatingLabelTextColor = UIColor.White,
                FloatingLabelActiveTextColor = UIColor.White,
                FloatingLabelFont = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                SecureTextEntry = true
            };
            ConfirmPassword.ShouldReturn += (textField) =>
            {
                ConfirmPassword.ResignFirstResponder();
                return true;
            };
            ConfirmPassword.EditingDidEnd += (object sender, EventArgs e) =>
            {
                if (((UITextField)sender).Text.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(ConfirmPassword));
                    ConfirmPassword.AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(ConfirmPassword));
                    ConfirmPassword.AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.White);
                }
            };

            var RegisterButton = new MDButton(new CGRect(RegistrationWrapper.Bounds.Width - (RegistrationWrapper.Bounds.Width / 2) + 5, UtilProvider.ItemPosition(7), (RegistrationWrapper.Bounds.Width / 2) - 5, 38), MDButtonType.Flat, UIColor.White);
            RegisterButton.SetTitle("REGISTER", UIControlState.Normal);
            RegisterButton.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            RegisterButton.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            RegisterButton.TouchUpInside += async (object sender, EventArgs e) =>
            {
                Name.ResignFirstResponder();
                Surname.ResignFirstResponder();
                Email.ResignFirstResponder();
                MobileNumber.ResignFirstResponder();
                ConfirmPassword.ResignFirstResponder();
                Password.ResignFirstResponder();


                var name = string.IsNullOrWhiteSpace(Name.Text) ? string.Empty : Name.Text.Trim();
                var surname = string.IsNullOrWhiteSpace(Surname.Text) ? string.Empty : Surname.Text.Trim();
                var email = string.IsNullOrWhiteSpace(Email.Text) ? string.Empty : Email.Text.Trim();
                var mobile = string.IsNullOrWhiteSpace(MobileNumber.Text) ? string.Empty : MobileNumber.Text.Trim();

                var password = string.IsNullOrWhiteSpace(Password.Text) ? string.Empty : Password.Text.Trim();
                var passConfirm = string.IsNullOrWhiteSpace(ConfirmPassword.Text) ? string.Empty : ConfirmPassword.Text.Trim();

                if (name.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Name));
                    Name.AttributedPlaceholder = new NSAttributedString("Name", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Name));
                Name.AttributedPlaceholder = new NSAttributedString("Name", null, UIColor.White);

                if (surname.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Surname));
                    Surname.AttributedPlaceholder = new NSAttributedString("Surname", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Surname));
                Surname.AttributedPlaceholder = new NSAttributedString("Surname", null, UIColor.White);

                if (email.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                if (!BackendProvider_iOS.IsValidEmail(email))
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    View.AddSubview(UtilProvider.GenericError("Incorrect Email format entered, please try again"));
                    return;
                }

                RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
                Email.AttributedPlaceholder = new NSAttributedString("Email", null, UIColor.White);

                if (mobile.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(MobileNumber));
                    MobileNumber.AttributedPlaceholder = new NSAttributedString("Mobile No", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }



                RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(MobileNumber));
                MobileNumber.AttributedPlaceholder = new NSAttributedString("Mobile No.", null, UIColor.White);

                if (password.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }


                RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
                Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.White);

                if (passConfirm.Length <= 0)
                {
                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(ConfirmPassword));
                    ConfirmPassword.AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(ConfirmPassword));
                ConfirmPassword.AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.White);

                if (password != passConfirm)
                {
                    View.Add(UtilProvider.GenericError("Your Passwords don't match"));

                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorderError(ConfirmPassword));
                    ConfirmPassword.AttributedPlaceholder = new NSAttributedString("Confirm Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                Operation operation = await BackendProvider.RegisterUser(AppDelegate.ApiKey, name, surname, email, mobile, password);
                if (operation.Result == OperationResult.Failure)
                {
                    loadingOverlay.Hide();
                    // Display error component with message
                    View.Add(UtilProvider.GenericError(operation.Message));
                    return;
                }

                var userResponse = (UserResponse)operation.Response;

                BackendProvider_iOS.SetUser(userResponse, true, password);

                if (!string.IsNullOrEmpty(PackageUsername) && !string.IsNullOrEmpty(PackagePassword))
                {
                    var linkoperation = await BackendProvider.LinkPurchasedPackage(AppDelegate.ApiKey, userResponse.UserId, PackageUsername, PackagePassword, "1");
                    if (linkoperation.Result == OperationResult.Success)
                    {
                        PackageUsername = "";
                        PackagePassword = "";
                    }
                }

                BackendProvider_iOS.UpdatePackagesFromServer();
                var _infoButton = await BackendProvider_iOS.GetInfoButton();

                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.RegisterRegister.ToString());

                UtilProvider.NavigationControllerWithCustomTransition(NavigationController).PushViewController(new Dashboard(_infoButton), false);
            };

            var Registered = new UILabel(new CGRect(0, UtilProvider.ItemPosition(8), 150, 30))
            {
                Text = "Already have an account?",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Left
            };

            var SignIn = new MDButton(new CGRect(RegistrationWrapper.Bounds.Width - 75, UtilProvider.ItemPosition(8) + 3, 75, 25), MDButtonType.Flat, UIColor.White);
            SignIn.SetTitle("SIGN IN", UIControlState.Normal);
            SignIn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            SignIn.BackgroundColor = UIColor.Clear;
            SignIn.Layer.BorderWidth = 1;
            SignIn.Layer.BorderColor = UIColor.White.CGColor;

            SignIn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.RegisterSignin.ToString());

                NavigationController.PushViewController(new LoginScreen(false), true);
            };

            RegistrationWrapper.Add(instruction);
            RegistrationWrapper.Add(Name);
            RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Name));
            RegistrationWrapper.Add(Surname);
            RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Surname));
            RegistrationWrapper.Add(Email);
            RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
            RegistrationWrapper.Add(MobileNumber);
            RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(MobileNumber));
            RegistrationWrapper.Add(Password);
            RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
            RegistrationWrapper.Add(ConfirmPassword);
            RegistrationWrapper.Add(UtilProvider.TextFieldBottomBorder(ConfirmPassword));

            RegistrationWrapper.Add(RegisterButton);
            RegistrationWrapper.Add(Registered);
            RegistrationWrapper.Add(SignIn);

            var scroll = new TPKeyboardAvoidingScrollView()
            {
                Frame = new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight)
            };
            scroll.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            scroll.Add(RegistrationWrapper);

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