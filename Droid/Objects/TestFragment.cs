using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using XamSvg;
using Fragment = Android.Support.V4.App.Fragment;
using AlwaysOn;

namespace AlwaysOn_Droid
{
    public class TestFragment : Fragment
    {
        private const string KeyContent = "TestFragment:Content";
        private List<KeyValuePair<string, string>> _content;

        public static TestFragment NewInstance(List<KeyValuePair<string, string>> iy)
        {
            var fragment = new TestFragment();
            fragment._content = iy;

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((savedInstanceState != null) && savedInstanceState.ContainsKey(KeyContent))
                _content = null;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var text = new CustomTextView(Activity)
            {
                Gravity = GravityFlags.Center,
                Text = _content[0].Value.ToString(),
                TextSize = 24,
            };
            text.SetTextColor(Color.ParseColor("#ffffff"));
            text.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(70), Utils.CalcDimension(70), 0);
            text.TextAlignment = TextAlignment.ViewStart;
            text.SetCustomFont("Fonts/FocoCorp-Bold.ttf");

            var subText = new CustomTextView(Activity)
            {
                Gravity = GravityFlags.Center,
                Text = _content[1].Value.ToString(),
                TextSize = 16
            };
            subText.SetTextColor(Color.ParseColor("#ffffff"));
            subText.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), 0);
            subText.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

            var image = new SvgImageView(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            };
            image.SetSvg(Activity, Convert.ToInt32(_content[3].Value));
            image.LayoutParameters.Width = Convert.ToInt32(_content[6].Value);
            image.SetPadding(Convert.ToInt32(_content[4].Value), Convert.ToInt32(_content[5].Value), Convert.ToInt32(_content[4].Value), Utils.CalcDimension(40));

            LinearLayout buttonLayout = new LinearLayout(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            buttonLayout.SetGravity(GravityFlags.CenterHorizontal);
            var button = new CustomButton(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(Utils.CalcDimension(300), Utils.CalcDimension(80)),
                Text = "GET STARTED",
                Gravity = GravityFlags.Center
            };
            button.TextSize = 15;
            button.SetPadding(0, 0, 0, 0);
            button.SetBackgroundResource(Resource.Drawable.GreenButton_Ripple);
            button.SetCustomFont("Fonts/FocoCorp-Bold.ttf");
            button.SetTextColor(Color.White);
            buttonLayout.AddView(button);

            LinearLayout carouselItem = new LinearLayout(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };

            carouselItem.Orientation = Orientation.Vertical;
            //carouselItem.SetGravity(GravityFlags.Left);
            //carouselItem.AddView(text);
            //carouselItem.AddView(subText);
            carouselItem.SetBackgroundColor(Color.ParseColor(_content[2].Value));
            carouselItem.AddView(image);

            if (Convert.ToBoolean(_content[7].Value))
            {
                AnalyticsProvider_Droid.TrackEventGA((Carousel)Context, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.IntroCarouselGetStarted.ToString());

                carouselItem.AddView(buttonLayout);
                button.Click += ((Carousel)Context).skip_Click;
            }

            return carouselItem;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KeyContent, "");
        }
    }
}