using System;
using Android.App;
using Android.Content;
using Android.Widget;

namespace LocationTest.Droid.BackgroundServices
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();

            Intent i = new Intent(context, typeof(LocationService));
            i.AddFlags(ActivityFlags.NewTask);
            context.StartService(i);
        }
    }
}