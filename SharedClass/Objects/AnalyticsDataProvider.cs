using System;


namespace AlwaysOn
{
    public enum EventLabel
    {
        IntroCarouselSkipIntro = 1,
        IntroCarouselGetStarted = 2,

        LoginForgot = 3,
        LoginRegister = 4,
        LoginSignin = 5,

        RegisterSignin = 6,
        RegisterRegister = 7,

        ForgotPasswordCancel = 8,
        ForgotPasswordRecover = 9,

        DashboardInfoButton = 10, //iOS Exclusive
        DashboardPackageSelect = 11,
        DashboardPackageConnect = 12,
        DashboardPackageDisconnect = 13,
        DashboardPackageViewDetails = 14,
        DashboardPackageUnlink = 15,
        DashboardTurnOnWifi = 16, //Android Exclusive
        DashboardConnectAlwaysOn = 17, //Android Exclusive

        MenuClose = 18,
        MenuSignout = 19,
        MenuDashboard = 20,
        MenuBuyPackage = 21, //Android Exclusive
        MenuLinkPackage = 22,
        MenuHotspotFinder = 23,
        MenuProfileDetails = 24,
        MenuSettings = 25,
        MenuHowToConnect = 26, //iOS Exclusive
        MenuTechnicalSupport = 27,
        MenuTermsConditions = 28,

        LinkPackageCancel = 29,
        LinkPackageLink = 30,

        //HotspotFinderListSuperWiFi = 31,
        HotspotFinderListMap = 32,
        HotspotFinderListBack = 33,

        HotspotFinderMapList = 34,
        HotspotFinderMapBack = 35,

        ProfileDetailsSignout = 36,
        ProfileDetailsEdit = 37,
        ProfileDetailsBack = 38,

        ProfileDetailsEditCancel = 39,
        ProfileDetailsEditSave = 40,
        ProfileDetailsEditBack = 41,

        SettingsTick = 42,
        SettingsBack = 43,

        HowToConnectGotIt = 44, //iOS Exclusive
        HowToConnectGetHelp = 45, //iOS Exclusive

        BuyPackageBuyThis = 46, //Android Exclusive
        //BuyPackageBuyThisTime = 47, //Android Exclusive
        BuyPackageBack = 48, //Android Exclusive

        PurchaseCancel = 49, //Android Exclusive
        PurchaseContinue = 50, //Android Exclusive
        PurchaseBack = 51, //Android Exclusive

        PurchaseConfirmationPay = 52, //Android Exclusive
        PurchaseConfirmationBack = 53, //Android Exclusive

        CloseApplication = 54, //Android Exclusive

        HotspotHelperAuthenticate = 55,
        HotspotHelperNoPackages = 56,
        HotspotHelperHasAlwaysOn = 57,

        LinkAccessPackageCancel = 58,
        LinkAccessPackageLink = 59,
        MenuLinkAccessPackage = 60
    }

    public enum TrackingCategory
    {
        Flow = 1,
        PageView = 2,
        Exception = 3
    }

    public enum TrackingAction
    {
        ButtonPress = 1,
        BackgroundProcess = 2
    }

    public enum PageName
    {

        IntroCarousel = 1,
        Login = 2,
        Registration = 3,
        ForgotPassword = 4,
        Dashboard = 5,
        LinkPackage = 6,
        HotspotFinderList = 7,
        HotspotFinderMap = 8,
        ProfileDetails = 9,
        ProfileDetailsEdit = 10,
        Settings = 11,
        HowToConnect = 12, //iOS Exclusive
        TechnicalSupport = 13,
        TermsAndConditions = 14,
        InfoButton = 15, //iOS Exclusive

        BuyPackage = 16, //Android Exclusive
        Purchase = 17, //Android Exclusive
        PurchaseConfirmation = 18, //Android Exclusive
        Splash = 19,
        LinkAccessCode = 20
    }
}