using AlwaysOn;
using AlwaysOn.Objects;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Threading.Tasks;
using XamSvg;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.PurchaseConfirmation")]
    public class PurchaseConfirmation : Activity
    {
        SvgImageView img;
        SvgImageView imgMenu;
        SvgImageView imgBack;
        CustomTextView txtTitle;
        CustomTextView txtSafeMsg;
        CustomTextView txtSafeMsgDesc;
        CustomTextView txtPer;
        LinearLayout lytForm;
        RelativeLayout lytMsg;
        CustomTextView txtName;
        CustomTextView txtCardnumber;
        CustomTextView txtExpiry;
        CustomTextView txtCvv;
        CustomTextView txtUserEmail;
        CustomTextView txtUserMobile;

        ProgressBar myProgressBar;
        RelativeLayout lytprogressBar;

        CustomButton fabPay;
        LinearLayout lytNotification;
        FrameLayout frameLayout;

        UserObject User = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.PurchaseConfirmation.ToString());

            SetContentView(Resource.Layout.PurchaseConfirmation);

            User = BackendProvider_Droid.GetUser;
            if (User == null)
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
            imgBack.Click += btnCancel_Click;

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.Click += imgMenu_Click;
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);

            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);
            txtTitle.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(100), 0, Utils.CalcDimension(50));

            lytMsg = FindViewById<RelativeLayout>(Resource.Id.lytMsg);
            lytMsg.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(25), Utils.CalcDimension(70), Utils.CalcDimension(25));

            txtSafeMsg = FindViewById<CustomTextView>(Resource.Id.txtMsg);
            txtSafeMsg.SetPadding(0, Utils.CalcDimension(10), 0, Utils.CalcDimension(10));
            string HeaderPre = Intent.GetStringExtra("HeaderPre") ?? "0";
            string HeaderPost = Intent.GetStringExtra("HeaderPost") ?? "0";
            string packageName = Intent.GetStringExtra("packageName") ?? "0";
            string packagePrice = Intent.GetStringExtra("packagePrice") ?? "0";
            txtSafeMsg.Text = HeaderPre + " " + HeaderPost + "  " + packagePrice;
            if (Resources.DisplayMetrics.HeightPixels < 801)
            {
                txtSafeMsg.TextSize = 32;
            }

            txtSafeMsgDesc = FindViewById<CustomTextView>(Resource.Id.txtMsgDesc);
            string packageDesc = Intent.GetStringExtra("packageDesc") ?? "0";
            txtSafeMsgDesc.Text = packageDesc;
            txtSafeMsgDesc.SetPadding(0, Utils.CalcDimension(25), 0, 0);
            txtSafeMsg.Text = HeaderPre + " " + HeaderPost + "  " + packagePrice;
            if (Resources.DisplayMetrics.HeightPixels < 801)
            {
                txtSafeMsgDesc.TextSize = 12;
            }

            txtPer = FindViewById<CustomTextView>(Resource.Id.txtPer);
            string packagePricePer = Intent.GetStringExtra("packagePricePer") ?? "0";
            string packagePricePeriod = Intent.GetStringExtra("packagePeriod") ?? "0";
            txtPer.Text = packagePricePeriod + "\n(" + packagePricePer + ")";
            txtPer.SetPadding(Utils.CalcDimension(45), Utils.CalcDimension(25), 0, 0);
            txtPer.Gravity = GravityFlags.CenterHorizontal;
            lytForm = FindViewById<LinearLayout>(Resource.Id.lytForm);
            lytForm.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            txtName = FindViewById<CustomTextView>(Resource.Id.txtName);
            txtName.SetPadding(0, 0, 0, Utils.CalcDimension(40));
            string cardHolderName = Intent.GetStringExtra("cardHolderName") ?? "0";
            txtName.Text = cardHolderName.ToUpper();

            txtCardnumber = FindViewById<CustomTextView>(Resource.Id.txtCardnumber);
            txtCardnumber.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            string Cardnumber = Intent.GetStringExtra("Cardnumber") ?? "0";
            txtCardnumber.Text = Cardnumber;

            txtExpiry = FindViewById<CustomTextView>(Resource.Id.txtExpiry);
            txtExpiry.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            string expiryMonth = Intent.GetStringExtra("expiryMonth").PadLeft(2, '0');
            string expiryYear = Intent.GetStringExtra("expiryYear").Substring(2, 2);
            txtExpiry.Text = expiryMonth + "/" + expiryYear;

            txtCvv = FindViewById<CustomTextView>(Resource.Id.txtCvv);
            txtCvv.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            string CVV = Intent.GetStringExtra("CVV") ?? "0";
            txtCvv.Text = CVV;

            txtUserEmail = FindViewById<CustomTextView>(Resource.Id.txtUserEmail);
            txtUserEmail.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            //string userEmail = Intent.GetStringExtra("userEmail") ?? "0";
            txtUserEmail.Text = Intent.GetStringExtra("emailaddress");

            txtUserMobile = FindViewById<CustomTextView>(Resource.Id.txtUserMobile);
            txtUserMobile.SetPadding(0, 0, 0, Utils.CalcDimension(30));
            //string userMobile = Intent.GetStringExtra("userMobile") ?? "0";
            txtUserMobile.Text = User.MobileNumber;

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            
            frameLayout = FindViewById<FrameLayout>(Resource.Id.frameLayout);
            frameLayout.SetPadding(0, 0, Utils.CalcDimension(50), Utils.CalcDimension(100));
            fabPay = FindViewById<CustomButton>(Resource.Id.fab_Pay);
            fabPay.SetBackgroundResource(Resource.Drawable.payNow);
            fabPay.Click += fabPay_Click;

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(10), Utils.CalcDimension(70), Utils.CalcDimension(10));
            lytNotification.Visibility = ViewStates.Gone;

            HideLoader();
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "PurchaseConfirmation");
            StartActivity(menu);
            Finish();
        }

        bool clickedPay = false;
        void fabPay_Click(object sender, EventArgs e)
        {
            if (clickedPay)
                return;

            clickedPay = true;

            ShowLoader();
            
            var text = new CustomTextView(this)
            {
                Gravity = GravityFlags.Left,
                Text = "",
                TextSize = 14
            };
            text.SetCustomFont("Fonts/FocoCorp-Regular.ttf");
            lytNotification.AddView(text);
            lytNotification.LayoutParameters.Height = Utils.CalcDimension(100);

            var debit = PaymentProvider_Droid.DirectDebit(User.UserId, Intent.GetStringExtra("emailaddress"), User.MobileNumber, Intent.GetStringExtra("packageID") ?? "0", Intent.GetStringExtra("Cardnumber"), Intent.GetStringExtra("CVV"), Intent.GetStringExtra("cardHolderName"), Intent.GetStringExtra("expiryMonth"), Intent.GetStringExtra("expiryYear"));
            if (debit.Success)
            {
                try
                {
                    string HeaderPre = Intent.GetStringExtra("HeaderPre");
                    string HeaderPost = Intent.GetStringExtra("HeaderPost");
                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.PurchaseConfirmationPay.ToString() + "_" + HeaderPre + HeaderPost);
                }
                catch { }

                var animation = new Android.Views.Animations.AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
                animation.AnimationEnd += (object s, Android.Views.Animations.Animation.AnimationEndEventArgs e2) =>
                {
                    lytNotification.RemoveView(text);
                    lytNotification.Visibility = ViewStates.Gone;
                    clickedPay = false;

                    BackendProvider_Droid.UpdatePackagesFromServer();
                    var intent = new Intent(this, typeof(Dashboard));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);

                    Finish();
                };

                text.Text = "Your transaction has been completed succesfully.";
                lytNotification.Visibility = ViewStates.Visible;
                lytNotification.SetBackgroundColor(Color.ParseColor("#3badb9"));
                lytNotification.LayoutParameters.Height = Utils.CalcDimension(110);
                lytNotification.StartAnimation(animation);
            }
            else
            {
                var animation = new Android.Views.Animations.AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
                animation.AnimationEnd += (object s, Android.Views.Animations.Animation.AnimationEndEventArgs e2) =>
                {
                    lytNotification.RemoveView(text);
                    lytNotification.Visibility = ViewStates.Gone;
                    clickedPay = false;

                    HideLoader();
                };

                text.Text = "Error: " + debit.Message;
                lytNotification.Visibility = ViewStates.Visible;
                lytNotification.SetBackgroundColor(Color.ParseColor("#f0674d"));
                lytNotification.LayoutParameters.Height = Utils.CalcDimension(110);
                lytNotification.StartAnimation(animation);
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.PurchaseConfirmationBack.ToString());

            var intent = new Intent(this, typeof(BuyPackage));
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