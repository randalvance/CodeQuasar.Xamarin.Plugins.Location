using CodeQuasar.Xamarin.Plugins.Location.Abstractions;
using System;

namespace CodeQuasar.Xamarin.Plugins.Location
{
    public class CrossLocationStatusListener
    {
        static Lazy<ILocationStatusListener> implementation = new Lazy<ILocationStatusListener>(() => Create(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        public static bool IsSupported => implementation.Value == null ? false : true;

        public static ILocationStatusListener Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ILocationStatusListener Create()
        {
#if PORTABLE
            return null;
#else
            return new LocationStatusListener();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
