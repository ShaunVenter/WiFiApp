using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Support.V4.View;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Content;
using AlwaysOn;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Android.Views.Animations;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Carousel")]
    public class Carousel : FragmentActivity
    {
        protected TestFragmentAdapter _adapter;
        protected ViewPager _pager;
        protected CirclePageIndicator _indicator;
        SvgImageView img;
        SvgImageView imgLeftArrow;
        SvgImageView imgRightArrow;
        LinearLayout lytSkipLayout;
        LinearLayout lytSkipLine;

        List<KeyValuePair<string, string>> carouselItem1;
        List<KeyValuePair<string, string>> carouselItem2;
        List<KeyValuePair<string, string>> carouselItem3;

        LinearLayout lytBottom;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.IntroCarousel.ToString());

            XamSvg.Setup.InitSvgLib();
            SetContentView(Resource.Layout.Carousel);
            
            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);

            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            carouselItem1 = new List<KeyValuePair<string, string>>();
            carouselItem1.Add(new KeyValuePair<string, string>("Title", "WE DON'T DO SLOW"));
            carouselItem1.Add(new KeyValuePair<string, string>("subTitle", "Super fast. Super afforable. Super WiFi."));
            carouselItem1.Add(new KeyValuePair<string, string>("backgroundColor", "#632E8E"));
            carouselItem1.Add(new KeyValuePair<string, string>("resourceID", Resource.Raw.Intro_03.ToString()));
            carouselItem1.Add(new KeyValuePair<string, string>("imagePaddingLeftRight", Utils.CalcDimensionRatio(55).ToString())); //125
            carouselItem1.Add(new KeyValuePair<string, string>("imagePaddingTop", Utils.CalcDimensionRatio(41).ToString())); //150
            carouselItem1.Add(new KeyValuePair<string, string>("imageWidth", Utils.CalcDimensionRatio(349).ToString())); //550
            carouselItem1.Add(new KeyValuePair<string, string>("lastSlide", "false"));

            carouselItem2 = new List<KeyValuePair<string, string>>();
            carouselItem2.Add(new KeyValuePair<string, string>("Title", "CONNECT ANYWHERE"));
            carouselItem2.Add(new KeyValuePair<string, string>("subTitle", "Find one of our 1800+ hotspots instantly and get online faster than ever."));
            carouselItem2.Add(new KeyValuePair<string, string>("backgroundColor", "#632E8E"));
            carouselItem2.Add(new KeyValuePair<string, string>("resourceID", Resource.Raw.Intro_02.ToString()));
            carouselItem2.Add(new KeyValuePair<string, string>("imagePaddingLeftRight", Utils.CalcDimensionRatio(55).ToString())); //100
            carouselItem2.Add(new KeyValuePair<string, string>("imagePaddingTop", Utils.CalcDimensionRatio(38).ToString())); //140
            carouselItem2.Add(new KeyValuePair<string, string>("imageWidth", Utils.CalcDimensionRatio(349).ToString())); //550
            carouselItem2.Add(new KeyValuePair<string, string>("lastSlide", "false"));

            carouselItem3 = new List<KeyValuePair<string, string>>();
            carouselItem3.Add(new KeyValuePair<string, string>("Title", "HOW MUCH HAVE I? ...\nOH, THAT'S EASY."));
            carouselItem3.Add(new KeyValuePair<string, string>("subTitle", "No star one hundred hashing,\njust instant access to your balance."));
            carouselItem3.Add(new KeyValuePair<string, string>("backgroundColor", "#632E8E"));
            carouselItem3.Add(new KeyValuePair<string, string>("resourceID", Resource.Raw.Intro_01.ToString()));
            carouselItem3.Add(new KeyValuePair<string, string>("imagePaddingLeftRight", Utils.CalcDimensionRatio(55).ToString())); //120
            carouselItem3.Add(new KeyValuePair<string, string>("imagePaddingTop", Utils.CalcDimensionRatio(44).ToString())); //160
            carouselItem3.Add(new KeyValuePair<string, string>("imageWidth", Utils.CalcDimensionRatio(349).ToString())); //550
            carouselItem3.Add(new KeyValuePair<string, string>("lastSlide", "true"));

            List<List<KeyValuePair<string, string>>> outerList = new List<List<KeyValuePair<string, string>>>();
            outerList.Add(carouselItem1);
            outerList.Add(carouselItem2);
            outerList.Add(carouselItem3);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230); //230

            imgRightArrow = FindViewById<SvgImageView>(Resource.Id.imgRightArrow);
            imgRightArrow.SetSvg(this, Resource.Raw.AO__next_btn);
            imgRightArrow.LayoutParameters.Width = Utils.CalcDimensionRatio(10); //15
            imgRightArrow.Click += imgRightArrow_Click;
            imgLeftArrow = FindViewById<SvgImageView>(Resource.Id.imgLeftArrow);
            imgLeftArrow.SetSvg(this, Resource.Raw.AO__rev_btn);
            imgLeftArrow.LayoutParameters.Width = Utils.CalcDimensionRatio(10); //15
            imgLeftArrow.Click += imgLeftArrow_Click;

            lytSkipLayout = FindViewById<LinearLayout>(Resource.Id.lytSkipLayout);
            lytSkipLayout.SetPadding(0, 0, Utils.CalcDimensionRatio(45), 0); //70

            lytSkipLine = FindViewById<LinearLayout>(Resource.Id.lytSkipLine);
            lytSkipLine.LayoutParameters.Width = Utils.CalcDimensionRatio(50); //120
            lytSkipLine.LayoutParameters.Height = Utils.CalcDimensionRatio(2); //3

            var skip = FindViewById<CustomButton>(Resource.Id.txtSkip);
            skip.TextSize = Utils.CalcFontRatio(14);
            skip.Click += skip_Click;
            skip.LayoutParameters.Width = lytSkipLine.LayoutParameters.Width;

            _adapter = new TestFragmentAdapter(SupportFragmentManager, outerList);

            _pager = FindViewById<ViewPager>(Resource.Id.pager);
            _pager.Adapter = _adapter;

            lytBottom = FindViewById<LinearLayout>(Resource.Id.lytBottomLayout);
            lytBottom.SetPadding(Utils.CalcDimensionRatio(45), Utils.CalcDimensionRatio(26), Utils.CalcDimensionRatio(45), Utils.CalcDimensionRatio(20));

            _indicator = FindViewById<CirclePageIndicator>(Resource.Id.indicator);
            //_indicator.ScrollChange += _indicator_ScrollChange;

            _indicator.SetViewPager(_pager);
            _indicator.PageColor = Color.White;
            _indicator.PageColor = Color.White;
            _indicator.Radius = Utils.CalcDimensionRatio(10);
            _indicator.SetPadding(Utils.CalcDimensionRatio(26), Utils.CalcDimensionRatio(2), Utils.CalcDimensionRatio(26), Utils.CalcDimensionRatio(2));
        }

        private void _indicator_ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            _pager.SetCurrentItem(_pager.CurrentItem + 1, true);
            if (_pager.CurrentItem == 2)
            {
                imgRightArrow.Visibility = ViewStates.Invisible;
            }
        }

        private void _indicator_PageScrolled(object sender, PageScrolledEventArgs args)
        {
            _pager.SetCurrentItem(_pager.CurrentItem + 1, true);
            if (_pager.CurrentItem == 2)
            {
                imgRightArrow.Visibility = ViewStates.Invisible;
            }
        }

        void imgRightArrow_Click(object sender, EventArgs e)
        {
            imgLeftArrow.Visibility = ViewStates.Visible;
            _pager.SetCurrentItem(_pager.CurrentItem + 1, true);
            if (_pager.CurrentItem == 2)
            {
                imgRightArrow.Visibility = ViewStates.Invisible;
            }
        }
        void imgLeftArrow_Click(object sender, EventArgs e)
        {
            imgRightArrow.Visibility = ViewStates.Visible;
            _pager.SetCurrentItem(_pager.CurrentItem - 1, true);
            if (_pager.CurrentItem == 0)
            {
                imgLeftArrow.Visibility = ViewStates.Invisible;
            }
        }

        public void skip_Click(object sender, EventArgs e)
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.IntroCarouselSkipIntro.ToString());

            var login = new Intent(this, typeof(Login));
            login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(login);
            Finish();
        }
    }
}