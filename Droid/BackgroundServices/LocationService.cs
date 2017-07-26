using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using LocationTest.Droid.Models;

namespace LocationTest.Droid.BackgroundServices
{
    [Service(Name="pro.wordbank.app.service")]
    public class LocationService : Service, ILocationListener
    {
        //string tg = "LocationService";

		// A notification requires an id that is unique to the application.
		const int NOTIFICATION_ID = 9000;

		LocationManager locationManager;
		Location newLocation;
		
        event EventHandler<Coordinates> MyLocation;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {
			// Work has finished, now dispatch anotification to let the user know.
			Notification.Builder notificationBuilder = new Notification.Builder(this)
				.SetContentTitle("Background service")
				.SetContentText("OnStartCommand");

			var notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());

            return StartCommandResult.Sticky;
        }

        public void OnLocationChanged(Location location)
        {
			if (location != null)
			{
				// Create an instance of our Coordinates 
				var coords = new Coordinates();

				// Assign our user's Latitude and Longitude   
				// values 
				coords.latitude = location.Latitude;
				coords.longitude = location.Longitude;

				// Update our new location to store the  
				// new details. 
				newLocation = new Location("Point A");
				newLocation.Latitude = coords.latitude;
				newLocation.Longitude = coords.longitude;

				// Pass the new location details to our  
				// Location Service EventHandler. 
				MyLocation(this, coords);

                Android.Util.Log.WriteLine(Android.Util.LogPriority.Debug, "LocationService", $"New location lat: {newLocation.Latitude} - long: {newLocation.Longitude}");
			};
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }

		~LocationService()
		{
			locationManager.RemoveUpdates(this);
		}
	}
}
