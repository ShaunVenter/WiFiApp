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
namespace AlwaysOn_Droid
{
    public class SuperHotspotFragment : Fragment
    {
        private const string KeyContent = "SuperHotspotsFragment:Content";
        LinearLayout tabItem;
        Hotspot current;

        public static SuperHotspotFragment NewInstance()
        {
            return new SuperHotspotFragment();
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

            foreach (var item in HotspotFinder.Hotspots)
            {
                if ((bool)item.superwifi == true)
                {
                    LinearLayout spotlItem = new LinearLayout(Activity)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
                    };
                    spotlItem.Orientation = Orientation.Horizontal;
                    LinearLayout.LayoutParams paramsnew = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    paramsnew.SetMargins(0, Utils.CalcDimension(15), 0, 0);
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
                    text.LayoutParameters.Width = Utils.CalcDimension(435);
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
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            var strAddress = ReverseGeocodeCurrentLocation(Activity, Double.Parse(item.lat), Double.Parse(item.lng));
                            Activity.RunOnUiThread(() =>
                            {
                                addressText.Text = strAddress;
                                addressText.Invalidate();
                            });
                        });
                    }

                    var distanceText = new CustomTextView(Activity)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent),
                        Gravity = GravityFlags.Left,
                        Text = item.distance.ToString("0.00") + " km",
                        TextSize = 14
                    };
                    distanceText.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

                    string inrange = item.inrange ? "In range" : "";

                    var inRangeText = new CustomTextView(Activity)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                        Gravity = GravityFlags.Right,
                        Text = inrange,
                        TextSize = 14
                    };
                    inRangeText.SetCustomFont("Fonts/FocoCorp-Bold.ttf");

                    //add image
                    var image = new SvgImageView(Activity)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                    };

                    int imagerRsc = !item.superwifi ? Resource.Raw.AO__teal_b : Resource.Raw.AO__orange_s;

                    image.LayoutParameters.Width = Utils.CalcDimension(50);
                    image.SetSvg(Activity, imagerRsc);

                    var signalImage = new SvgImageView(Activity)
                    {
                        LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent,ViewGroup.LayoutParams.WrapContent)
                    };
                    signalImage.SetPadding(Utils.CalcDimension(15), Utils.CalcDimension(50), 0, 0);
                    signalImage.LayoutParameters.Width = Utils.CalcDimension(50);
                    int imagesRsc = 0;

                    switch (item.signalstrength)
                    {
                        default:
                        case 0: imagesRsc = Resource.Raw.AO__wifi_0; break;
                        case 1: imagesRsc = Resource.Raw.AO__wifi_1; break;
                        case 2: imagesRsc = Resource.Raw.AO__wifi_2; break;
                        case 3: imagesRsc = Resource.Raw.AO__wifi_3; break;
                    }
                    signalImage.SetSvg(Activity, imagesRsc);
                    imageLayout.AddView(image); 
                    imageLayout.AddView(signalImage);

                    textLayout.AddView(text);
                    textLayout.AddView(addressText);
                    textLayout.AddView(distanceText);
                    textLayout.AddView(inRangeText);
                    spotlItem.AddView(textLayout);
                    spotlItem.AddView(imageLayout);

                    tabItem.AddView(spotlItem);
                }
            }
            scroll.AddView(tabItem);
            return scroll;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KeyContent, "");
        }

        public string ReverseGeocodeCurrentLocation(Context context, double lat, double longs)
        {
            try
            {
                return new Android.Locations.Geocoder(context)
                                            .GetFromLocationAsync(lat, longs, 10)
                                            .Result
                                            .Select(n => n.GetAddressLine(0))
                                            .FirstOrDefault() ?? "";
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}