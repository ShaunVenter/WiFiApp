using AlwaysOn.Objects;
using AlwaysOn_iOS.Objects;
using Foundation;
using Google.Analytics;
using UIKit;
using UserNotifications;

namespace AlwaysOn_iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        public const string ApiKey = "1osa9913-av27-1ala-7amg-60a2ancjoke2";

        // class-level declarations
        const string MapsApiKey = "AIzaSyBt9T4RyDi7D9bWmaJgg7sxT5l7z4Gx40U";

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Gai.SharedInstance.DispatchInterval = 20;
            Gai.SharedInstance.TrackUncaughtExceptions = true;
            Gai.SharedInstance.GetTracker(ConfigurationProvider.GATrackingId);

            XamSvg.Setup.InitSvgLib();

            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
            //System.Net.ServicePointManager.DnsRefreshTimeout = 0;

            Google.Maps.MapServices.ProvideAPIKey(MapsApiKey);

            HotspotHelper hh = new HotspotHelper();

            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.MakeKeyAndVisible();
            Window.RootViewController = new UINavigationController(new SplashScreen()) { NavigationBarHidden = true };

            return true;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.

            // Games should use this method to pause the game.

            UIViewController vc = application.KeyWindow.RootViewController;

        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.

            UIViewController currentControllerName = ((UINavigationController)this.Window.RootViewController).VisibleViewController;

            if (currentControllerName.NibName == "Dashboard")
            {
                UtilProvider.LoadDashBoard(currentControllerName);
            }
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
            AlwaysOnNotifications.RemoveAll(UNUserNotificationCenter.Current);
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // show an alert
            UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            Window.RootViewController.PresentViewController(okayAlertController, true, null);

            UIViewController currentControllerName = ((UINavigationController)this.Window.RootViewController).VisibleViewController;
            if (currentControllerName.NibName == "Dashboard")
            {
                UtilProvider.LoadDashBoard(currentControllerName);
            }

            AlwaysOnNotifications.RemoveAll(UNUserNotificationCenter.Current);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}


