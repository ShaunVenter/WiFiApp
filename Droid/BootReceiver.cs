using Android.App;
using Android.Content;
using Android.OS;

namespace AlwaysOn_Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true, Name = "com.is.alwayson.BootReceiver")]
    [IntentFilter(new[] { Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, "android.intent.action.QUICKBOOT_POWERON" })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            HotspotService.StartServiceIfNotRunning(context);
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true, Name = "com.is.alwayson.AlarmReceiver", Process = ":alwaysonalarm")]
    public class AlarmReceiver : BroadcastReceiver
    {
        public static int RequestCode = 466664;
        public override void OnReceive(Context context, Intent intent)
        {
            //var hotspotServiceBinder = PeekService(context, HotspotService.GetIntent(context));

            HotspotService.StartServiceIfNotRunning(context);
        }

        public static void ScheduleAlarm(Context context)
        {
            var alarmTime = 60000; //1minute
            var pIntent = PendingIntent.GetBroadcast(context, AlarmReceiver.RequestCode, new Intent(context, typeof(AlarmReceiver)), PendingIntentFlags.UpdateCurrent);
            var alarm = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarm.SetInexactRepeating(AlarmType.RtcWakeup, 0, alarmTime, pIntent);
        }

        public override IBinder PeekService(Context context, Intent service)
        {
            return base.PeekService(context, service);
        }
    }

    [Service(DirectBootAware = true, Enabled = true, Icon = "@drawable/Icon", Name = "com.is.alwayson.HotspotService")]
    public class HotspotService : Service
    {
        static bool Executing = false;
        Handler handler;

        private void RegisterHotspotHelper()
        {
            if (Executing) return;
            Executing = true;

            AlarmReceiver.ScheduleAlarm(this);

            HotspotHelper.RegisterHotspotHelper(this);

            if (handler == null)
            {
                handler = new Handler(Looper.MainLooper);
            }

            handler.PostDelayed(() =>
            {
                Executing = false;
                RegisterHotspotHelper();
            }, 60000);
        }

        public override void OnCreate()
        {
            RegisterHotspotHelper();
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            RegisterHotspotHelper();
            base.OnDestroy();
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            RegisterHotspotHelper();
        }

        public override void OnRebind(Intent intent)
        {
            RegisterHotspotHelper();
            base.OnRebind(intent);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            RegisterHotspotHelper();
            return StartCommandResult.Sticky | StartCommandResult.StickyCompatibility;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            RegisterHotspotHelper();
            base.OnTaskRemoved(rootIntent);
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            RegisterHotspotHelper();
            base.OnTrimMemory(level);
        }

        public override bool OnUnbind(Intent intent)
        {
            return base.OnUnbind(intent);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public static bool IsRunning(Context context)
        {
            ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            foreach (ActivityManager.RunningServiceInfo service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName == "com.is.alwayson.HotspotService")
                {
                    return true;
                }
            }
            return false;
        }

        public static void StartService(Context context)
        {
            context.StartService(new Intent(context, typeof(HotspotService)));
        }

        public static void StartServiceIfNotRunning(Context context)
        {
            if (!IsRunning(context))
            {
                StartService(context);
            }
            else
            {
                context.SendBroadcast(new Intent("com.is.alwayson.hotspotservice.keepalive")); //HotspotHelper
            }
        }

        public static Intent GetIntent(Context context)
        {
            return new Intent(context, typeof(HotspotService));
        }
    }
}