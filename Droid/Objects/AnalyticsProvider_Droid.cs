using AlwaysOn.Objects;
using Android.Content;
using Android.Gms.Analytics;
using System.Threading.Tasks;

namespace AlwaysOn_Droid
{
    public class AnalyticsProvider_Droid
    {
        public static void TrackEventGA(Context context, string category, string action, string label)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var GoogleAnal = GoogleAnalytics.GetInstance(context);
                    if (!GoogleAnal.IsInitialized)
                    {
                        GoogleAnal.Initialize();
                    }
                    var GoogleTrack = GoogleAnal.NewTracker(ConfigurationProvider.GATrackingId);
                    GoogleTrack.Send(new HitBuilders.EventBuilder(category, action).SetLabel(label).Build());
                    GoogleAnal.DispatchLocalHits();
                }, TaskCreationOptions.LongRunning);
            }
            catch { }
        }

        public static void PageViewGA(Context context, string PageName)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var GoogleAnal = GoogleAnalytics.GetInstance(context);
                    if (!GoogleAnal.IsInitialized)
                    {
                        GoogleAnal.Initialize();
                    }
                    var GoogleTrack = GoogleAnal.NewTracker(ConfigurationProvider.GATrackingId);
                    GoogleTrack.SetScreenName(PageName);
                    GoogleTrack.Send(new HitBuilders.ScreenViewBuilder().Build());
                    GoogleAnal.DispatchLocalHits();
                }, TaskCreationOptions.LongRunning);
            }
            catch { }
        }
    }
}