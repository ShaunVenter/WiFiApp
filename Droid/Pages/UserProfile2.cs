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
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.UserProfile2")]
    public class UserProfile2 : Activity
    {
        SvgImageView img;
        SvgImageView imgBack;
        SvgImageView imgMenu;
        CustomEditText txtName;
        CustomEditText txtSurname;
        CustomEditText txtEmail;
        CustomEditText txtMobile;
        TextInputLayout lyttxtName;
        TextInputLayout lyttxtSurname;
        LinearLayout lytNotification;
        TextInputLayout lyttxtEmail;
        TextInputLayout lyttxtMobile;
        CustomButton btnSave;
        CustomButton btnCancel;
        CustomTextView txtNotification;
        RelativeLayout lytContainer;

        RelativeLayout lytprogressBar;
        ProgressBar myProgressBar;

        Animation fadeInAnimation;

        UserObject User = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.ProfileDetailsEdit.ToString());

            SetContentView(Resource.Layout.UserProfile2);

            User = BackendProvider_Droid.GetUser;
            if (User == null)
            {
                var login = new Intent(this, typeof(Login));
                login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(login);
                Finish();
                return;
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
            imgBack.Click += imgBack_Click;

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);
            imgMenu.Click += imgMenu_Click;

            lytContainer = FindViewById<RelativeLayout>(Resource.Id.lytContainer);
            lytContainer.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            lyttxtName = FindViewById<TextInputLayout>(Resource.Id.lyttxtName);
            lyttxtName.SetPadding(0, Utils.CalcDimension(55), 0, 0);
            lyttxtName.Hint = "Name";
            txtName = FindViewById<CustomEditText>(Resource.Id.txtName);
            txtName.Text = User.Name;
            txtName.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    lyttxtName.Error = "";

                    if (txtName.HasFocus == false)
                    {
                        lyttxtName.Error = " ";
                    }
                }
            };

            lyttxtSurname = FindViewById<TextInputLayout>(Resource.Id.lyttxtSurname);
            lyttxtSurname.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            lyttxtSurname.Hint = "Surname";
            txtSurname = FindViewById<CustomEditText>(Resource.Id.txtSurname);
            txtSurname.Text = User.Surname;
            txtSurname.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtSurname.Text))
                {
                    lyttxtSurname.Error = "";

                    if (txtSurname.HasFocus == false)
                    {
                        lyttxtSurname.Error = " ";
                    }
                }
            };

            lyttxtEmail = FindViewById<TextInputLayout>(Resource.Id.lyttxtEmail);
            lyttxtEmail.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            lyttxtEmail.Hint = "Email";
            txtEmail = FindViewById<CustomEditText>(Resource.Id.txtEmail);
            txtEmail.Text = User.LoginCredential;
            txtEmail.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtEmail.Text) || BackendProvider_Droid.IsValidEmail(txtEmail.Text) == false)
                {
                    lyttxtEmail.Error = "";

                    if (txtEmail.HasFocus == false)
                    {
                        lyttxtEmail.Error = " ";
                    }
                }
            };

            lyttxtMobile = FindViewById<TextInputLayout>(Resource.Id.lyttxtMobile);
            lyttxtMobile.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            lyttxtMobile.Hint = "Mobile";
            txtMobile = FindViewById<CustomEditText>(Resource.Id.txtMobile);
            txtMobile.Text = User.MobileNumber;
            txtMobile.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtMobile.Text))
                {
                    lyttxtMobile.Error = "";

                    if (txtMobile.HasFocus == false)
                    {
                        lyttxtMobile.Error = " ";
                    }
                }
            };

            btnCancel = FindViewById<CustomButton>(Resource.Id.btnCancel);
            btnCancel.LayoutParameters.Width = Utils.CalcDimension(245);
            btnCancel.LayoutParameters.Height = Utils.CalcDimension(75);
            btnCancel.Click += imgBack_Click;

            btnSave = FindViewById<CustomButton>(Resource.Id.btnSave);
            btnSave.LayoutParameters.Width = Utils.CalcDimension(245);
            btnSave.LayoutParameters.Height = Utils.CalcDimension(75);
            btnSave.Click += btnSave_Click;

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.TranslationY = Utils.CalcDimension(100);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), 0);
            lytNotification.Visibility = ViewStates.Gone;
            txtNotification = FindViewById<CustomTextView>(Resource.Id.txtNotification);

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            fadeInAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeInAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                lytNotification.Visibility = ViewStates.Gone;

                HideLoader();

                OnBackPressed();
            };

            HideLoader();
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "UserProfile");
            StartActivity(menu);
            Finish();
        }

        void imgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            ShowLoader();

            lyttxtName.Error = "";
            lyttxtSurname.Error = "";
            lyttxtEmail.Error = "";
            lyttxtMobile.Error = "";

            bool valid = true;

            var name = string.IsNullOrWhiteSpace(txtName.Text) ? string.Empty : txtName.Text.Trim();
            var surname = string.IsNullOrWhiteSpace(txtSurname.Text) ? string.Empty : txtSurname.Text.Trim();
            var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? string.Empty : txtEmail.Text.Trim();
            var mobile = string.IsNullOrWhiteSpace(txtMobile.Text) ? string.Empty : txtMobile.Text.Trim();
            
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

            if (string.IsNullOrEmpty(email) || BackendProvider_Droid.IsValidEmail(email) == false)
            {
                lyttxtEmail.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(mobile))
            {
                lyttxtMobile.Error = " ";
                valid = false;
            }

            if (valid)
            {
                var operation = await BackendProvider.UpdateProfile(MainApplication.ApiKey, User.UserId, User.Title, name, surname, email,  mobile);
                if (operation.Result == OperationResult.Failure)
                {
                    txtNotification.Text = "Oops! Something went wrong, please try again.";
                    lytNotification.Visibility = ViewStates.Visible;
                    await Task.Delay(2000);
                    lytNotification.StartAnimation(fadeInAnimation);

                    HideLoader();

                    return;
                }

                AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsEditSave.ToString());

                txtNotification.Text = "Your profile has been updated.";
                lytNotification.Visibility = ViewStates.Visible;
                lytNotification.SetBackgroundColor(Color.ParseColor("#3badb9"));
                lytNotification.LayoutParameters.Height = Utils.CalcDimension(85);
                await Task.Delay(2000);
                lytNotification.StartAnimation(fadeInAnimation);

                var userProfile = BackendProvider_Droid.GetUser;
                var userUpdate = new UserResponse()
                {
                    Name = name,
                    Surname = surname,
                    AccountsStatus = userProfile.AccountStatusID,
                    CountryID = userProfile.CountryId,
                    DateCreated = userProfile.DateCreated,
                    EmailEnc = userProfile.EmailEnc,
                    LoginCredential = email,
                    MobileNumber = mobile,
                    Title = userProfile.Title,
                    UserId = User.UserId
                };

                BackendProvider_Droid.SetUser(userUpdate, true, userProfile.Password);

                HideLoader();

                return;
            }

            HideLoader();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsEditBack.ToString());

            var intent = new Intent(this, typeof(UserProfile));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
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

