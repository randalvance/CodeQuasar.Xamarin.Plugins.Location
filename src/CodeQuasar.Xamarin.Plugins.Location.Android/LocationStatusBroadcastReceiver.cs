using Android.App;
using Android.Content;
using Android.Locations;
using CodeQuasar.Xamarin.Plugins.Location.Abstractions;
using Xamarin.Forms;

namespace CodeQuasar.Xamarin.Plugins.Location
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { LocationManager.ProvidersChangedAction })]
    public class LocationStatusBroadcastReceiver : BroadcastReceiver, ILocationStatusSource
    {
        private static bool? PreviousState = null;

        public override void OnReceive(Context context, Intent intent)
        {
            LocationManager manager = (LocationManager)context.GetSystemService(Context.LocationService);
            var locationEnabled = manager.IsProviderEnabled(LocationManager.GpsProvider);
            
            // Circumvent android bug where broadcast receiver is called twice on some divices
            if (!PreviousState.HasValue || PreviousState.Value != locationEnabled)
            {
                PreviousState = locationEnabled;

                MessagingCenter.Send<ILocationStatusSource>(this, MessengerKeys.LocationStatusBroadcastReceived);
            }
        }
    }
}