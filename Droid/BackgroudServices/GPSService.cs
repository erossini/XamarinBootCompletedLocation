using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Util;
using LocationTest.Droid.BackgroudServices.Models;

namespace LocationTest.Droid.BackgroudServices
{
    [Service]
    public class GPSService : Service, ILocationListener
    {
        // A notification requires an id that is unique to the application.
        const int NOTIFICATION_ID = 9000;

        // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
        const int pendingIntentId = 0;
        PendingIntent pendingIntent;

        /// <summary>
        /// The tag definition
        /// </summary>
        private static readonly string TAG = typeof(GPSService).FullName;

        /// <summary>
        /// The source address.
        /// </summary>
        private const string _sourceAddress = "8 Blomfield Court, Maida Vale, W91TS, London, United Kingdom";
        private string _location = string.Empty;
        private string _address = string.Empty;
        private string _remarks = string.Empty;

        public const string LOCATION_UPDATE_ACTION = "LOCATION_UPDATED";
        private Location _currentLocation;
        IBinder _binder;
        protected LocationManager _locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(LocationService);

		/// <summary>
		/// Is the service already started?
		/// </summary>
		bool isStarted;

		/// <summary>
		/// Ons the bind.
		/// </summary>
		/// <returns>The bind.</returns>
		/// <param name="intent">Intent.</param>
		public override IBinder OnBind(Intent intent)
        {
            Log.WriteLine(LogPriority.Debug, TAG, "OnBind");
            _binder = new GPSServiceBinder(this);
            StartNotification(intent);
            return _binder;
        }

        /// <summary>
        /// Ons the create.
        /// </summary>
        public override void OnCreate()
        {
            Log.Info(TAG, "OnCreate is called.");
            DispatchNotificationThatServiceIsRunning();
            StartLocationUpdates();

            Log.Info(TAG, "OnCreate: GPSService is initializing.");
            base.OnCreate();
        }

        /// <summary>
        /// On the destroy.
        /// </summary>
        public override void OnDestroy()
        {
            // We need to shut things down.
            Log.Info(TAG, "OnDestroy: The started service is shutting down.");

            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(NOTIFICATION_ID);

            base.OnDestroy();
        }

        /// <summary>
        /// Starts the notification.
        /// </summary>
        /// <param name="intent">Intent.</param>
        public void StartNotification(Intent intent)
        {
            DispatchNotificationThatServiceIsRunning();
            pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
        }

        /// <summary>
        /// Ons the start command.
        /// </summary>
        /// <returns>The start command.</returns>
        /// <param name="intent">Intent.</param>
        /// <param name="flags">Flags.</param>
        /// <param name="startId">Start identifier.</param>
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.WriteLine(LogPriority.Debug, TAG, "OnStartCommand");

            if (isStarted)
            {
                Log.Info(TAG, "OnStartCommand: This service has already been started.");
            }
            else
            {
                StartLocationUpdates();
                StartNotification(intent);
                isStarted = true;
            }

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Starts the location updates.
        /// </summary>
        public void StartLocationUpdates()
        {
            Log.WriteLine(LogPriority.Debug, TAG, "StartLocationUpdates");

            Criteria criteriaForGPSService = new Criteria
            {
                // A constant indicating an approximate accuracy  
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Low
            };

            var locationProvider = _locationManager.GetBestProvider(criteriaForGPSService, true);
            _locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }

        /// <summary>
        /// Occurs when location changed.
        /// </summary>
        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };

