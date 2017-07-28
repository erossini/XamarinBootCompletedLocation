using System;
using Android.OS;

namespace LocationTest.Droid.BackgroudServices
{
    /// <summary>
    /// GPSS ervice binder.
    /// </summary>
    public class GPSServiceBinder : Binder
    {
        /// <summary>
        /// The location service.
        /// </summary>
		protected GPSService LocService;

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="T:LocationTest.Droid.BackgroudServices.GPSServiceBinder"/> is bound.
        /// </summary>
        /// <value><c>true</c> if is bound; otherwise, <c>false</c>.</value>
		public bool IsBound { get; set; }
		
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        public GPSService Service { 
            get { 
                return this.LocService; 
            } 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LocationTest.Droid.BackgroudServices.GPSServiceBinder"/> class.
        /// </summary>
        /// <param name="service">Service.</param>
        public GPSServiceBinder(GPSService service) { 
            this.LocService = service; 
        }
    }
}