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
        public override void OnReceive(Context context, Intent intent)
        {
            MessagingCenter.Send<ILocationStatusSource>(this, MessengerKeys.LocationStatusBroadcastReceived);
        }
    }
}