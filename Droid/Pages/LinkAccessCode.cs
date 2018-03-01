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
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.LinkAccessCode")]
    public class LinkAccessCode : Activity
    {
        SvgImageView img;
        RelativeLayout lytContainer;
        CustomButton btnCancel;
        CustomButton btnRecover;

        TextInputLayout lyttxtAccessCode;
        CustomEditText txtAccessCode;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        LinearLayout lytNotification;
        CustomTextView txtNotification;
        Animation fadeInAnimation;
        LinearLayout lytSpinner;
        Spinner spinnerServiceProvider;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.LinkAccessCode.ToString());

            SetContentView(Resource.Layout.LinkAccessCode);

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

            lyttxtAccessCode = FindViewById<TextInputLayout>(Resource.Id.lyttxtAccessCode);
            lyttxtAccessCode.Hint = "Prepaid Pin";
            lyttxtAccessCode.SetPadding(0, Utils.CalcDimension(100), 0, 0);

            txtAccessCode = FindViewById<CustomEditText>(Resource.Id.txtAccessCode);
            //txtAccessCode.Hint = "00000000";

            /*lytSpinner = FindViewById<LinearLayout>(Resource.Id.lytSpinner);
            lytSpinner.SetPadding(0, Utils.CalcDimension(50), 0, 0);
            spinnerServiceProvider = FindViewById<Spinner>(Resource.Id.spinner2);
            spinnerServiceProvider.LayoutParameters.Height = Utils.CalcDimension(50);
            spinnerServiceProvider.SetSelection(0);*/

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
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkAccessPackageCancel.ToString());

            BackendProvider_Droid.UpdatePackagesFromServer();
            var intent = new Intent(this, typeof(Dashboard));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            ShowLoader();

            bool valid = true;

            var code = string.IsNullOrWhiteSpace(txtAccessCode.Text) ? string.Empty : txtAccessCode.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                lyttxtAccessCode.Error = " ";
                valid = false;
            }

            if (valid)
            {
                var operation = await BackendProvider.LinkAccessCode(MainApplication.ApiKey, BackendProvider_Droid.GetUser.UserId, code);
                if (operation.Result == OperationResult.Failure)
                {
                    txtNotification.Text = "Oops! Something went wrong, please check your access code and try again.";
                    lytNotification.Visibility = ViewStates.Visible;
                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(120);
                    lytNotification.StartAnimation(fadeInAnimation);
                }
                else
                {
                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.LinkAccessPackageLink.ToString());

                    txtNotification.Text = "Your Prepaid Pin was linked succesfully.";
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