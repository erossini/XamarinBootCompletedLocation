using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using LocationTest.Droid.BackgroudServices;
using LocationTest.Droid.Receivers;

namespace LocationTest.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        private static readonly string TAG = typeof(BootReceiver).FullName;

        public override void OnReceive(Context context, Intent intent)
        {
            Log.WriteLine(LogPriority.Debug, TAG, "BootReceiver OnReceive");

            Intent i = new Intent(context, typeof(GPSService));
            i.AddFlags(ActivityFlags.NewTask);
            context.StartService(i);
            Log.WriteLine(LogPriority.Debug, TAG, "BootReceiver OnReceive LocationService started");

			//i = new Intent(context, typeof(TimerService));
			//i.AddFlags(ActivityFlags.NewTask);
			//context.StartService(i);
			//Log.WriteLine(LogPriority.Debug, TAG, "BootReceiver OnReceive TimerService started");
		}
    }
}