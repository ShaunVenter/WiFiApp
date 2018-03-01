
using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.BuyPackage")]
    public class BuyPackage : FragmentActivity
    {
        RelativeLayout lytMain;
        SvgImageView img;
        SvgImageView imgBack;
        SvgImageView imgMenu;

        protected TabFragmentAdapter _adapter;
        protected ViewPager _pager;
        protected TabLayout _indicator;


        List<VoucherPackage> listData = new List<VoucherPackage>();
        List<VoucherPackage> listTime = new List<VoucherPackage>();
        List<VoucherPackage> listSubscription = new List<VoucherPackage>();
        List<VoucherPackage> _packages;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        LinearLayout lytNotification;
        CustomTextView txtNotification;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.BuyPackage.ToString());

            SetContentView(Resource.Layout.BuyPackage);
            lytMain = FindViewById<RelativeLayout>(Resource.Id.lytMain);

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

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.Click += imgMenu_Click;
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(10), Utils.CalcDimension(70), Utils.CalcDimension(10));
            lytNotification.Visibility = ViewStates.Gone;
            txtNotification = new CustomTextView(this)
            {
                Gravity = GravityFlags.Left,
                Text = "",
                TextSize = 14
            };
            lytNotification.AddView(txtNotification);
            lytNotification.LayoutParameters.Height = Utils.CalcDimension(120);
            txtNotification.LayoutParameters.Height = LinearLayout.LayoutParams.MatchParent;

            _packages = GetVoucherPackages();
            if (_packages != null && _packages.Count > 0)
            {
                foreach (var item in _packages)
                {
                    if (item.packageType.Equals("DT"))
                    {
                        listData.Add(item);
                    }

                    if (item.packageType.Equals("T"))
                    {
                        listTime.Add(item);
                    }

                    if (item.packageType.Equals("S"))
                    {
                        listSubscription.Add(item);
                    }
                }
            }
            else
            {
                txtNotification.Text = "Could not retrieve available packages";
                lytNotification.Visibility = ViewStates.Visible;
            }
            List<List<VoucherPackage>> outerList = new List<List<VoucherPackage>>();
            if (listData.Count > 0) outerList.Add(listData);
            if (listTime.Count > 0) outerList.Add(listTime);
            if (listSubscription.Count > 0) outerList.Add(listSubscription);

            _adapter = new TabFragmentAdapter(SupportFragmentManager, outerList);

            _pager = FindViewById<ViewPager>(Resource.Id.viewpager);
            _pager.Adapter = _adapter;

            _indicator = FindViewById<TabLayout>(Resource.Id.tabs);
            _indicator.SetupWithViewPager(_pager);
            _indicator.LayoutParameters.Height = Utils.CalcDimension(95);
            
            imgBack.Click += imgBack_Click;

            HideLoader();
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "BuyPackage");
            StartActivity(menu);
            Finish();
        }
        
        private List<VoucherPackage> GetVoucherPackages()
        {
            using (var client = new HotspotHelperWebClient())
            {
                client.Timeout = 8000;

                try
                {
                    var text = client.UploadString(ConfigurationProvider.GetVoucherPackagesUrl + "?api_key=" + MainApplication.ApiKey, "api_key=" + MainApplication.ApiKey);
                    var packages = new VoucherPackageResponse(text);
                    if (packages.Success)
                    {
                        return packages.PackageList ?? new List<VoucherPackage>();
                    }
                }
                catch (Exception ex) { var exception = ex; }

                return new List<VoucherPackage>();
            }
        }

        void imgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        void _pager_OnFocusChangeListener(object sender, EventArgs e)
        {
            //jkljklkl
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.BuyPackageBack.ToString());

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

