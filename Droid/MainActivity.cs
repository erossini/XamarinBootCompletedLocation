using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LocationTest.Droid.BackgroudServices;
using Android.Util;
using LocationTest.Droid.Receivers;

namespace LocationTest.Droid
{
    [Activity(Label = "LocationTest.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static readonly string TAG = typeof(MainActivity).FullName;

		GPSServiceBinder _binder;
		GPSServiceConnection _gpsServiceConnection;
		Intent _gpsServiceIntent;
		private GPSServiceReceiver _receiver;

        // TimeService Test
		static readonly string SERVICE_STARTED_KEY = "has_service_been_started";
		Intent serviceToStart;
		bool isStarted = false;

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // register location provider
			RegisterGPSService();
            RegisterGPSBroadcastReceiver();

			if (bundle != null)
			{
                isStarted = bundle.GetBoolean(SERVICE_STARTED_KEY, false);
			}

            // Start TimeService
            //TimerService ts = new TimerService();
			//serviceToStart = new Intent(this, typeof(TimerService));
			//serviceToStart.AddFlags(ActivityFlags.NewTask);
			//StartTimerService();

			LoadApplication(new App());
        }

        /// <summary>
        /// Ons the resume.
        /// </summary>
		protected override void OnResume()
		{
			base.OnResume();
			RegisterGPSBroadcastReceiver();
		}

        /// <summary>
        /// Ons the pause.
        /// </summary>
		protected override void OnPause()
		{
			base.OnPause();
			UnRegisterGPSBroadcastReceiver();
		}

        /// <summary>
        /// Registers the service.
        /// </summary>
        private void RegisterGPSService()
		{
			_gpsServiceConnection = new GPSServiceConnection(_binder);
			_gpsServiceIntent = new Intent(Android.App.Application.Context, typeof(GPSService));
			BindService(_gpsServiceIntent, _gpsServiceConnection, Bind.AutoCreate);
            Log.WriteLine(LogPriority.Debug, TAG, "RegisterService");
		}

        /// <summary>
        /// Registers the broadcast receiver.
        /// </summary>
        private void RegisterGPSBroadcastReceiver()
		{
			IntentFilter filter = new IntentFilter(GPSServiceReceiver.LOCATION_UPDATED);
			filter.AddCategory(Intent.CategoryDefault);
			_receiver = new GPSServiceReceiver(this);
			RegisterReceiver(_receiver, filter);
            Log.WriteLine(LogPriority.Debug, TAG, "RegisterBroadcastReceiver");
		}

        /// <summary>
        /// Uns the register broadcast receiver.
        /// </summary>
        private void UnRegisterGPSBroadcastReceiver()
		{
			UnregisterReceiver(_receiver);
		}

        /// <summary>
        /// Updates the user interface.
        /// </summary>
        /// <param name="intent">Intent.</param>
		public void UpdateUI(Intent intent)
		{
			//_locationText.Text = intent.GetStringExtra("Location");
			//_addressText.Text = intent.GetStringExtra("Address");
			//_remarksText.Text = intent.GetStringExtra("Remarks");
            Log.WriteLine(LogPriority.Debug, TAG, "UpdateUI");
		}

		void StartTimerService()
		{
			StartService(serviceToStart);

			Log.Info(TAG, "TimerService: StartTimerService is started.");
			isStarted = true;
		}

		void StopTimerService()
		{
			Log.Info(TAG, "TimerService: StopTimerService is stopped.");
			StopService(serviceToStart);
			isStarted = false;
		}
	}
}