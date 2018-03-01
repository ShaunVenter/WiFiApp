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
using TPKeyboardAvoiding;
using Google.Analytics;
using Shimmer;
using Xamarin.Social;
using Xamarin.Social.Services;

namespace AlwaysOn_iOS
{
    public partial class LoginScreen : UIViewController
    {
        string PackageUsername = "";
        string PackagePassword = "";

        UISvgImageView _CheckBoxOff = new UISvgImageView();
        UISvgImageView _CheckBoxOn = new UISvgImageView();
        LoadingOverlay loadingOverlay;

        bool _messages = false;
        bool RememberUser = true;

        public LoginScreen(bool Messages) : base("LoginScreen", null)
        {
            UtilProvider.MenuIsOpen = false;
            _messages = Messages;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            // remove nav bar

            AnalyticsProvider_iOS.PageViewGA(PageName.Login.ToString());

            // screen bounds

            var shimmeringView = new ShimmeringView(View.Bounds);
            View.AddSubview(shimmeringView);

            var navController = base.NavigationController;
            navController.NavigationBarHidden = true;

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var LoginWrapper = new UIView(new CGRect(40, UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight - UtilProvider.ScreenHeight12th));

            var instruction = new UILabel(new CGRect(0, 10, 150, 30))
            {
                Text = "Please sign in below.",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14)
            };

