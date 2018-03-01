using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Threading.Tasks;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Register")]
    public class Register : Activity
    {
        string PackageUsername = "";
        string PackagePassword = "";

        SvgImageView img;
        SvgImageView imgBack;

        CustomTextView txtTitle;
        RelativeLayout lytContainer;
        RelativeLayout lytMain;
        CustomEditText txtName;
        CustomEditText txtSurname;
        CustomEditText txtEmail;
        CustomEditText txtPassword;
        CustomEditText txtConfirmPassword;
        CustomEditText txtPhoneNumber;
        LinearLayout lytName;
        LinearLayout lytSurname;
        LinearLayout lytEmail;
        LinearLayout lytPassword;
        LinearLayout lytPhoneNumber;
        LinearLayout lytConfirmPassword;
        TextInputLayout lyttxtName;
        TextInputLayout lyttxtSurname;
        TextInputLayout lyttxtEmail;
        TextInputLayout lyttxtPassword;
        TextInputLayout lyttxtPhoneNumber;
        TextInputLayout lyttxtConfirmPassword;
        RelativeLayout lytButtons;
        CustomButton btnSignIn;

        RelativeLayout lytprogressBar;
        ProgressBar myProgressBar;

        Animation fadeInAnimation;
        LinearLayout lytNotification;
        CustomButton btnRegister;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.Registration.ToString());

            SetContentView(Resource.Layout.Register);

            if (Intent.HasExtra("PackageUsername"))
            {
                PackageUsername = Intent.GetStringExtra("PackageUsername");
                PackagePassword = Intent.GetStringExtra("PackagePassword");
            }

            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            imgBack = FindViewById<SvgImageView>(Resource.Id.imgBack);
            imgBack.SetSvg(this, Resource.Raw.AO__arrow_back);
            imgBack.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(20));
            imgBack.LayoutParameters.Width = Utils.CalcDimension(130);
            imgBack.Click += btnCancel_Click;

            lytContainer = FindViewById<RelativeLayout>(Resource.Id.lytContainer);
            lytContainer.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            lytMain = FindViewById<RelativeLayout>(Resource.Id.lytMain);
            lytMain.RequestFocus();

            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);

            lytName = FindViewById<LinearLayout>(Resource.Id.lytName);
            lytName.SetPadding(0, Utils.CalcDimension(15), 0, 0);
            lyttxtName = FindViewById<TextInputLayout>(Resource.Id.lyttxtName);
            lyttxtName.Hint = "Name";
            txtName = FindViewById<CustomEditText>(Resource.Id.txtName);
            txtName.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    lyttxtName.Error = "";

                    if (txtName.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtName.Error = " ";
                    }
                }
            };

            lytSurname = FindViewById<LinearLayout>(Resource.Id.lytSurname);
            lytSurname.SetPadding(0, Utils.CalcDimension(15), 0, 0);
            lyttxtSurname = FindViewById<TextInputLayout>(Resource.Id.lyttxtSurname);
            lyttxtSurname.Hint = "Surname";
            txtSurname = FindViewById<CustomEditText>(Resource.Id.txtSurName);
            txtSurname.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtSurname.Text))
                {
                    lyttxtSurname.Error = "";

                    if (txtSurname.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtSurname.Error = " ";
                    }
                }
            };

            lytEmail = FindViewById<LinearLayout>(Resource.Id.lytEmail);
            lytEmail.SetPadding(0, Utils.CalcDimension(15), 0, 0);
            lyttxtEmail = FindViewById<TextInputLayout>(Resource.Id.lyttxtEmail);
            lyttxtEmail.Hint = "Email";
            txtEmail = FindViewById<CustomEditText>(Resource.Id.txtEmail);
            txtEmail.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtEmail.Text) || BackendProvider_Droid.IsValidEmail(txtEmail.Text) == false)
                {
                    lyttxtEmail.Error = "";

                    if (txtEmail.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtEmail.Error = " ";
                    }
                }
            };
            
            lytPhoneNumber = FindViewById<LinearLayout>(Resource.Id.lytPhoneNumber);
            lytPhoneNumber.SetPadding(0, Utils.CalcDimension(15), 0, 0);
            lyttxtPhoneNumber = FindViewById<TextInputLayout>(Resource.Id.lyttxtPhoneNumber);
            lyttxtPhoneNumber.Hint = "Mobile No.";
            txtPhoneNumber = FindViewById<CustomEditText>(Resource.Id.txtPhoneNumber);
            txtPhoneNumber.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                    lyttxtPhoneNumber.Error = "";

                    if (txtPhoneNumber.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtPhoneNumber.Error = " ";
                    }
                }
            };

            lytPassword = FindViewById<LinearLayout>(Resource.Id.lytPassword);
            lytPassword.SetPadding(0, Utils.CalcDimension(15), 0, 0);
            lyttxtPassword = FindViewById<TextInputLayout>(Resource.Id.lyttxtPassword);
            lyttxtPassword.Hint = "Password";
            txtPassword = FindViewById<CustomEditText>(Resource.Id.txtPassword);
            txtPassword.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    lyttxtPassword.Error = "";

                    if (txtPassword.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtPassword.Error = " ";
                    }
                }
            };

            lytConfirmPassword = FindViewById<LinearLayout>(Resource.Id.lytConfirmPassword);
            lytConfirmPassword.SetPadding(0, Utils.CalcDimension(15), 0, 0);
            lyttxtConfirmPassword = FindViewById<TextInputLayout>(Resource.Id.lyttxtConfirmPassword);
            lyttxtConfirmPassword.Hint = "Confirm Password";
            txtConfirmPassword = FindViewById<CustomEditText>(Resource.Id.txtConfirmPassword);
            txtConfirmPassword.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtConfirmPassword.Text))
                {
                    lyttxtConfirmPassword.Error = "";

                    if (txtConfirmPassword.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtConfirmPassword.Error = " ";
                    }
                }
            };

            lytButtons = FindViewById<RelativeLayout>(Resource.Id.lytButtons);
            lytButtons.LayoutParameters.Height = Utils.CalcDimension(160);

            btnRegister = FindViewById<CustomButton>(Resource.Id.btnRegister);
            btnRegister.LayoutParameters.Width = Utils.CalcDimension(245);
            btnRegister.LayoutParameters.Height = Utils.CalcDimension(75);
            btnRegister.Click += BtnRegister_Click;

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            btnSignIn = FindViewById<CustomButton>(Resource.Id.btnSignIn);
            btnSignIn.LayoutParameters.Width = Utils.CalcDimension(155);
            btnSignIn.LayoutParameters.Height = Utils.CalcDimension(55);
            btnSignIn.Click += btnCancel_Click;
            
            fadeInAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeInAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                lytNotification.Visibility = ViewStates.Gone;
                HideLoader();
            };

            HideLoader();
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            ShowLoader();

            bool valid = true;

            var name = string.IsNullOrWhiteSpace(txtName.Text) ? string.Empty : txtName.Text.Trim();
            var surname = string.IsNullOrWhiteSpace(txtSurname.Text) ? string.Empty : txtSurname.Text.Trim();
            var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? string.Empty : txtEmail.Text.Trim();
            var mobile = string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? string.Empty : txtPhoneNumber.Text.Trim();
            var password = string.IsNullOrWhiteSpace(txtPassword.Text) ? string.Empty : txtPassword.Text.Trim();
            var confirmpassword = string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ? string.Empty : txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || BackendProvider_Droid.IsValidEmail(email) == false)
            {
                lyttxtEmail.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(name))
            {
                lyttxtName.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(surname))
            {
                lyttxtSurname.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(password))
            {
                lyttxtPassword.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(confirmpassword))
            {
                lyttxtConfirmPassword.Error = " ";
                valid = false;
            }
            if (string.IsNullOrEmpty(mobile))
            {
                lyttxtPhoneNumber.Error = " ";
                valid = false;
            }

            if (!(password.Equals(confirmpassword)))
            {
                lyttxtConfirmPassword.Error = " ";
                lyttxtPassword.Error = " ";
                valid = false;
            }

            if (valid)
            {
                Operation operation = await BackendProvider.RegisterUser(MainApplication.ApiKey, name, surname, email, mobile, password);

                if (operation.Result == OperationResult.Failure)
                {
                    lytNotification = new LinearLayout(this)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
                    };
                    lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(10), Utils.CalcDimension(70), Utils.CalcDimension(10));

                    var text = new CustomTextView(this)
                    {
                        Gravity = GravityFlags.Left,
                        Text = "Oops! An account for this email address has already been created.",
                        TextSize = 14,
                    };
                    text.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

                    lytMain.AddView(lytNotification);
                    lytNotification.AddView(text);
                    lytNotification.Visibility = ViewStates.Visible;
                    await Task.Delay(2000);
                    lytNotification.StartAnimation(fadeInAnimation);
                    lytNotification.SetBackgroundColor(Color.ParseColor("#f0674d"));
                    lytNotification.TranslationY = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);
                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(100);
                    lytprogressBar.Visibility = ViewStates.Gone;

                    HideLoader();

                    return;
                }

                var userResponse = (UserResponse)operation.Response;

                BackendProvider_Droid.SetUser(userResponse, true, password);

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

                AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.RegisterRegister.ToString());

                var intent = new Intent(this, typeof(Dashboard));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
                Finish();
            }

            HideLoader();
        }
        
        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.RegisterSignin.ToString());

            var login = new Intent(this, typeof(Login));
            login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(login);
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

