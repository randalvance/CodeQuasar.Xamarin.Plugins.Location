using Android.Content;
using Android.Locations;
using CodeQuasar.Xamarin.Plugins.Location.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeQuasar.Xamarin.Plugins.Location
{
    public class LocationStatusListener : ILocationStatusListener, ILocationStatusSource
    {
        private LocationManager _locationManager;

        private bool _isListening = false;

        private string[] Providers => Manager.GetProviders(enabledOnly: false).ToArray();

        private string[] IgnoredProviders => new string[] { LocationManager.PassiveProvider, "local_database" };

        private LocationManager Manager
        {
            get
            {
                if (_locationManager == null)
                    _locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);

                return _locationManager;
            }
        }

        public event EventHandler<LocationStatusChangedEventArgs> LocationStatusChanged;

        public bool IsLocationEnabled => Providers.Any(p => !IgnoredProviders.Contains(p) && Manager.IsProviderEnabled(p));

        public bool IsListening => _isListening;

        public async Task<bool> StartListeningAsync()
        {
            if (IsListening)
                throw new InvalidOperationException("Already listening!");
            
            if (!await CheckPermissions())
                return false;

            MessagingCenter.Subscribe<ILocationStatusSource>(this, MessengerKeys.LocationStatusBroadcastReceived, LocationStatusBroadcastReceived);

            _isListening = true;

            return true;
        }

        public async Task<bool> StopListeningAsync()
        {
            return await Task.Run(() =>
            {
                MessagingCenter.Unsubscribe<ILocationStatusListener>(this, MessengerKeys.LocationStatusBroadcastReceived);

                _isListening = false;

                return true;
            });
        }

        private void LocationStatusBroadcastReceived(ILocationStatusSource source)
        {
            LocationStatusChanged?.Invoke(this, new LocationStatusChangedEventArgs() { IsEnabled = this.IsLocationEnabled });
        }

        private async Task<bool> CheckPermissions()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Currently does not have Location permissions, requesting permissions");

                var request = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                if (request[Permission.Location] != PermissionStatus.Granted)
                {
                    Console.WriteLine("Location permission denied, can not get positions async.");
                    return false;
                }
            }

            return true;
        }
    }
}