        /// <summary>
        /// On the location changed.
        /// </summary>
        /// <param name="location">Location.</param>
        public void OnLocationChanged(Location location)
        {
            Log.WriteLine(LogPriority.Debug, TAG, "OnLocationChanged");

            try
            {
                _currentLocation = location;

                if (_currentLocation == null)
                {
                    _location = "Unable to determine your location.";
                    Log.WriteLine(LogPriority.Debug, TAG, "Unable to determine your location.");
                }
                else
                {
                    _location = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
                    Log.WriteLine(LogPriority.Debug, TAG, _location);

                    Geocoder geocoder = new Geocoder(this);

                    //The Geocoder class retrieves a list of address from Google over the internet  
                    IList<Address> addressList = geocoder.GetFromLocation(_currentLocation.Latitude, _currentLocation.Longitude, 10);

                    Address addressCurrent = addressList.FirstOrDefault();

                    if (addressCurrent != null)
                    {
                        StringBuilder deviceAddress = new StringBuilder();

                        for (int i = 0; i < addressCurrent.MaxAddressLineIndex; i++)
                            deviceAddress.Append(addressCurrent.GetAddressLine(i))
                                .AppendLine(",");

                        _address = deviceAddress.ToString();
                        Log.WriteLine(LogPriority.Debug, TAG, _address);
                    }
                    else
                    {
                        _address = "Unable to determine the address.";
                        Log.WriteLine(LogPriority.Debug, TAG, _address);
                    }

                    IList<Address> source = geocoder.GetFromLocationName(_sourceAddress, 1);
                    Address addressOrigin = source.FirstOrDefault();

                    var coord1 = new Coords(addressOrigin.Latitude, addressOrigin.Longitude);
                    var coord2 = new Coords(addressCurrent.Latitude, addressCurrent.Longitude);

                    var distanceInRadius = Utils.HaversineDistance(coord1, coord2, Utils.DistanceUnit.Miles);

                    _remarks = string.Format("You are {0} miles away from your original location.", distanceInRadius);
                    Log.WriteLine(LogPriority.Debug, TAG, _remarks);

                    Intent intent = new Intent(this, typeof(GPSServiceReceiver));
                    intent.SetAction(GPSServiceReceiver.LOCATION_UPDATED);
                    intent.AddCategory(Intent.CategoryDefault);
                    intent.PutExtra("Location", _location);
                    intent.PutExtra("Address", _address);
                    intent.PutExtra("Remarks", _remarks);
                    SendBroadcast(intent);

                    UpdateNotification($"You are in " + _address);
                }
            }
            catch (Exception ex)
            {
                _address = "Unable to determine the address.";
                Log.WriteLine(LogPriority.Debug, TAG, _address + ex.Message);
            }
        }

        /// <summary>
        /// On the status changed.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="status">Status.</param>
        /// <param name="extras">Extras.</param>
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            switch (status)
            {
                case Availability.OutOfService:
                    UpdateNotification("Location service is out of service.");
                    break;
                case Availability.TemporarilyUnavailable:
                    UpdateNotification("Location service is temporarily unavailable.");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// On the provider disabled.
        /// </summary>
        /// <param name="provider">Provider.</param>
        public void OnProviderDisabled(string provider)
        {
            Log.WriteLine(LogPriority.Debug, TAG, $"Location service {provider} is disabled.");
            UpdateNotification($"Location service {provider} is disabled.");
        }

        /// <summary>
        /// Ons the provider enabled.
        /// </summary>
        /// <param name="provider">Provider.</param>
        public void OnProviderEnabled(string provider)
        {
            Log.WriteLine(LogPriority.Debug, TAG, $"Location service {provider} is enabled.");
            UpdateNotification($"Location service {provider} is enabled.");
        }

        #region Notification
        void DispatchNotificationThatServiceIsRunning()
        {
            Notification.Builder notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                .SetContentTitle("LocationTest")
                .SetContentText("Localization is running!");

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
        }

        void UpdateNotification(string content)
        {
            if (pendingIntent != null)
            {
                var notification = GetNotification(content, pendingIntent);

                NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Notify(NOTIFICATION_ID, notification);
            }
        }

        Notification GetNotification(string content, PendingIntent intent)
        {
            return new Notification.Builder(this)
                                   .SetContentTitle(TAG)
                                   .SetContentText(content)
                                   .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                                   .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon))
                                   .SetContentIntent(intent).Build();
        }
        #endregion
    }
}