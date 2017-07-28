using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;

namespace LocationTest.Droid.Receivers
{
    [Service]
    public class TimerService : Service
    {
        static readonly string TAG = typeof(TimerService).FullName;
        static readonly int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
        static readonly int NOTIFICATION_ID = 10000;

        UtcTimestamper timestamper;
        Handler handler;
        Action runnable;

        /// <summary>
        /// Is the service already started?
        /// </summary>
        bool isStarted;

		// Create a PendingIntent; we're only using one PendingIntent (ID = 0):
		const int pendingIntentId = 0;
        PendingIntent pendingIntent;

        public override void OnCreate()
        {
            DispatchNotificationThatServiceIsRunning();
            Log.Info(TAG, "OnCreate: the service is initializing.");
            base.OnCreate();

            timestamper = new UtcTimestamper();
            handler = new Handler();

            // This Action is only for demonstration purposes.
            runnable = new Action(() =>
                            {
                                if (timestamper != null)
                                {
                                    UpdateNotification(timestamper.GetFormattedTimestamp());
                                    Log.Debug(TAG, timestamper.GetFormattedTimestamp());
                                    handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
                                }
                            });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (isStarted)
            {
                Log.Info(TAG, "OnStartCommand: This service has already been started.");
            }
            else
            {
                Log.Info(TAG, "OnStartCommand: The service is starting.");
                DispatchNotificationThatServiceIsRunning();
                handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
                isStarted = true;
            }

            pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }

        public override void OnDestroy()
        {
            // We need to shut things down.
            Log.Debug(TAG, GetFormattedTimestamp());
            Log.Info(TAG, "OnDestroy: The started service is shutting down.");

            // Stop the handler.
            handler.RemoveCallbacks(runnable);

            // Remove the notification from the status bar.
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(NOTIFICATION_ID);

            timestamper = null;
            isStarted = false;
            base.OnDestroy();
        }

        /// <summary>
        /// This method will return a formatted timestamp to the client.
        /// </summary>
        /// <returns>A string that details what time the service started and how long it has been running.</returns>
        string GetFormattedTimestamp()
        {
            return timestamper?.GetFormattedTimestamp();
        }

        void DispatchNotificationThatServiceIsRunning()
        {
            Notification.Builder notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                .SetContentTitle("LocationTest")
                .SetContentText("TimeService is running");

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
        }

        void UpdateNotification(string content)
        {
            var notification = GetNotification(content, pendingIntent);

            NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(NOTIFICATION_ID, notification);
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
    }
}