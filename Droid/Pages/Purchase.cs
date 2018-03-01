using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using Android.Support.V7.AppCompat;
using AlwaysOn;
using AlwaysOn.Objects;
using Android.Views.Animations;

namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", Icon = "@drawable/Icon", Theme = "@style/AlwaysOnTheme", ScreenOrientation = ScreenOrientation.Portrait, Name = "com.is.alwayson.Purchase")]
    public class Purchase : Activity
    {
        SvgImageView img;
        SvgImageView imgBack;
        SvgImageView imgMenu;
        SvgImageView imgVisa;
        SvgImageView imgMastercard;
        CustomTextView txtTitle;
        CustomTextView txtSafeMsg;
        LinearLayout lytForm;
        LinearLayout lytCardImages;
        CustomEditText txtName;
        CustomEditText txtCardNumber;
        CustomEditText txtEmail;
        CustomEditText txtCvv;
        LinearLayout lytName;
        LinearLayout lytCardNumber;
        LinearLayout lytEmail;
        TextInputLayout lyttxtName;
        TextInputLayout lyttxtCardNumber;
        LinearLayout lytNotification;
        TextInputLayout lyttxtEmail;
        TextInputLayout lyttxtCvv;
        RelativeLayout lytButtons;
        CustomButton btnSignIn;

        RelativeLayout lytprogressBar;
        ProgressBar myProgressBar;

        Animation fadeInAnimation;
        CustomButton btnRegister;
        Spinner monthSpinner;
        Spinner yearSpinner;
        LinearLayout lytCardExpiry;
        LinearLayout lytCvv;
        string CardType;
        static string SelectedMonth = "01";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AnalyticsProvider_Droid.PageViewGA(this, PageName.Purchase.ToString());

            SetContentView(Resource.Layout.Purchase);

            var User = BackendProvider_Droid.GetUser;
            if (User == null)
            {
                var login = new Intent(this, typeof(Login));
                login.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(login);
                Finish();
                return;
            }

            var packageID = Intent.GetStringExtra("packageIDs");

            RelativeLayout lytActionBar = FindViewById<RelativeLayout>(Resource.Id.lytActionBar);
            lytActionBar.LayoutParameters.Height = (int)(Resources.DisplayMetrics.HeightPixels / 11.4);

            img = FindViewById<SvgImageView>(Resource.Id.imgLog2);
            img.SetSvg(this, Resource.Raw.AO__full_logo);
            img.LayoutParameters.Width = Utils.CalcDimension(230);

            imgBack = FindViewById<SvgImageView>(Resource.Id.imgBack);
            imgBack.SetSvg(this, Resource.Raw.AO__arrow_back);
            imgBack.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(20), Utils.CalcDimension(30), Utils.CalcDimension(20));
            imgBack.LayoutParameters.Width = Utils.CalcDimension(130);
            imgBack.Click += imgBack_Click;

            imgMenu = FindViewById<SvgImageView>(Resource.Id.imgMenu);
            imgMenu.SetSvg(this, Resource.Raw.AO__menu_icn, "ffffff=632e8e");
            imgMenu.Click += imgMenu_Click;
            imgMenu.SetPadding(Utils.CalcDimension(30), Utils.CalcDimension(20), Utils.CalcDimension(70), Utils.CalcDimension(20));
            imgMenu.LayoutParameters.Height = Utils.CalcDimension(80);
            imgMenu.LayoutParameters.Width = Utils.CalcDimension(110);

            txtTitle = FindViewById<CustomTextView>(Resource.Id.txtTitle);
            txtTitle.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(100), 0, Utils.CalcDimension(50));

            txtSafeMsg = FindViewById<CustomTextView>(Resource.Id.txtSafeMsg);
            txtSafeMsg.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(25), Utils.CalcDimension(70), Utils.CalcDimension(25));

            lytForm = FindViewById<LinearLayout>(Resource.Id.lytForm);
            lytForm.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(30), Utils.CalcDimension(70), Utils.CalcDimension(30));

            imgVisa = FindViewById<SvgImageView>(Resource.Id.imgVisa);
            imgVisa.SetSvg(this, Resource.Raw.AO__CC_Visa);
            imgVisa.LayoutParameters.Width = Utils.CalcDimension(80);
            imgMastercard = FindViewById<SvgImageView>(Resource.Id.imgMasterCard);
            imgMastercard.SetSvg(this, Resource.Raw.AO__CC_MasterCard);
            imgMastercard.LayoutParameters.Width = Utils.CalcDimension(80);

            lytCardImages = FindViewById<LinearLayout>(Resource.Id.lytCardImages);
            lytCardImages.SetPadding(0, Utils.CalcDimension(30), 0, 0);

            lytName = FindViewById<LinearLayout>(Resource.Id.lytName);
            lytName.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            lyttxtName = FindViewById<TextInputLayout>(Resource.Id.lyttxtName);
            lyttxtName.Hint = "Full name on card";
            txtName = FindViewById<CustomEditText>(Resource.Id.txtName);
            txtName.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtName.Text))
                {
                    lyttxtName.Error = "";

                    if (txtName.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtName.Error = " ";
                    }
                }
            };

            lytCardNumber = FindViewById<LinearLayout>(Resource.Id.lytCardNumber);
            lytCardNumber.SetPadding(0, Utils.CalcDimension(30), 0, Utils.CalcDimension(30));
            lyttxtCardNumber = FindViewById<TextInputLayout>(Resource.Id.lyttxtCardNumber);
            lyttxtCardNumber.Hint = "Card number";
            txtCardNumber = FindViewById<CustomEditText>(Resource.Id.txtCardNumber);
            txtCardNumber.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtCardNumber.Text))
                {
                    lyttxtCardNumber.Error = "";

                    if (txtCardNumber.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtCardNumber.Error = " ";
                    }
                }
            };


            lytEmail = FindViewById<LinearLayout>(Resource.Id.lytEmail);
            lytEmail.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            lyttxtEmail = FindViewById<TextInputLayout>(Resource.Id.lyttxtEmail);
            lyttxtEmail.Hint = "Email";
            txtEmail = FindViewById<CustomEditText>(Resource.Id.txtEmail);
            txtEmail.Text = User.LoginCredential;
            txtEmail.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtEmail.Text) || BackendProvider_Droid.IsValidEmail(txtEmail.Text) == false)
                {
                    lyttxtEmail.Error = "";

                    if (txtEmail.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtEmail.Error = " ";
                    }
                }
            };

            lytCardExpiry = FindViewById<LinearLayout>(Resource.Id.lytCardExpiry);
            lytCardExpiry.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            
            monthSpinner = FindViewById<Spinner>(Resource.Id.spinner1);
            monthSpinner.LayoutParameters.Width = Utils.CalcDimension(145);
            monthSpinner.SetSelection(Convert.ToInt32(SelectedMonth) - 1);
            var months = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            var monthAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, months);
            monthAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            monthSpinner.Adapter = monthAdapter;
            monthSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                SelectedMonth = months[e.Position];
            };

            yearSpinner = FindViewById<Spinner>(Resource.Id.spinner2);
            yearSpinner.LayoutParameters.Width = Utils.CalcDimension(155);
            yearSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                if(yearSpinner.SelectedItem.ToString() == DateTime.Now.Year.ToString())
                {
                    months = Enumerable.Range(DateTime.Now.Month, 13 - DateTime.Now.Month).Select(n => n.ToString().PadLeft(2, '0')).ToArray();
                }
                else
                {
                    months = new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
                }
                monthAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, months);
                monthAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                monthSpinner.Adapter = monthAdapter;

                var index = Array.IndexOf(months, SelectedMonth);
                index = index == -1 ? 0 : index;
                SelectedMonth = months[index];
                monthSpinner.SetSelection(index);
            };
            yearSpinner.SetSelection(1);
            var next6Years = Enumerable.Range(DateTime.Now.Year, 6).Select(n => n.ToString()).ToList();
            var yearAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, next6Years);
            yearAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            yearSpinner.Adapter = yearAdapter;

            var cardAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.card_types, Android.Resource.Layout.SimpleSpinnerItem);

            lytCvv = FindViewById<LinearLayout>(Resource.Id.lytCvv);
            lytCvv.SetPadding(0, Utils.CalcDimension(30), 0, 0);
            lyttxtCvv = FindViewById<TextInputLayout>(Resource.Id.lyttxtCvv);
            lyttxtCvv.Hint = "CVV";
            txtCvv = FindViewById<CustomEditText>(Resource.Id.txtCvv);
            txtCvv.LayoutParameters.Width = Utils.CalcDimension(155);
            txtCvv.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
            {
                if (string.IsNullOrEmpty(txtCvv.Text))
                {
                    lyttxtCvv.Error = "";

                    if (txtCvv.HasFocus == false)
                    {
                        //txtEmail.SetHintTextColor(Color.Red);
                        lyttxtCvv.Error = " ";
                    }
                }
            };

            lytButtons = FindViewById<RelativeLayout>(Resource.Id.lytButtons);
            lytButtons.LayoutParameters.Height = Utils.CalcDimension(160);

            btnRegister = FindViewById<CustomButton>(Resource.Id.btnContinue);
            btnRegister.LayoutParameters.Width = Utils.CalcDimension(245);
            btnRegister.LayoutParameters.Height = Utils.CalcDimension(75);
            btnRegister.Click += BtnRegister_Click;

            lytprogressBar = FindViewById<RelativeLayout>(Resource.Id.lytprogressBar);
            myProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            btnSignIn = FindViewById<CustomButton>(Resource.Id.btnCancel);
            btnSignIn.LayoutParameters.Width = Utils.CalcDimension(245);
            btnSignIn.LayoutParameters.Height = Utils.CalcDimension(75);
            btnSignIn.Click += btnCancel_Click;
            imgBack.Click += btnCancel_Click;

            fadeInAnimation = new AlphaAnimation(1, 0) { Duration = 1000, StartOffset = 2000 };
            fadeInAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
            {
                lytNotification.Visibility = ViewStates.Gone;
                HideLoader();
            };

            lytNotification = FindViewById<LinearLayout>(Resource.Id.lytNotification);
            lytNotification.SetPadding(Utils.CalcDimension(70), Utils.CalcDimension(10), Utils.CalcDimension(70), Utils.CalcDimension(10));
            lytNotification.Visibility = ViewStates.Gone;

            HideLoader();
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            lyttxtName.Error = "";
            lyttxtEmail.Error = "";
            lyttxtCardNumber.Error = "";
            lyttxtCvv.Error = "";
            bool valid = true;

            var name = string.IsNullOrWhiteSpace(txtName.Text) ? string.Empty : txtName.Text.Trim();
            var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? string.Empty : txtEmail.Text.Trim();
            var cardNumber = string.IsNullOrWhiteSpace(txtCardNumber.Text) ? string.Empty : txtCardNumber.Text.Trim();
            var cvv = string.IsNullOrWhiteSpace(txtCvv.Text) ? string.Empty : txtCvv.Text.Trim();

            if (string.IsNullOrEmpty(email) || BackendProvider_Droid.IsValidEmail(email) == false)
            {
                lyttxtEmail.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(name))
            {
                lyttxtName.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(cardNumber) || ValidateCreditCard(cardNumber) == false)
            {
                lyttxtCardNumber.Error = " ";
                valid = false;
            }

            if (string.IsNullOrEmpty(cvv))
            {
                lyttxtCvv.Error = " ";
                valid = false;
            }

            if (valid)
            {
                var headerPre = Intent.GetStringExtra("HeaderPre");
                var headerPost = Intent.GetStringExtra("HeaderPost");

                var confirm = new Intent(this, typeof(PurchaseConfirmation));
                confirm.PutExtra("packageID", Intent.GetStringExtra("packageIDs"));
                confirm.PutExtra("packageName", Intent.GetStringExtra("packageName"));
                confirm.PutExtra("packagePrice", Intent.GetStringExtra("packagePrice"));
                confirm.PutExtra("packagePriceV", Intent.GetStringExtra("packagePriceV"));
                confirm.PutExtra("HeaderPre", headerPre);
                confirm.PutExtra("HeaderPost", headerPost);
                confirm.PutExtra("packagePeriod", Intent.GetStringExtra("packagePeriod"));
                confirm.PutExtra("packagePricePer", Intent.GetStringExtra("packagePricePer"));
                confirm.PutExtra("packageDesc", Intent.GetStringExtra("packageDesc"));

                confirm.PutExtra("cardHolderName", name);
                confirm.PutExtra("emailaddress", email);
                confirm.PutExtra("Cardnumber", cardNumber);
                confirm.PutExtra("CVV", cvv);
                confirm.PutExtra("expiryMonth", monthSpinner.SelectedItem.ToString());
                confirm.PutExtra("expiryYear", yearSpinner.SelectedItem.ToString());
                confirm.PutExtra("cardType", CardType);

                confirm.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

                try
                {
                    AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.PurchaseContinue.ToString() + "_" + headerPre + headerPost);
                }
                catch { }

                StartActivity(confirm);
            }
            else
            {
                lytNotification.SetBackgroundColor(Color.ParseColor("#f0674d"));
                var text = new CustomTextView(this)
                {
                    Gravity = GravityFlags.Left,
                    Text = "Oops! Something went wrong, please review your details and try again.",
                    TextSize = 14,
                };
                lytNotification.AddView(text);
                lytNotification.LayoutParameters.Height = Utils.CalcDimension(100);
            }
        }

        void imgMenu_Click(object sender, EventArgs e)
        {
            var menu = new Intent(this, typeof(Menu));
            menu.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            menu.PutExtra("prevPage", "Purchase");
            StartActivity(menu);
            Finish();
        }

        void txtForgot_Click(object sender, EventArgs e)
        {
            var forgotPassword = new Intent(this, typeof(ForgotPassword));
            forgotPassword.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(forgotPassword);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        void imgBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        
        public override void OnBackPressed()
        {
            AnalyticsProvider_Droid.TrackEventGA(this, TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.PurchaseCancel.ToString());

            var intent = new Intent(this, typeof(BuyPackage));
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }

        bool ValidateCreditCard(string number)
        {
            //master card
            if (number.Substring(0, 1).Equals("5")
                && int.Parse(number.Substring(1, 1)) > 0
                && int.Parse(number.Substring(1, 1)) < 6
                && number.Length == 16)
            {
                CardType = "MASTER";
                return true;
            }

            //visa
            if (number.Substring(0, 1).Equals("4") && (number.Length == 13 || number.Length == 16))
            {
                CardType = "VISA";
                return true;
            }

            CardType = string.Empty;
            return false;
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

