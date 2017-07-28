using System;
using Android.Content;
using Android.OS;

namespace LocationTest.Droid.BackgroudServices
{
    /// <summary>
    /// GPSS ervice connection.
    /// </summary>
    public class GPSServiceConnection : Java.Lang.Object, IServiceConnection
    {
        GPSServiceBinder _binder;

        /// <summary>
        /// Occurs when connected.
        /// </summary>
        public event Action Connected;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LocationTest.Droid.BackgroudServices.GPSServiceConnection"/> class.
        /// </summary>
        /// <param name="binder">Binder.</param>
        public GPSServiceConnection(GPSServiceBinder binder)
        {
            if (binder != null)
                this._binder = binder;
        }

        /// <summary>
        /// Ons the service connected.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="service">Service.</param>
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            GPSServiceBinder serviceBinder = (GPSServiceBinder)service;

            if (serviceBinder != null)
            {
                this._binder = serviceBinder;
                this._binder.IsBound = true;
                serviceBinder.Service.StartLocationUpdates();
                if (Connected != null)
                    Connected.Invoke();
            }
        }

        /// <summary>
        /// Ons the service disconnected.
        /// </summary>
        /// <param name="name">Name.</param>
        public void OnServiceDisconnected(ComponentName name) { this._binder.IsBound = false; }
    }
}
