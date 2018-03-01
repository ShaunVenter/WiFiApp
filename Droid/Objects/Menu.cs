using AlwaysOn;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Menu")]
    public class Menu : Activity
    {
        SvgImageView img, imgClose;
        RelativeLayout lytSignInDetails;
        LinearLayout lytMain;
        CustomTextView txtSigninas, txtUserName;
        CustomButton btnSignOut, btnDashboard, btnLinkPackage, btnBuyPackage, btnHotspot, btnProfile, btnSettings, btnTechnical, btnTerms, btnLinkAccessCode;

        enum MenuButtons { Close = 0, SignOut = 1, Dashboard = 2, LinkPackage = 3, BuyPackage = 4, HotspotFinder = 5, UserProfile = 6, Settings = 7, TechnicalHelp = 8, Terms = 9, LinkAccessCode = 10 };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu);

            var User = BackendProvider_Droid.GetUser;
            if (User == null)
            {
                var login = new Intent(this, typeof(Login));
                login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(login);
                Finish();
                return;
            }

            lytMain = FindViewById<LinearLayout>(Resource.Id.lytMain);

            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            imgClose = FindViewById<SvgImageView>(Resource.Id.imgClose);
            imgClose.SetSvg(this, Resource.Raw.AO__close_icn, "ffffff=632e8e");
            imgClose.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgClose.LayoutParameters.Width = Utils.CalcDimension(125);
            imgClose.Click += delegate { ClickHandler(MenuButtons.Close); };

            lytSignInDetails = FindViewById<RelativeLayout>(Resource.Id.lytSignInDetails);
            lytSignInDetails.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            lytSignInDetails.LayoutParameters.Height = Utils.CalcDimension(110);

            txtSigninas = FindViewById<CustomTextView>(Resource.Id.txtSigninas);

            txtUserName = FindViewById<CustomTextView>(Resource.Id.txtUserName);
            txtUserName.Text = User.Name + " " + User.Surname;

            btnSignOut = FindViewById<CustomButton>(Resource.Id.btnSignOut);
            btnSignOut.Click += delegate { ClickHandler(MenuButtons.SignOut); };

            var paramsnew = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            {
                Height = Utils.CalcDimension(90),
                TopMargin = Utils.CalcDimension(15)
            };

            btnDashboard = FindViewById<CustomButton>(Resource.Id.btnDashboard);
            btnDashboard.Click += delegate { ClickHandler(MenuButtons.Dashboard); };

            btnBuyPackage = FindViewById<CustomButton>(Resource.Id.btnBuyPackage);
            btnBuyPackage.LayoutParameters = paramsnew;
            btnBuyPackage.Click += delegate { ClickHandler(MenuButtons.BuyPackage); };

            btnLinkPackage = FindViewById<CustomButton>(Resource.Id.btnLinkPackage);
            btnLinkPackage.LayoutParameters = paramsnew;
            btnLinkPackage.Click += delegate { ClickHandler(MenuButtons.LinkPackage); };

            btnLinkAccessCode = FindViewById<CustomButton>(Resource.Id.btnLinkAccessCode);
            btnLinkAccessCode.LayoutParameters = paramsnew;
            btnLinkAccessCode.Click += delegate { ClickHandler(MenuButtons.LinkAccessCode); };

            btnHotspot = FindViewById<CustomButton>(Resource.Id.btnHotspot);
            btnHotspot.LayoutParameters = paramsnew;
            btnHotspot.Click += delegate { ClickHandler(MenuButtons.HotspotFinder); };

            btnProfile = FindViewById<CustomButton>(Resource.Id.btnProfile);
            btnProfile.LayoutParameters = paramsnew;
            btnProfile.Click += delegate { ClickHandler(MenuButtons.UserProfile); };

            btnSettings = FindViewById<CustomButton>(Resource.Id.btnSettings);
            btnSettings.LayoutParameters = paramsnew;
            btnSettings.Click += delegate { ClickHandler(MenuButtons.Settings); };

            btnTechnical = FindViewById<CustomButton>(Resource.Id.btnTechnical);
            btnTechnical.LayoutParameters = paramsnew;
            btnTechnical.Click += delegate { ClickHandler(MenuButtons.TechnicalHelp); };

            btnTerms = FindViewById<CustomButton>(Resource.Id.btnTerms);
            btnTerms.LayoutParameters = paramsnew;
            btnTerms.Click += delegate { ClickHandler(MenuButtons.Terms); };

            lytMain.StartAnimation(new TranslateAnimation(Resources.DisplayMetrics.WidthPixels, 0, 0, 0) { Duration = 700 });
        }

        private Type GetPreviousClass(string PreviousClass)
        {
            switch (PreviousClass)
            {
                case "Dashboard": { return typeof(Dashboard); }
                case "Purchase": { return typeof(Purchase); }
                case "PurchaseConfirmation": { return typeof(PurchaseConfirmation); }
                case "BuyPackage": { return typeof(BuyPackage); }
                case "HotspotFinder": { return typeof(HotspotFinder); }
                case "Maps": { return typeof(Maps); }
                case "Settings": { return typeof(Settings); }
                case "UserProfile": { return typeof(UserProfile); }
                case "UserProfile2": { return typeof(UserProfile2); }
                case "TechSupport": { return typeof(TechSupport); }
                default: return null;
            }
        }

        private void ClickHandler(MenuButtons Button)
        {
            Type gotoClass = GetPreviousClass(Intent.GetStringExtra("prevPage"));
            bool gotoPage = true;

            switch (Button)
            {
                case MenuButtons.Close:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuClose.ToString());
                        break;
                    }
                case MenuButtons.SignOut:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuSignout.ToString());
                        BackendProvider_Droid.ClearSettings(); gotoClass = typeof(Login);
                        break;
                    }
                case MenuButtons.Dashboard:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuDashboard.ToString());
                        BackendProvider_Droid.UpdatePackagesFromServer(); gotoClass = typeof(Dashboard);
                        break;
                    }
                case MenuButtons.LinkPackage:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuLinkPackage.ToString());
                        gotoClass = typeof(LinkPackage);
                        break;
                    }
                case MenuButtons.LinkAccessCode:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuLinkAccessPackage.ToString());
                        gotoClass = typeof(LinkAccessCode);
                        break;
                    }
                case MenuButtons.BuyPackage:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuBuyPackage.ToString());
                        gotoClass = typeof(BuyPackage);
                        break;
                    }
                case MenuButtons.HotspotFinder:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuHotspotFinder.ToString());
                        gotoClass = typeof(HotspotFinder);
                        break;
                    }
                case MenuButtons.UserProfile:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuProfileDetails.ToString());
                        gotoClass = typeof(UserProfile);
                        break;
                    }
                case MenuButtons.Settings:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuSettings.ToString());
                        gotoClass = typeof(Settings);
                        break;
                    }
                case MenuButtons.TechnicalHelp:
                    {
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuTechnicalSupport.ToString());
                        gotoClass = typeof(TechSupport);
                        break;
                    }
                case MenuButtons.Terms:
                    {
                        gotoPage = false;

                        AnalyticsProvider_Droid.PageViewGA(this, PageName.TermsAndConditions.ToString());
                        AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.MenuTermsConditions.ToString());

                        var intent = new Intent(this, gotoClass);
                        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        StartActivity(intent);

                        var termsIntent = new Intent();
                        termsIntent.SetAction(Intent.ActionView);
                        termsIntent.AddCategory(Intent.CategoryBrowsable);
                        termsIntent.SetData(Android.Net.Uri.Parse("https://www.alwayson.co.za/terms.aspx"));
                        termsIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        StartActivity(termsIntent);

                        break;
                    }
            }

            if (gotoPage)
            {
                var intent = new Intent(this, gotoClass);
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
            }

            Finish();
        }
    }
}