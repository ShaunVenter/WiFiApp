
using System;
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
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.ForgotPassword")]
    public class ForgotPassword : Activity
    {
        SvgImageView img;
        SvgImageView imgBack;

        RelativeLayout lytContainer;
        CustomButton btnCancel;
        CustomButton btnRecover;
        TextInputLayout lyttxtPhone;
        CustomEditText txtPhone;
        TextInputLayout lyttxtEmail;
        CustomEditText txtEmail;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        LinearLayout lytNotification;
        CustomTextView txtNotification;
        Animation fadeOutAnimation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.ForgotPassword.ToString());

            SetContentView(Resource.Layout.ForgotPassword);

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

            lyttxtPhone = FindViewById<TextInputLayout>(Resource.Id.lyttxtPhone);
            lyttxtPhone.Hint = "Phone number";
            lyttxtPhone.SetPadding(0, Utils.CalcDimension(40), 0, 0);
            txtPhone = FindViewById<CustomEditText>(Resource.Id.txtPhone);

            lyttxtEmail = FindViewById<TextInputLayout>(Resource.Id.lyttxtEmail);
            lyttxtEmail.Hint = "Email address";
            lyttxtEmail.SetPadding(0, Utils.CalcDimension(40), 0, 0);
            txtEmail = FindViewById<CustomEditText>(Resource.Id.txtEmail);

            btnRecover = FindViewById<CustomButton>(Resource.Id.btnRecover);
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
            txtNotification = FindViewById<CustomTextView>(Resource.Id.txtNotification);

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            fadeOutAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeOutAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                txtNotification.Text = "";
                lytNotification.Visibility = ViewStates.Gone;
                
                HideLoader();

                OnBackPressed();
            };
            
            HideLoader();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            ShowLoader();

            lyttxtPhone.Error = "";

            bool valid = true;

            string mobile = string.IsNullOrWhiteSpace(txtPhone.Text) ? string.Empty : txtPhone.Text.Trim();
            var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? string.Empty : txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(mobile))
            {
                lyttxtPhone.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(email) || BackendProvider_Droid.IsValidEmail(email) == false)
            {
                lyttxtEmail.Error = " ";
                valid = false;
            }

            if (valid)
            {
                var operation = await BackendProvider.ResetPassword(MainApplication.ApiKey, email, mobile);
                if (operation.Result == OperationResult.Failure)
                {
                    txtNotification.Text = "Oops! Something went wrong, please try again.";
                    lytNotification.Visibility = ViewStates.Visible;
                    lytNotification.StartAnimation(fadeOutAnimation);

                    HideLoader();

                    return;
                }
                else
                {
                    txtNotification.Text = "A password link with your credentials has been emailed and sms'd to you";
                    lytNotification.Visibility = ViewStates.Visible;
                    lytNotification.LayoutParameters.Height = Utils.CalcDimension(120);
                    lytNotification.SetBackgroundColor(Color.ParseColor("#3badb9"));

                    fadeOutAnimation.StartOffset = 4000;
                    lytNotification.StartAnimation(fadeOutAnimation);

                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ForgotPasswordRecover.ToString());

                    HideLoader();

                    return;
                }
            }

            HideLoader();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ForgotPasswordCancel.ToString());

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

