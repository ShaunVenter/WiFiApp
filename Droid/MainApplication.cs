using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using Android.Content;
using Android.Net.Wifi;
using Android.Net;
using System.Net;

namespace AlwaysOn_Droid
{
    //You can specify additional application information in this attribute
    [Application(Name = "com.is.alwayson.MainApplication")]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public const string ApiKey = "4ndr0108-av27-1ala-7amg-60a2ancjoke2";

        public MainApplication(IntPtr handle, JniHandleOwnership transer) : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };

            HotspotService.StartServiceIfNotRunning(this);

            RegisterActivityLifecycleCallbacks(this);
            //A great place to initialize Xamarin.Insights and Dependency Services!
        }

        public override void OnTerminate()
        {
            base.OnTerminate();

            HotspotService.StartServiceIfNotRunning(this);

            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
            HotspotService.StartServiceIfNotRunning(this);
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;

            // Clear the Notification Bar after you've clicked on the message in the Notification Bar
            NotificationManager nMgr = (NotificationManager)GetSystemService(Context.NotificationService);
            nMgr.CancelAll();
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
            HotspotService.StartServiceIfNotRunning(this);
        }
    }
}