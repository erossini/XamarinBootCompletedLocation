using System;
namespace LocationTest.Droid.Models
{
    /// <summary>
    /// Coordinates.
    /// </summary>
	public class Coordinates : EventArgs
	{
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>The latitude.</value>
		public double latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>The longitude.</value>
		public double longitude { get; set; }
	}
}