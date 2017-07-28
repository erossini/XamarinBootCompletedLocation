using System;
using Android.App;
using Android.Content;
using Android.Util;
using LocationTest.Droid.BackgroudServices;
using static Android.Provider.Settings;

namespace LocationTest.Droid
{
	[BroadcastReceiver]
    [IntentFilter(new[] { Android.Content.Intent.ActionAirplaneModeChanged })]
	public class AirplaneModeReceiver : BroadcastReceiver
	{
		private static readonly string TAG = typeof(AirplaneModeReceiver).FullName;

		public override void OnReceive(Context context, Intent intent)
		{
			Log.WriteLine(LogPriority.Debug, TAG, "AirplaneModeReceiver OnReceive Mode " + isAirplaneModeOn(context));
		}

		private static bool isAirplaneModeOn(Context context)
		{
			var airplane = Android.Provider.Settings.Global.GetInt(context.ContentResolver, 
                                                                   Android.Provider.Settings.Global.AirplaneModeOn);
			return airplane != 0;
		}
	}
}