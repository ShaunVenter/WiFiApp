using Android.OS;

namespace AlwaysOn_Droid
{
    public enum State { Disabled = 0, Enabled = 1, ConnectedToSSID = 2, Authenticated = 3, SSIDAvailable = 4, Reconnecting = 5, None = 9 }
    public enum UserInfoOperation { current_usage, active_sessions, user_details }
    
    public partial class HotspotHelperConsts
    {
        public static bool IsLollipop { get; set; } = Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
        public static bool IsMarshmallow { get; set; } = Build.VERSION.SdkInt >= BuildVersionCodes.M;
        public static string WisprTAG { get; set; } = "WISPAccessGatewayParam";
    }
}