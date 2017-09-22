# CodeQuasar.Xamarin.Plugins.Location
[![Build status](https://ci.appveyor.com/api/projects/status/fdptnvdb1690aonr?svg=true)](https://ci.appveyor.com/project/randalvance/codequasar-xamarin-plugins-location)

# NuGet
[![NuGet](https://img.shields.io/nuget/v/CodeQuasar.Xamarin.Plugins.Location.svg)](https://www.nuget.org/packages/CodeQuasar.Xamarin.Plugins.Location)

# Listening to Location Status Change
**Example:**
```csharp
CrossLocationStatusListener.Current.LocationStatusChanged += OnLocationStatusChanged;
CrossLocationStatusListener.Current.StartListeningAsync();
```