            var Email = new FloatLabeledTextField(new CGRect(0, UtilProvider.ScreenHeight12th * 2, LoginWrapper.Bounds.Width, 35))
            {
                Placeholder = "Email address",
                AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.White),
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
                if (((UITextField)sender).Text.Length <= 0)// || !BackendProvider_iOS.IsValidEmail(((UITextField)sender).Text))
                {
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    LoginWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email)); ;
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    Email.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    LoginWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.White);
                    Email.FloatingLabelActiveTextColor = UIColor.White;
                }
            };

            var Password = new FloatLabeledTextField(new CGRect(0, UtilProvider.ScreenHeight12th * 3, LoginWrapper.Bounds.Width, 35))
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
                    //_borderBottom.BackgroundColor = UIColor.Red;
                    LoginWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                }
                else
                {
                    //_borderBottom.BackgroundColor = UIColor.White;	
                    LoginWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.White);
                }
            };

            var ForgotPwd = new UILabel(new CGRect(Password.Bounds.Width - 50, (UtilProvider.ScreenHeight12th * 3) + 40, 50, 30))
            {
                Text = "Forgot?",
                TextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 12f),
                TextAlignment = UITextAlignment.Center
            };
            ForgotPwd.UserInteractionEnabled = true;
            ForgotPwd.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LoginForgot.ToString());

                NavigationController.PushViewController(new ResetPasswordPage(), true);
            }));

            _CheckBoxOff = new UISvgImageView("svg/Check_off.svg")
            {
                Frame = new CGRect(0, (UtilProvider.ScreenHeight12th * 5) - 10, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            _CheckBoxOff.UserInteractionEnabled = true;
            _CheckBoxOff.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                RememberUser = true;
                _CheckBoxOn.Alpha = 1;
            }));
            _CheckBoxOn = new UISvgImageView("svg/Check_on.svg")
            {
                Frame = new CGRect(0, (UtilProvider.ScreenHeight12th * 5) - 10, 15, 30),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            _CheckBoxOn.UserInteractionEnabled = true;
            _CheckBoxOn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                RememberUser = false;
                _CheckBoxOn.Alpha = 0;
            }));

            var RememberMe = new UILabel(new CGRect(25, (UtilProvider.ScreenHeight12th * 5) - 10, 75, 30))
            {
                Text = "Remember me",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Center
            };
            RememberMe.UserInteractionEnabled = true;
            RememberMe.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (RememberUser)
                {
                    RememberUser = false;
                    _CheckBoxOn.Alpha = 0;

                }
                else
                {
                    RememberUser = true;
                    _CheckBoxOn.Alpha = 1;
                }
            }));

            var ErrorWrapper = new UIView(new CGRect(40, 20, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight12th * 2 + 20))
            {
                BackgroundColor = UIColor.Clear.FromHex(0xf2bdbb),
                Hidden = true
            };
            ErrorWrapper.Layer.BorderWidth = 1.5f;
            ErrorWrapper.Layer.BorderColor = UIColor.Clear.FromHex(0xd13932).CGColor;
            ErrorWrapper.Layer.CornerRadius = 5f;

            var ErrorText = new UILabel(new CGRect(10, 0, ErrorWrapper.Bounds.Width - 20, ErrorWrapper.Bounds.Height))
            {
                TextColor = UIColor.Clear.FromHex(0xd13932),
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Left
            };
            ErrorText.PreferredMaxLayoutWidth = 280f;
            ErrorWrapper.Add(ErrorText);

            var SignIn = new MDButton(new CGRect(LoginWrapper.Bounds.Width - (LoginWrapper.Bounds.Width / 2), (UtilProvider.ScreenHeight12th * 5) - 15, LoginWrapper.Bounds.Width / 2, 40), MDButtonType.Flat, UIColor.White);
            SignIn.SetTitle("SIGN IN", UIControlState.Normal);
            SignIn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            SignIn.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            SignIn.TouchUpInside += async (object sender, EventArgs e) =>
            {
                if (!ErrorWrapper.Hidden)
                {
                    var frame = LoginWrapper.Frame;
                    frame.Offset(0, -1 * ErrorWrapper.Bounds.Height);
                    ErrorWrapper.Hidden = true;
                    LoginWrapper.Frame = frame;
                }

                // hide the keyboard
                Email.ResignFirstResponder();
                Password.ResignFirstResponder();

                var email = string.IsNullOrWhiteSpace(Email.Text) ? string.Empty : Email.Text.Trim();
                var password = string.IsNullOrWhiteSpace(Password.Text) ? string.Empty : Password.Text.Trim();
                //var rememberMe = RemeberUser == true ? 1: 0;

                if (email.Length <= 0)
                {
                    LoginWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email));
                    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                    Email.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                    return;
                }

                //if (!BackendProvider_iOS.IsValidEmail(email))
                //{
                //    LoginWrapper.Add(UtilProvider.TextFieldBottomBorderError(Email));
                //    Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                //    Email.FloatingLabelActiveTextColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError);
                //    View.AddSubview(UtilProvider.GenericError("Incorrect Email format entered, please try again"));
                //    return;
                //}

                Email.AttributedPlaceholder = new NSAttributedString("Email address", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));
                Email.FloatingLabelActiveTextColor = UIColor.White;
                LoginWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));

                if (password.Length <= 0)
                {
                    LoginWrapper.Add(UtilProvider.TextFieldBottomBorderError(Password));
                    Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                    return;
                }

                LoginWrapper.Add(UtilProvider.TextFieldBottomBorder(Password));
                Password.AttributedPlaceholder = new NSAttributedString("Password", null, UIColor.Clear.FromHex(ConfigurationProvider.AppColorFieldError));

                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new LoadingOverlay(UtilProvider.ScreenBounds);
                View.Add(loadingOverlay);

                Operation operation = await BackendProvider.UserLogin(AppDelegate.ApiKey, email, password, RememberUser);
                if (operation.Result == OperationResult.Failure)
                {
                    loadingOverlay.Hide();
                    // Display error component with message
                    var error = UtilProvider.GenericError(operation.Message);
                    error.Frame = new CGRect(0, UtilProvider.ScreenHeight / 12, UtilProvider.ScreenWidth, (UtilProvider.ScreenHeight / 13) + 15);
                    View.AddSubview(error);
                    return;
                }
                var userResponse = (UserResponse)operation.Response;
                if (userResponse.IsPackageCredentials)
                {
                    if (userResponse.IsPackageLinked)
                    {
                        ErrorWrapper.Hidden = false;
                        ErrorText.Text = "You have entered a package username and password.\n\nThis package is already linked to an AlwaysOn email account \n" + userResponse.PackageLinkedAccount;
                        ErrorText.Lines = 6;
                    }
                    else
                    {
                        PackageUsername = email;
                        PackagePassword = password;

                        ErrorWrapper.Hidden = false;
                        ErrorText.Text = "You have entered a package username and password.\n\nPlease sign in with your AlwaysOn email account or register a new account by clicking the register button below.";
                        ErrorText.Lines = 8;
                    }

                    var frame = LoginWrapper.Frame;
                    frame.Offset(0, ErrorWrapper.Bounds.Height);
                    LoginWrapper.Frame = frame;
                    
                    loadingOverlay.Hide();
                }
                else
                {
                    BackendProvider_iOS.SetUser(userResponse, RememberUser, password);

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

                    AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LoginSignin.ToString());

                    UtilProvider.NavigationControllerWithCustomTransition(NavigationController).PushViewController(new Dashboard(_infoButton), false);
                }
            };

            var NotRegistered = new UILabel(new CGRect(0, UtilProvider.ScreenHeight12th * 6, 95, 30))
            {
                Text = "Not yet registered?",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 12f),
                TextAlignment = UITextAlignment.Center
            };

            var RegisterBtn = new MDButton(new CGRect(LoginWrapper.Bounds.Width - 95, UtilProvider.ScreenHeight12th * 6, 95, 30), MDButtonType.Flat, UIColor.White);
            RegisterBtn.SetTitle("REGISTER", UIControlState.Normal);
            RegisterBtn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            RegisterBtn.BackgroundColor = UIColor.Clear;
            RegisterBtn.Layer.BorderWidth = 1;
            RegisterBtn.Layer.BorderColor = UIColor.White.CGColor;
            RegisterBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LoginRegister.ToString());

                NavigationController.PushViewController(new RegistrationScreen(PackageUsername, PackagePassword), true);
            };


            LoginWrapper.Add(instruction);

            LoginWrapper.Add(Email);
            LoginWrapper.Add(UtilProvider.TextFieldBottomBorder(Email));
            LoginWrapper.Add(Password);
            LoginWrapper.AddSubview(UtilProvider.TextFieldBottomBorder(Password));

            LoginWrapper.Add(ForgotPwd);

            LoginWrapper.Add(SignIn);
            LoginWrapper.Add(RememberMe);
            LoginWrapper.Add(_CheckBoxOff);
            LoginWrapper.Add(_CheckBoxOn);

            LoginWrapper.Add(NotRegistered);
            LoginWrapper.Add(RegisterBtn);

            var scroll = new TPKeyboardAvoidingScrollView()
            {
                Frame = new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight)
            };
            scroll.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            scroll.Add(ErrorWrapper);
            scroll.Add(LoginWrapper);

            View.Add(scroll);
            View.Add(UtilProvider.HeaderStrip(false, false, this));

            if (_messages)
            {
                View.Add(UtilProvider.SuccessMesage("An SMS and Email have been sent to you with your Password"));
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}