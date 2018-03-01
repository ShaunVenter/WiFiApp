using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.UserProfile")]
    public class UserProfile : Activity
    {
        SvgImageView img;
        SvgImageView imgBack;
        SvgImageView imgMenu;
        CustomTextView txtTitle;
        LinearLayout lytForm;
        RelativeLayout lytButtons;
        CustomTextView txtName;
        CustomTextView txtSurname;
        CustomTextView txtUserEmail;
        CustomTextView txtUserMobile;
        CustomButton btnSignOut;
        CustomButton btnEdit;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        UserObject User = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.ProfileDetails.ToString());

            SetContentView(Resource.Layout.UserProfile);

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
            imgBack.Click += btnCancel_Click;

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.Click += imgMenu_Click;
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);

            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);
            txtTitle.SetPadding(Utils.CalcDimension(70), 0, 0, Utils.CalcDimension(50));

            lytForm = FindViewById<LinearLayout>(Resource.Id.lytForm);
            lytForm.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            txtName = FindViewById<CustomTextView>(Resource.Id.txtName);
            txtName.SetPadding(0, 0, 0, Utils.CalcDimension(40));
            txtName.Text = User.Name;

            txtSurname = FindViewById<CustomTextView>(Resource.Id.txtSurname);
            txtSurname.SetPadding(0, 0, 0, Utils.CalcDimension(40));
            txtSurname.Text = User.Surname;

            txtUserEmail = FindViewById<CustomTextView>(Resource.Id.txtEmail);
            txtUserEmail.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            txtUserEmail.Text = User.LoginCredential;

            txtUserMobile = FindViewById<CustomTextView>(Resource.Id.txtMobile);
            txtUserMobile.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            //string userMobile = Intent.GetStringExtra("userMobile") ?? "0";
            txtUserMobile.Text = User.MobileNumber;

            lytButtons = FindViewById<RelativeLayout>(Resource.Id.lytButtons);
            lytButtons.SetPadding(Utils.CalcDimension(70), 0, Utils.CalcDimension(70), 0);

            btnSignOut = FindViewById<CustomButton>(Resource.Id.btnSignOut);
            btnSignOut.LayoutParameters.Width = Utils.CalcDimension(245);
            btnSignOut.LayoutParameters.Height = Utils.CalcDimension(75);
            btnSignOut.Click += BtnSignOut_Click; ;

            btnEdit = FindViewById<CustomButton>(Resource.Id.btnEdit);
            btnEdit.LayoutParameters.Width = Utils.CalcDimension(245);
            btnEdit.LayoutParameters.Height = Utils.CalcDimension(75);
            
            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            btnEdit.Click += btnEdit_Click;

            HideLoader();
        }

        void BtnSignOut_Click(object sender, EventArgs e)
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsSignout.ToString());

            BackendProvider_Droid.ClearSettings();
            
            var login = new Intent(this, typeof(Login));
            login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(login);

            ActivityCompat.FinishAffinity(this);
            Finish();
        }
        
        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "UserProfile");
            StartActivity(menu);
            Finish();
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsEdit.ToString());

            var intent = new Intent(this, typeof(UserProfile2));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsBack.ToString());

            BackendProvider_Droid.UpdatePackagesFromServer();
            var intent = new Intent(this, typeof(Dashboard));
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