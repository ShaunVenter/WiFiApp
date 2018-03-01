using System;
using UIKit;
using AlwaysOn_iOS.Objects;
using CoreGraphics;
using FloatLabeledEntry;
using Foundation;
using MaterialControls;
using AlwaysOn;
using AlwaysOn.Objects;
using System.Text.RegularExpressions;
using System.Drawing;
using TPKeyboardAvoiding;

namespace AlwaysOn_iOS
{
    public partial class ProfileDetailsPage : UIViewController
    {
        public ProfileDetailsPage() : base("ProfileDetails", null)
        {
            UtilProvider.MenuIsOpen = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            AnalyticsProvider_iOS.PageViewGA(PageName.ProfileDetails.ToString());

            View.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorDarkGrey);

            var ProfileWrapper = new UIView(new CGRect(40, 0, UtilProvider.ScreenWidth - 80, UtilProvider.ScreenHeight));
            var instruction = new UILabel(new CGRect(0, 10, ProfileWrapper.Bounds.Width, 30))
            {
                Text = "Account Details",
                TextColor = UIColor.White,
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 23)
            };

            var userObject = BackendProvider_iOS.GetUser;

            UILabel FullNameLabel = new UILabel(new CGRect(0, UtilProvider.ItemPosition(1) - 15, ProfileWrapper.Bounds.Width, 35))
            {
                Text = "Name",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                TextColor = UIColor.White
            };
            var Name = new UILabel(new CGRect(0, UtilProvider.ItemPosition(1), ProfileWrapper.Bounds.Width, 35))
            {
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                Text = userObject.Name
            };

            UILabel SurnameLabel = new UILabel(new CGRect(0, UtilProvider.ItemPosition(2) - 15, ProfileWrapper.Bounds.Width, 35))
            {
                Text = "Surname",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                TextColor = UIColor.White
            };
            var Surname = new UILabel(new CGRect(0, UtilProvider.ItemPosition(2), ProfileWrapper.Bounds.Width, 35))
            {
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                Text = userObject.Surname
            };

            UILabel EmailLabel = new UILabel(new CGRect(0, UtilProvider.ItemPosition(3) - 15, ProfileWrapper.Bounds.Width, 35))
            {
                Text = "Email",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                TextColor = UIColor.White
            };
            var Email = new UILabel(new CGRect(0, UtilProvider.ItemPosition(3), ProfileWrapper.Bounds.Width, 35))
            {
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                Text = userObject.LoginCredential
            };

            UILabel MobileLabel = new UILabel(new CGRect(0, UtilProvider.ItemPosition(4) - 15, ProfileWrapper.Bounds.Width, 35))
            {
                Text = "Mobile number",
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Light, 13),
                TextColor = UIColor.White
            };
            var MobileNumber = new UILabel(new CGRect(0, UtilProvider.ItemPosition(4), ProfileWrapper.Bounds.Width, 35))
            {
                Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Regular, 14),
                TintColor = UIColor.White,
                TextColor = UIColor.White,
                Text = userObject.MobileNumber
            };

            var EditBtn = new MDButton(new CGRect(ProfileWrapper.Bounds.Width - (ProfileWrapper.Bounds.Width / 2) + 5, UtilProvider.ItemPosition(6), ProfileWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            EditBtn.SetTitle("EDIT", UIControlState.Normal);
            EditBtn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            EditBtn.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorCyan);
            EditBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsEdit.ToString());

                NavigationController.PushViewController(new UpdateUserProfile(), true);
            };

            var LogoutBtn = new MDButton(new CGRect(0, UtilProvider.ItemPosition(6), ProfileWrapper.Bounds.Width / 2 - 10, 40), MDButtonType.Flat, UIColor.White);
            LogoutBtn.SetTitle("SIGN OUT", UIControlState.Normal);
            LogoutBtn.Font = UtilProvider.GetAppFont(UtilProvider.FontShape.Bold, 13f);
            LogoutBtn.BackgroundColor = UIColor.Clear;
            LogoutBtn.Layer.BorderWidth = 1;
            LogoutBtn.Layer.BorderColor = UIColor.White.CGColor;
            LogoutBtn.TouchUpInside += (object sender, EventArgs e) =>
            {
                AnalyticsProvider_iOS.TrackEventGA(TrackingCategory.Flow.ToString(), TrackingAction.ButtonPress.ToString(), EventLabel.ProfileDetailsSignout.ToString());

                BackendProvider_iOS.ClearSettings();
                NavigationController.PushViewController(new LoginScreen(false), true);
            };

            ProfileWrapper.Add(instruction);

            ProfileWrapper.Add(FullNameLabel);
            ProfileWrapper.Add(Name);
            ProfileWrapper.Add(SurnameLabel);
            ProfileWrapper.Add(Surname);
            ProfileWrapper.Add(EmailLabel);
            ProfileWrapper.Add(Email);
            ProfileWrapper.Add(MobileLabel);
            ProfileWrapper.Add(MobileNumber);

            ProfileWrapper.Add(EditBtn);
            ProfileWrapper.Add(LogoutBtn);

            View.Add(ProfileWrapper);

            var parent = new UIView(new CGRect(0, UtilProvider.SafeTop + UtilProvider.ScreenHeight12th, UtilProvider.ScreenWidth, UtilProvider.ScreenHeight - UtilProvider.SafeTop - UtilProvider.ScreenHeight12th));
            parent.BackgroundColor = UIColor.Clear.FromHex(ConfigurationProvider.AppColorPurple);
            parent.Add(ProfileWrapper);

            View.Add(parent);
            View.Add(UtilProvider.HeaderStrip(true, true, this));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}


