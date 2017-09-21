using System;
using System.Threading.Tasks;

namespace CodeQuasar.Xamarin.Plugins.Location.Abstractions
{
    public interface ILocationStatusListener
    {
        event EventHandler<LocationStatusChangedEventArgs> LocationStatusChanged;

        Task<bool> StartListeningAsync();

        Task<bool> StopListeningAsync();

        bool IsListening { get; }

        bool IsLocationEnabled { get; }
    }
}
