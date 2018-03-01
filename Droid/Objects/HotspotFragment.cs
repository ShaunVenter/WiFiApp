using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using XamSvg;
using Fragment = Android.Support.V4.App.Fragment;
using AlwaysOn.Objects;
using System.Threading;

namespace AlwaysOn_Droid
{
    public class HotspotsFragment : Fragment
    {
        private const string KeyContent = "HotspotsFragment:Content";
        LinearLayout tabItem;
        Hotspot current;

        public static HotspotsFragment NewInstance()
        {
            return new HotspotsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            current = new Hotspot();

            ScrollView scroll = new ScrollView(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };

            tabItem = new LinearLayout(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };
            tabItem.Orientation = Orientation.Vertical;

            List<oAddress> addresses = new List<oAddress>();

            foreach (var item in HotspotFinder.Hotspots)
            {
                LinearLayout spotlItem = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
                };
                spotlItem.Orientation = Orientation.Horizontal;
                LinearLayout.LayoutParams paramsnew = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                paramsnew.SetMargins(0, Utils.CalcDimension(15), 0, 0);
                //paramsnew.Height = CalcNewHeight (90);
                spotlItem.LayoutParameters = paramsnew;
                spotlItem.SetBackgroundColor(Color.ParseColor("#3a3a3a"));
                spotlItem.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));

                LinearLayout textLayout = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                };
                textLayout.LayoutParameters.Width = Utils.CalcDimension(435);
                textLayout.Orientation = Orientation.Vertical;

                LinearLayout imageLayout = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                };
                //imageLayout.SetGravity(GravityFlags.Right);
                imageLayout.SetPadding(Utils.CalcDimension(10), 0, 0, 0);
                imageLayout.Orientation = Orientation.Vertical;

                //add text
                var text = new CustomTextView(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
                    Gravity = GravityFlags.Left,
                    Text = item.data,
                    TextSize = 14
                };
                text.SetTextColor(Color.White);
                text.SetCustomFont("Fonts/FocoCorp-Bold.ttf");

                var addressText = new CustomTextView(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
                    Gravity = GravityFlags.Left,
                    Text = item.address ?? "",
                    TextSize = 14
                };
                addressText.SetCustomFont("Fonts/FocoCorp-Regular.ttf");
                //Async
                if (string.IsNullOrEmpty(item.address))
                {
                    addresses.Add(new oAddress() { AddressTextView = addressText, lat = Double.Parse(item.lat), lng = Double.Parse(item.lng) });
                }

                var distanceText = new CustomTextView(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
                    Gravity = GravityFlags.Left,
                    Text = item.distance.ToString("0.00") + " km",
                    TextSize = 14
                };
                distanceText.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

                var image = new ImageView(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                    {
                        Width = Utils.CalcDimension(50)
                    }
                };
                image.SetImageResource(item.superwifi ? Resource.Drawable.S_30 : (item.international ? Resource.Drawable.I_30 : Resource.Drawable.B_30));

                imageLayout.AddView(image);

                textLayout.AddView(text);
                textLayout.AddView(addressText);
                textLayout.AddView(distanceText);

                spotlItem.AddView(textLayout);
                spotlItem.AddView(imageLayout);

                tabItem.AddView(spotlItem);
            }

            ReverseGeocodeCurrentLocation(Activity, addresses);

            scroll.AddView(tabItem);
            return scroll;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KeyContent, "");
        }

        public void ReverseGeocodeCurrentLocation(Context context, List<oAddress> addresses)
        {
            try
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    addresses.ForEach(n =>
                    {
                        try
                        {
                            var text = new Android.Locations.Geocoder(context).GetFromLocationAsync(n.lat, n.lng, 10).Result.Select(o => o.GetAddressLine(0) + " " + o.GetAddressLine(1)).FirstOrDefault() ?? "";
                            Activity.RunOnUiThread(() => { n.AddressTextView.Text = text; n.AddressTextView.Invalidate(); });
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }).Start();
            }
            catch (Exception ex)
            {
            }
        }

        public class oAddress
        {
            public CustomTextView AddressTextView { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }
    }
}