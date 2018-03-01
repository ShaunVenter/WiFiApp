using Google.Analytics;
using System.Threading.Tasks;

namespace AlwaysOn_iOS
{
    public class AnalyticsProvider_iOS
    {
        public static void TrackEventGA(string category, string action, string label)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var gaEvent = DictionaryBuilder.CreateEvent(category, action, label, null).Build();
                    Gai.SharedInstance.DefaultTracker.Send(gaEvent);
                    Gai.SharedInstance.Dispatch();
                }, TaskCreationOptions.LongRunning);
            }
            catch { }
        }

        public static void PageViewGA(string PageName)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, PageName);
                    Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
                }, TaskCreationOptions.LongRunning);
            }
            catch { }
        }
    }
}

