using Android.App;
using Android.Content;
using Android.Locations;
using CodeQuasar.Xamarin.Plugins.Location.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeQuasar.Xamarin.Plugins.Location
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { LocationManager.ProvidersChangedAction })]
    public class LocationStatusBroadcastReceiver : BroadcastReceiver, ILocationStatusSource
    {
        private static bool? oldLocationEnabled = null;

        public override void OnReceive(Context context, Intent intent)
        {
            var locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
            var newLocationEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

            if (!oldLocationEnabled.HasValue || oldLocationEnabled.Value != newLocationEnabled)
            {
                oldLocationEnabled = newLocationEnabled;
                MessagingCenter.Send<ILocationStatusSource>(this, MessengerKeys.LocationStatusBroadcastReceived);
            }
        }
    }
}