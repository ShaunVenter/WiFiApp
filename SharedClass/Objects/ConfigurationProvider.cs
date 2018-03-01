using System;

namespace AlwaysOn.Objects
{
    public static class ConfigurationProvider
    {
        //App Info
        public static string AppVersion { get; set; } = "v2.8";

        // App Colors
        public static int AppColorPurple { get; set; } = 0x632e8e;
        public static int AppColorCyan { get; set; } = 0x33babc;
        public static int AppColorYellow { get; set; } = 0xf4cc5d;
        public static int AppColorGrey { get; set; } = 0x3a3a3a;
        public static int AppColorDarkGrey { get; set; } = 0x303030;
        public static int AppColorTextGrey { get; set; } = 0x5a5a5a;
        public static int AppColorErrorOrange { get; set; } = 0xf0664d;
        public static int AppColorTextLightGrey { get; set; } = 0x797979;
        public static int AppColorFieldError { get; set; } = 0xf3684b;
        public static int AppColorSwicthTrack { get; set; } = 0x747474;
        public static int AppColorDataTabPurple { get; set; } = 0x843298;
        public static int AppColorDarkPurple { get; set; } = 0x502077;
        public static int AppColorButtonTouch { get; set; } = 0xede9e9;
        public static int AppColorHotspotBgGrey { get; set; } = 0x333333;
        public static int AppColorHotspotBarGrey { get; set; } = 0x111010;
        public static int AppColorSlideOrange { get; set; } = 0xdd5d5b;
        public static int AppColorTan { get; set; } = 0xe6bf59;

        //Fonts
        public static string FocoLight { get; set; } = "FocoCorp-Light";
        public static string FocoLightItalic { get; set; } = "FocoCorp-LightItalic";
        public static string FocoRegular { get; set; } = "FocoCorp-Regular";
        public static string FocoBold { get; set; } = "FocoCorp-Bold";
        public static string FocoBoldItalic { get; set; } = "FocoCorp-BoldItalic";
        public static string FocoItalic { get; set; } = "FocoCorp-Italic";

        //API
        public static string BaseUrl { get; set; } = "https://hotspot.alwayson.co.za/AlwaysOnMobileServiceAPI/AlwaysOnMobileService.svc";

        /* CHANGED TO PLATFORM SPECIFIC API KEYS */ //public static string ApiKey { get; set; } = "6ace7626-df27-4aca-8cde-08a4effccea8";
        /* ANDROID - MainApplication.ApiKey      */
        /* iOS     - AppDelegate.ApiKey          */

        public static string BackendEncodingType { get; set; } = "application/json";
        public static string RegistrationUrl { get; set; } = BaseUrl + "/RegisterUser";
        public static string LoginUrl { get; set; } = BaseUrl + "/getUserProfile";
        //public static string GetUserPackagesUrl { get; set; } = BaseUrl + "/getUserPackages";
        public static string GetUserPackagesUrlV2 { get; set; } = BaseUrl + "/getUserPackagesV2";
        public static string GetVoucherPackagesUrl { get; set; } = BaseUrl + "/GetVoucherPackages";
        public static string LinkExistingCredentialsUrl { get; set; } = BaseUrl + "/link_existing_credentials";
        public static string UnlinkExistingCredentialsUrl { get; set; } = BaseUrl + "/unlink_existing_credentials";
        public static string UpdateProfileUrl { get; set; } = BaseUrl + "/updateUserProfile";
        public static string ResetPasswordUrl { get; set; } = BaseUrl + "/forgotPassword";
        public static string GetServiceProvidersUrl { get; set; } = BaseUrl + "/getServiceProviders";
        public static string GetInfoButton { get; set; } = BaseUrl + "/getInfoButton";
        public static string UpdatePackageRanking { get; set; } = BaseUrl + "/updatePackageRanking";
        public static string SecurePayment { get; set; } = BaseUrl + "/SecureDirectPayment";
        public static string GetSSIDs { get; set; } = BaseUrl + "/getSSIDs";
        public static string GetHotspotLocationsInternational { get; set; } = BaseUrl + "/getHotspotLocationsInternational";
        public static string AccurisLogin { get; set; } = BaseUrl + "/LoginAccuris";
        public static string AccurisLogout { get; set; } = BaseUrl + "/LogoutAccuris";
        public static string AccurisActiveSessions { get; set; } = BaseUrl + "/UserInfoActiveSessions";
        public static string LinkSingleCode { get; set; } = BaseUrl + "/linkSingleCode";

        //Google Analytics tracking ID
        public static string GATrackingId { get; set; } = "UA-76965013-1";
    }
}

