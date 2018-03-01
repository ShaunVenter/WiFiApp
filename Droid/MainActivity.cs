using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views.Animations;
using XamSvg;



namespace AlwaysOn_Droid
{
    [Activity(Label = "AlwaysOn", MainLauncher = true, Icon = "@drawable/Icon", Theme = "@style/SplashTheme", ScreenOrientation = ScreenOrientation.Portrait, DirectBootAware = true, Name = "com.is.alwayson.MainActivity")]
    public class MainActivity : Activity
    {
        SvgImageView img;
        Animation fadeInAnimation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Xamarin.Insights.Initialize(XamarinInsights.ApiKey, this);

            base.OnCreate(savedInstanceState);
            
            AnalyticsProvider_Droid.PageViewGA(this, AlwaysOn.PageName.Splash.ToString());
            
            SetContentView(Resource.Layout.Main);

            img = FindViewById<SvgImageView>(Resource.Id.imgIconLogo2);
            img.SetSvg(this, Resource.Raw.AlwaysOn_logo_Feb16_AllInOne, "ffffff=ffffff");
            img.LayoutParameters.Width = Utils.CalcDimension(img.LayoutParameters.Width);

            BackendProvider_Droid.GetServiceProviders();

            var userObj = BackendProvider_Droid.GetUser;
            if (userObj == null)
            {
                fadeInAnimation = new AlphaAnimation(0, 1) { Duration = 1500 };
                fadeInAnimation.AnimationEnd += (object sender, Animation.AnimationEndEventArgs e) =>
                {
                    var intent = new Intent(this, typeof(Carousel));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    Finish();
                };
                img.StartAnimation(fadeInAnimation);
            }
            else
            {
                img.Alpha = 1;
                
                if (userObj.RememberMe)
                {
                    BackendProvider_Droid.UpdatePackagesFromServer();
                    var intent = new Intent(this, typeof(Dashboard));
                    intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    var carousel = new Intent(this, typeof(Carousel));
                    carousel.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                    StartActivity(carousel);
                    Finish();
                }
            }
        }
    }
}