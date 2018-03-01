using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Timers;

using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using XamSvg;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Support.V7.AppCompat;
using AlwaysOn;
using AlwaysOn.Objects;
using Android.Views.Animations;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Login")]
    public class Login : Activity
    {
        string PackageUsername = "";
        string PackagePassword = "";

        SvgImageView img;
        CustomTextView txtError;
        CustomTextView txtTitle;
        LinearLayout lytContainer;
        CustomEditText txtEmail;
        CustomEditText txtPassword;
        TextInputLayout lyttxtPassword;
        LinearLayout lytEmail;
        TextInputLayout lyttxtEmail;
        RelativeLayout lytPassword;
        RelativeLayout lytButtons;
        CustomCheckBox chkRemember;
        CustomButton btnSignIn;
        CustomTextView txtForgot;
        CustomTextView lblRegister;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;
        
        LinearLayout lytNotification;

        CustomButton btnRegister;
        Animation fadeOutAnimation;

        bool RememberUser = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.Login.ToString());

            XamSvg.Setup.InitSvgLib();
            SetContentView(Resource.Layout.Login);

            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            lytContainer = FindViewById<LinearLayout>(Resource.Id.lytForm);
            lytContainer.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), 0);

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(10), Utils.CalcDimension(70), Utils.CalcDimension(10));
            lytNotification.Visibility = ViewStates.Gone;
            txtError = FindViewById<CustomTextView>(Resource.Id.txtError);
            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);
            //txtTitle.TextSize = CalcNewWidth (14);

            lytEmail = FindViewById<LinearLayout>(Resource.Id.lytEmail);
            lytEmail.SetPadding(0, Utils.CalcDimension(200), 0, 0);

            lyttxtEmail = FindViewById<TextInputLayout>(Resource.Id.lyttxtEmail);
            lyttxtEmail.Hint = "Email";
            txtEmail = FindViewById<CustomEditText>(Resource.Id.txtEmail);
            txtEmail.SetHintTextColor(Color.White);
            txtEmail.SetHighlightColor(Color.White);

            //focus change event
            txtEmail.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtEmail.Text))// || BackendProvider_Droid.IsValidEmail(txtEmail.Text) == false)
                {
                    lyttxtEmail.Error = "";

                    if (txtEmail.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtEmail.Error = " ";
                    }
                }
            };

            lytPassword = FindViewById<RelativeLayout>(Resource.Id.lytPassword);
            lytPassword.LayoutParameters.Height = Utils.CalcDimension(115);

            lyttxtPassword = FindViewById<TextInputLayout>(Resource.Id.lyttxtPassword);
            lyttxtPassword.Hint = "Password";
            txtPassword = FindViewById<CustomEditText>(Resource.Id.txtPassword);
            txtPassword.SetHintTextColor(Color.Yellow);
            
            lytButtons = FindViewById<RelativeLayout>(Resource.Id.lytButtons);
            lytButtons.LayoutParameters.Height = Utils.CalcDimension(160);
            lytButtons.SetPadding(0, Utils.CalcDimension(45), 0, 0);

            chkRemember = FindViewById<CustomCheckBox>(Resource.Id.chkRemember);
            chkRemember.Checked = true;
            chkRemember.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e) =>
            {
                RememberUser = chkRemember.Checked;
            };

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            
            txtForgot = FindViewById<CustomTextView>(Resource.Id.txtForgot);
            txtForgot.Click += txtForgot_Click;
            txtForgot.SetPadding(0, 0, 0, Utils.CalcDimension(20));

            txtPassword.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                lyttxtPassword.Error = "";
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    if (txtPassword.HasFocus == false)
                    {
                        lyttxtPassword.Error = " ";
                        //txtEmail.SetHintTextColor(Color.Red);
                        txtForgot.SetPadding(0, 0, 0, Utils.CalcDimension(45));
                    }
                }
            };

            fadeOutAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeOutAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                lytNotification.Visibility = ViewStates.Gone;

                HideLoader();
            };

            btnSignIn = FindViewById<CustomButton>(Resource.Id.btnSignIn);
            //btnSignIn.TextSize = CalcNewWidth (15);
            btnSignIn.LayoutParameters.Width = Utils.CalcDimension(245);
            btnSignIn.LayoutParameters.Height = Utils.CalcDimension(75);

            lblRegister = FindViewById<CustomTextView>(Resource.Id.lblRegister);
            //lblRegister.TextSize = txtPassword.TextSize = CalcNewWidth (14);

            btnRegister = FindViewById<CustomButton>(Resource.Id.btnRegister);
            btnRegister.Click += btnRegister_Click;
            btnRegister.LayoutParameters.Width = Utils.CalcDimension(155);
            btnRegister.LayoutParameters.Height = Utils.CalcDimension(55);

            btnSignIn.Click += BtnSignIn_Click;

            HideLoader();
        }
        
        private async void BtnSignIn_Click(object sender, EventArgs e)
        {
            ShowLoader();

            lyttxtEmail.Error = "";
            lyttxtPassword.Error = "";
            txtError.Visibility = ViewStates.Gone;

            bool valid = true;

            string email = string.IsNullOrWhiteSpace(txtEmail.Text) ? string.Empty : txtEmail.Text.Trim();
            string password = string.IsNullOrWhiteSpace(txtPassword.Text) ? string.Empty : txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email))// || BackendProvider_Droid.IsValidEmail(email) == false)
            {
                lyttxtEmail.Error = " ";

                valid = false;
            }

            if (string.IsNullOrEmpty(password))
            {
                lyttxtPassword.Error = " ";
                txtForgot.SetPadding(0, 0, 0, Utils.CalcDimension(40));
                valid = false;
            }

            if (valid)
            {
                Operation operation = await BackendProvider.UserLogin(MainApplication.ApiKey, email, password, RememberUser);

                if (operation.Result == OperationResult.Failure)
                {
                    var text = new CustomTextView(this)
                    {
                        Gravity = GravityFlags.Left,
                        Text = "Oops! Invalid Email and Password, please try again",
                        TextSize = 14,
                    };
                    text.SetCustomFont("Fonts/FocoCorp-Regular.ttf");
                    lytNotification.AddView(text);
                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(100);
                    lytNotification.Visibility = ViewStates.Visible;

                    await Task.Delay(2000);
                    lytNotification.StartAnimation(fadeOutAnimation);

                    HideLoader();

                    return;
                }
                var userResponse = (UserResponse)operation.Response;
                if (userResponse.IsPackageCredentials)
                {
                    if (userResponse.IsPackageLinked)
                    {
                        txtError.Visibility = ViewStates.Visible;
                        txtError.Text = "You have entered a package username and password.\n\nThis package is already linked to an AlwaysOn email account \n" + userResponse.PackageLinkedAccount;
                    }
                    else
                    {
                        PackageUsername = email;
                        PackagePassword = password;

                        txtError.Visibility = ViewStates.Visible;
                        txtError.Text = "You have entered a package username and password.\n\nPlease sign in with your AlwaysOn email account or register a new account by clicking the register button below.";
                    }
                }
                else
                {
                    BackendProvider_Droid.SetUser(userResponse, RememberUser, password);

                    if (!string.IsNullOrEmpty(PackageUsername) && !string.IsNullOrEmpty(PackagePassword))
                    {
                        var linkoperation = await BackendProvider.LinkPurchasedPackage(MainApplication.ApiKey, userResponse.UserId, PackageUsername, PackagePassword, "1");
                        if (linkoperation.Result == OperationResult.Success)
                        {
                            PackageUsername = "";
                            PackagePassword = "";
                        }
                    }

                    BackendProvider_Droid.UpdatePackagesFromServer();

                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LoginSignin.ToString());

                    var intent = new Intent(this, typeof(Dashboard));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    Finish();
                }
            }

            HideLoader();
        }

        void txtForgot_Click(object sender, EventArgs e)
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LoginForgot.ToString());

            var forgotPassword = new Intent(this, typeof(ForgotPassword));
            forgotPassword.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(forgotPassword);
            Finish();
        }

        void btnRegister_Click(object sender, EventArgs e)
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LoginRegister.ToString());

            var register = new Intent(this, typeof(Register));
            register.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            register.PutExtra("PackageUsername", PackageUsername);
            register.PutExtra("PackagePassword", PackagePassword);
            StartActivity(register);
            Finish();
        }

        public void ShowLoader()
        {
            myProgressBar.Visibility = ViewStates.Visible;
            myProgressBar.Clickable = true;
            lytprogressBar.Visibility = ViewStates.Visible;
            lytprogressBar.Clickable = true;
        }

        public void HideLoader()
        {
            myProgressBar.Visibility = ViewStates.Invisible;
            myProgressBar.Clickable = false;
            lytprogressBar.Visibility = ViewStates.Invisible;
            lytprogressBar.Clickable = false;
        }
    }
}
