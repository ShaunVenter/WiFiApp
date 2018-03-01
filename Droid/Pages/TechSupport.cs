using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Telephony;
using Android.Graphics;
using XamSvg;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.AppCompat;
using AlwaysOn;
using AlwaysOn.Objects;
using Android.Views.Animations;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.TechSupport")]
    public class TechSupport : Activity
    {
        SvgImageView img;
        SvgImageView imgBack;
        SvgImageView imgMenu;
        CustomButton btnEmail;
        CustomButton btnCall;
        CustomTextView lblAppVersion;
        RelativeLayout lytContainer;

        LinearLayout lnrFacebook;
        LinearLayout lnrTwitter;
        LinearLayout lnrYoutube;

        RelativeLayout lytprogressBar;
        ProgressBar myProgressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.TechnicalSupport.ToString());

            SetContentView(Resource.Layout.TechSupport);

            if (BackendProvider_Droid.GetUser == null)
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
            imgBack.Click += (object sender, EventArgs e) =>
            {
                OnBackPressed();
            };

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);
            imgMenu.Click += (object sender, EventArgs e) =>
            {
                var menu = new Intent(this, typeof(Menu));
                menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                menu.PutExtra("prevPage", "TechSupport");
                StartActivity(menu);
                Finish();
            };

            lytContainer = FindViewById<RelativeLayout>(Resource.Id.lytContainer);
            lytContainer.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            lnrFacebook = FindViewById<LinearLayout>(Resource.Id.lnrFacebook);
            lnrFacebook.Click += delegate { StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://www.facebook.com/AlwaysOnWiFi/"))); };

            lnrTwitter = FindViewById<LinearLayout>(Resource.Id.lnrTwitter);
            lnrTwitter.Click += delegate { StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://twitter.com/alwaysonwifi"))); };

            lnrYoutube = FindViewById<LinearLayout>(Resource.Id.lnrYoutube);
            lnrYoutube.Click += delegate { StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://www.youtube.com/channel/UCUwaK59ri2_AY-6sQA1WERw"))); };

            btnEmail = FindViewById<CustomButton>(Resource.Id.btnEmail);
            btnEmail.LayoutParameters.Width = Utils.CalcDimension(245);
            btnEmail.LayoutParameters.Height = Utils.CalcDimension(75);
            btnEmail.Click += (object sender, EventArgs e) =>
            {
                try
                {
                    Intent emailIntent = new Intent(Intent.ActionSendto, Android.Net.Uri.FromParts("mailto", "support@alwayson.co.za", null));
                    emailIntent.PutExtra(Intent.ExtraSubject, "Subject");
                    emailIntent.PutExtra(Intent.ExtraText, "Body");
                    StartActivity(Intent.CreateChooser(emailIntent, "Send email..."));
                }
                catch { }
            };

            btnCall = FindViewById<CustomButton>(Resource.Id.btnCall);
            btnCall.LayoutParameters.Width = Utils.CalcDimension(245);
            btnCall.LayoutParameters.Height = Utils.CalcDimension(75);
            btnCall.Click += (object sender, EventArgs e) =>
            {
                Intent intent = new Intent(Intent.ActionDial, Android.Net.Uri.Parse("tel:" + "08614687768"));
                StartActivity(intent);
            };

            lblAppVersion = FindViewById<CustomTextView>(Resource.Id.lblAppVersion);
            lblAppVersion.Text = ConfigurationProvider.AppVersion;

            HideLoader();
        }
        
        public override void OnBackPressed()
        {
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

