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
using AlwaysOn;

namespace AlwaysOn_Droid
{
    public class TabFragment : Fragment
    {
        private const string KeyContent = "TestFragment:Content";
        private List<VoucherPackage> _content;
        LinearLayout tabItem;
        LinearLayout rightList;
        LinearLayout leftContainer;
        LinearLayout lineSelected;
        View circle;
        VoucherPackage current;

        public static TabFragment NewInstance(List<VoucherPackage> packages)
        {
            return new TabFragment()
            {
                _content = packages
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if ((savedInstanceState != null) && savedInstanceState.ContainsKey(KeyContent))
                _content = null;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                current = new VoucherPackage();
                tabItem = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
                };
                tabItem.Orientation = Orientation.Horizontal;
                rightList = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                };
                rightList.Id = 999;
                rightList.Orientation = Orientation.Vertical;
                rightList.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(70), 0, 0);

                lineSelected = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
                };
                lineSelected.LayoutParameters.Height = Utils.CalcDimension(3);
                lineSelected.LayoutParameters.Width = Utils.CalcDimension(110);
                lineSelected.SetBackgroundColor(Color.White);
                lineSelected.SetHorizontalGravity(GravityFlags.Right);
                lineSelected.TranslationY = Utils.CalcDimension(115);
                lineSelected.TranslationX = Utils.CalcDimension(-120);

                circle = new View(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(Utils.CalcDimension(8), Utils.CalcDimension(8))
                };
                circle.SetBackgroundResource(Resource.Drawable.WhiteCircle);
                circle.TranslationY = Utils.CalcDimension(90);
                circle.TranslationX = Utils.CalcDimension(-100);

                leftContainer = new LinearLayout(Activity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                };
                leftContainer.Id = 888;
                leftContainer.Orientation = Orientation.Vertical;

                foreach (var item in _content)
                {
                    int cnt = 0;
                    LinearLayout leftLayout = (LinearLayout)CreateLeftLayout(item);

                    leftContainer.AddView(leftLayout);
                    rightList.AddView(createListItem(item.packageName, item.Id));
                    cnt++;
                }

                if (_content[0].packageType.Equals("DT"))
                {
                    tabItem.SetBackgroundColor(Color.ParseColor("#33babc"));
                }
                if (_content[0].packageType.Equals("T"))
                {
                    tabItem.SetBackgroundColor(Color.ParseColor("#843298"));
                }
                leftContainer.GetChildAt(0).Visibility = ViewStates.Visible;
                tabItem.AddView(leftContainer);
                tabItem.SetPadding(Utils.CalcDimension(70), 0, 0, 0);
                tabItem.AddView(rightList);
                tabItem.AddView(lineSelected);
                tabItem.AddView(circle);
                return tabItem;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(KeyContent, "");
        }

        private View createListItem(string name, int id)
        {

            string[] title = name.Split('-');
            string packageName = title[0];
            var listItem = new CustomTextView(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Gravity = GravityFlags.Right,
                Text = packageName,
                TextSize = 18
            };
            listItem.Id = id;
            listItem.Gravity = GravityFlags.Right;
            listItem.SetTextColor(Color.ParseColor("#ffffff"));
            listItem.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            listItem.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

            listItem.Click += ListItem_Click;
            
            return listItem;
        }

        private View CreateLeftLayout(VoucherPackage vpackage)
        {
            LinearLayout leftLayout = new LinearLayout(Activity)
            {

                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };

            leftLayout.LayoutParameters.Width = Utils.CalcDimension(330);
            leftLayout.Id = vpackage.Id * 10;


            string[] title = vpackage.packageName.Split('-');
            string packageName = title[0];

            var text = new CustomTextView(Activity)
            {
                Gravity = GravityFlags.Left,
                Text = packageName,
                TextSize = 45
            };
            text.SetTextColor(Color.ParseColor("#ffffff"));
            text.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            text.TextAlignment = TextAlignment.ViewStart;
            text.SetMaxLines(1);
            text.SetCustomFont("Fonts/FocoCorp-Bold.ttf");

            string[] priceValue = vpackage.displayPrice.Split('.');
            string priceNewValue = priceValue[0];
            var price = new CustomTextView(Activity)
            {
                Gravity = GravityFlags.Left,
                Text = priceNewValue + "*",
                TextSize = 27
            };
            price.SetTextColor(Color.ParseColor("#ffffff"));
            price.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

            var priceDetail = new CustomTextView(Activity)
            {
                Gravity = GravityFlags.Right,
                Text = vpackage.Period + "\n" + vpackage.PricePer,
                TextSize = 13
            };
            priceDetail.SetTextColor(Color.ParseColor("#ffffff"));
            priceDetail.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

            var description = new CustomTextView(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Gravity = GravityFlags.Right,
                Text = vpackage.usageDescription,
                TextSize = 16
            };
            description.LayoutParameters.Height = Utils.CalcDimension(150);
            description.SetTextColor(Color.ParseColor("#ffffff"));
            description.SetPadding(0, Utils.CalcDimension(25), 0, 0);
            description.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

            string expiryText = string.Empty;
            if (vpackage.packageType.Equals("DT"))
            {
                expiryText = "Expires in " + vpackage.expirationDays + " days";
            }
            if (vpackage.packageType.Equals("T"))
            {
                if (Convert.ToInt16(vpackage.expirationHours) == 0)
                {
                    expiryText = "";
                }
                else
                    expiryText = "Expires in " + vpackage.expirationHours + " hours";
            }

            var expires = new CustomTextView(Activity)
            {
                LayoutParameters =
                    new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent),
                Gravity = GravityFlags.Right,
                Text = expiryText,
                TextSize = 16
            };
            expires.LayoutParameters.Height = Utils.CalcDimension(110);
            expires.SetTextColor(Color.ParseColor("#ffffff"));
            expires.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            expires.SetCustomFont("Fonts/FocoCorp-Regular.ttf");

            var usage = new CustomTextView(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Gravity = GravityFlags.Center,
                Text = "up to " + vpackage.Songs + " songs or\nup to " + vpackage.Videos + " videos or\nup to " + vpackage.Voice + " minutes voice",
                TextSize = 16
            };
            usage.SetTextColor(Color.ParseColor("#ffffff"));
            usage.LayoutParameters.Height = Utils.CalcDimension(200);
            usage.SetCustomFont("Fonts/FocoCorp-Regular.ttf");
            SvgImageView svg = new SvgImageView(Activity);
            int svgSrc;
            if (vpackage.packageType == "DT")
            {
                svgSrc = Resource.Raw.AO__purple_speach;
            }
            else
                svgSrc = Resource.Raw.AO__teal_speach;

            svg.SetSvg(Activity, svgSrc);
            usage.Background = svg.ImageDrawable;

            var btnBuy = new CustomButton(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent),
                Text = "BUY THIS PACKAGE",
                TextSize = 16
            };
            btnBuy.Id = vpackage.Id;
            btnBuy.SetTextColor(Color.ParseColor("#ffffff"));
            btnBuy.SetCustomFont("Fonts/FocoCorp-Bold.ttf");
            btnBuy.SetMaxLines(1);
            if (vpackage.packageType == "DT")
            {
                btnBuy.SetBackgroundResource(Resource.Drawable.YellowButton_Ripple);
            }
            else
                btnBuy.SetBackgroundResource(Resource.Drawable.PurpleButton_Ripple);


            RelativeLayout spaceButton = new RelativeLayout(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            spaceButton.LayoutParameters.Height = 20;
            spaceButton.SetBackgroundColor(Color.Transparent);

            RelativeLayout priceLayout = new RelativeLayout(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            };
            priceLayout.AddView(price);
            priceLayout.SetPadding(0, Utils.CalcDimension(40), 0, 0);

            RelativeLayout line = new RelativeLayout(Activity)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1)
            };
            line.SetBackgroundColor(Color.White);

            RelativeLayout.LayoutParams newParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            newParams.AddRule(LayoutRules.AlignRight);
            priceDetail.LayoutParameters = newParams;
            priceLayout.AddView(priceDetail);

            leftLayout.Orientation = Orientation.Vertical;
            leftLayout.SetGravity(GravityFlags.Right);
            leftLayout.AddView(text);
            leftLayout.AddView(priceLayout);
            leftLayout.AddView(line);
            leftLayout.AddView(description);
            leftLayout.AddView(expires);
            leftLayout.AddView(usage);
            leftLayout.AddView(spaceButton);
            leftLayout.AddView(btnBuy);
            leftLayout.Visibility = ViewStates.Gone;
            btnBuy.Click += btnBuyPackage_Click;
            current = _content[0];

            return leftLayout;
        }

        void ListItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < leftContainer.ChildCount; i++)
            {
                leftContainer.GetChildAt(i).Visibility = ViewStates.Gone;
            }

            TextView sbutton = (TextView)sender;

            LinearLayout theTab = tabItem.FindViewById<LinearLayout>(sbutton.Id * 10);
            var indexx = rightList.IndexOfChild(sbutton);
            LinearLayout thisone = (LinearLayout)leftContainer.GetChildAt(indexx);
            thisone.Visibility = ViewStates.Visible;

            var baseLine = 115;
            var baseCircle = 90;

            lineSelected.TranslationY = Utils.CalcDimension(baseLine + (73 * indexx));
            circle.TranslationY = Utils.CalcDimension(baseCircle + (73 * indexx));

            current = _content[indexx];
        }

        void btnBuyPackage_Click(object sender, EventArgs e)
        {
            try
            {
                AnalyticsProvider_Droid.TrackEventGA((BuyPackage)Context, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.BuyPackageBuyThis.ToString() + "_" + current.HeaderPre + current.HeaderPost);
            }
            catch { }
            
            var purchase = new Intent(Activity, typeof(Purchase));
            purchase.PutExtra("packageIDs", current.Id.ToString());
            purchase.PutExtra("packageName", current.packageName);
            purchase.PutExtra("packagePrice", current.Price);
            purchase.PutExtra("packagePriceV", current.iveriPrice);
            purchase.PutExtra("HeaderPre", current.HeaderPre);
            purchase.PutExtra("HeaderPost", current.HeaderPost);
            purchase.PutExtra("packagePeriod", current.Period);
            purchase.PutExtra("packagePricePer", current.PricePer);
            purchase.PutExtra("packageDesc", current.packageDesc);

            purchase.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            //purchase.PutExtra("packageID", current.Id);
            StartActivity(purchase);
            ((BuyPackage)Context).Finish();
        }
    }
}