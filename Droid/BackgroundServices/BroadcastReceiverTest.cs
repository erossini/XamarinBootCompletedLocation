
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LocationTest.Droid.BackgroundServices
{
    [BroadcastReceiver]
    public class BroadcastReceiverTest : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "New Received intent!", ToastLength.Short).Show();
        }
    }
}
