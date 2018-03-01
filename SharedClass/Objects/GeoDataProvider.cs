using Plugin.Geolocator;
using System;
using System.Threading.Tasks;

namespace AlwaysOn
{
    public class GeoDataProvider
    {
        public static void GetCurrentPosition(Action<RadiusCoordinates> Callback)
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        var locator = CrossGeolocator.Current;
                        locator.DesiredAccuracy = 50;

                        var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(3000), null, true);

                        Callback?.Invoke(new RadiusCoordinates(position.Latitude, position.Longitude));
                    }
                    catch (Exception ex)
                    {

                    }
                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
            }
        }
    }
}

