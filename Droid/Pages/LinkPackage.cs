
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using XamSvg;
using AlwaysOn;
using AlwaysOn.Objects;
using Android.Views.Animations;
using Android.Support.Design.Widget;
using Android.Support.V7.AppCompat;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.LinkPackage")]
    public class LinkPackage : Activity
    {
        SvgImageView img;
        RelativeLayout lytContainer;
        CustomButton btnCancel;
        CustomButton btnRecover;
        TextInputLayout lyttxtPassword;
        CustomEditText txtPassword;
        TextInputLayout lyttxtUsername;
        CustomEditText txtUsername;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        LinearLayout lytNotification;
        CustomTextView txtNotification;
        Animation fadeInAnimation;
        LinearLayout lytSpinner;
        Spinner spinnerServiceProvider;
        List<ServiceProvider> serviceProviders;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.LinkPackage.ToString());

            SetContentView(Resource.Layout.LinkPackage);

            if (BackendProvider_Droid.GetUser == null)
            {
                var login = new Intent(this, typeof(Login));
                login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(login);
                Finish();
                return;
            }

            InitializeLayout();
        }

        private async void InitializeLayout()
        {
            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            lytContainer = FindViewById<RelativeLayout>(Resource.Id.lytContainer);
            lytContainer.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            lyttxtPassword = FindViewById<TextInputLayout>(Resource.Id.lyttxtPassword);
            lyttxtPassword.Hint = "Password";
            lyttxtPassword.SetPadding(0, Utils.CalcDimension(50), 0, 0);
            txtPassword = FindViewById<CustomEditText>(Resource.Id.txtPassword);

            lyttxtUsername = FindViewById<TextInputLayout>(Resource.Id.lyttxtUsername);
            lyttxtUsername.Hint = "Username";
            lyttxtUsername.SetPadding(0, Utils.CalcDimension(100), 0, 0);
            txtUsername = FindViewById<CustomEditText>(Resource.Id.txtUsername);

            lytSpinner = FindViewById<LinearLayout>(Resource.Id.lytSpinner);
            lytSpinner.SetPadding(0, Utils.CalcDimension(50), 0, 0);
            spinnerServiceProvider = FindViewById<Spinner>(Resource.Id.spinner2);
            spinnerServiceProvider.LayoutParameters.Height = Utils.CalcDimension(50);
            spinnerServiceProvider.SetSelection(0);

            serviceProviders = new List<ServiceProvider>();
            var sp = BackendProvider_Droid.GetStoredServiceProviders();
            if (sp.Result == OperationResult.Success && ((ServiceProviders)sp.Response).Items?.Count > 0)
            {
                serviceProviders = ((ServiceProviders)sp.Response).Items;
            }
            else
            {
                serviceProviders.Add(new ServiceProvider("1", "AlwaysOn") { });
            }

            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, serviceProviders.Select(n => n.Name).ToArray());
            spinnerServiceProvider.Adapter = adapter;

            btnRecover = FindViewById<CustomButton>(Resource.Id.btnLinkPackage);
            btnRecover.LayoutParameters.Width = Utils.CalcDimension(245);
            btnRecover.LayoutParameters.Height = Utils.CalcDimension(75);
            btnRecover.Click += BtnSubmit_Click;

            btnCancel = FindViewById<CustomButton>(Resource.Id.btnCancel);
            btnCancel.LayoutParameters.Width = Utils.CalcDimension(245);
            btnCancel.LayoutParameters.Height = Utils.CalcDimension(75);
            btnCancel.Click += btnCancel_Click;

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.TranslationY = Utils.CalcDimension(100);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), 0);
            lytNotification.Visibility = ViewStates.Gone;
            lytNotification.LayoutParameters.Height = LinearLayout.LayoutParams.WrapContent;
            txtNotification = FindViewById<CustomTextView>(Resource.Id.txtNotification);
            txtNotification.LayoutParameters.Height = LinearLayout.LayoutParams.MatchParent;

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            fadeInAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeInAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                txtNotification.Text = "";
                lytNotification.Visibility = ViewStates.Gone;

                HideLoader();
            };

            HideLoader();
        }
        
        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkPackageCancel.ToString());

            BackendProvider_Droid.UpdatePackagesFromServer();
            var intent = new Intent(this, typeof(Dashboard));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            ShowLoader();

            lyttxtPassword.Error = "";

            bool valid = true;

            string password = string.IsNullOrWhiteSpace(txtPassword.Text) ? string.Empty : txtPassword.Text.Trim();
            var username = string.IsNullOrWhiteSpace(txtUsername.Text) ? string.Empty : txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(password))
            {
                lyttxtPassword.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(username))
            {
                lyttxtUsername.Error = " ";
                valid = false;
            }

            var serviceProviderID = serviceProviders[spinnerServiceProvider.SelectedItemPosition].Id;

            if (valid)
            {
                var operation = await BackendProvider.LinkPurchasedPackage(MainApplication.ApiKey, BackendProvider_Droid.GetUser.UserId, username, password, serviceProviderID);
                if (operation.Result == OperationResult.Failure)
                {
                    txtNotification.Text = "Oops! Something went wrong, please check your credentials and try again.";
                    lytNotification.Visibility = ViewStates.Visible;
                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(120);
                    lytNotification.StartAnimation(fadeInAnimation);
                }
                else
                {
                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkPackageLink.ToString());

                    txtNotification.Text = "Your voucher was linked succesfully.";
                    lytNotification.Visibility = ViewStates.Visible;
                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(120);
                    lytNotification.SetBackgroundColor(Color.ParseColor("#33babc"));
                    lytNotification.StartAnimation(fadeInAnimation);

                    fadeInAnimation.AnimationEnd += (object sender1, Animation.AnimationEndEventArgs e1) =>
                    {
                        BackendProvider_Droid.UpdatePackagesFromServer();
                        var dashboard = new Intent(this, typeof(Dashboard));
                        dashboard.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        StartActivity(dashboard);
                        Finish();
                    };
                }
            }
            else
            {
                HideLoader();
            }
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