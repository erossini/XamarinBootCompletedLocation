using System;
using Android.Content;
using Android.Util;

namespace LocationTest.Droid.BackgroudServices
{
    /// <summary>
    /// GPSS ervice receiver.
    /// </summary>
    [BroadcastReceiver]
    internal class GPSServiceReceiver : BroadcastReceiver
    {
        public MainActivity _mainActivity;

        public static readonly string LOCATION_UPDATED = "LOCATION_UPDATED";
        private static readonly string TAG = typeof(GPSServiceReceiver).FullName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LocationTest.Droid.BackgroudServices.GPSServiceReceiver"/> class.
        /// </summary>
        public GPSServiceReceiver() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LocationTest.Droid.BackgroudServices.GPSServiceReceiver"/> class.
        /// </summary>
        /// <param name="activity">Activity.</param>
        public GPSServiceReceiver(MainActivity activity) : base()
        {
            _mainActivity = activity;
        }

        /// <summary>
        /// Ons the receive.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="intent">Intent.</param>
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(LOCATION_UPDATED))
            {
                if (_mainActivity != null)
                    _mainActivity.UpdateUI(intent);
                else {
                    Log.WriteLine(LogPriority.Debug, TAG, "GPSServiceReceiver: Location updated: " + intent.GetStringExtra("Location"));
                }
            }
        }
    }
}