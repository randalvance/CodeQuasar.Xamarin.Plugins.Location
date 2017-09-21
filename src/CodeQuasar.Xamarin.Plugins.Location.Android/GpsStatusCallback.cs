using Android.Locations;
using System;

namespace CodeQuasar.Xamarin.Plugins.Location
{
    public class GpsStatusCallback : GnssStatus.Callback
    {
        public event EventHandler Started;

        public event EventHandler Stopped;

        public override void OnStarted()
        {
            base.OnStarted();

            Started?.Invoke(this, null);
        }

        public override void OnStopped()
        {
            base.OnStopped();

            Stopped?.Invoke(this, null);
        }
    }
}