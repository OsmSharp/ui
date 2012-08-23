using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.GeoCoding
{
    /// <summary>
    /// Describes what a geo coder result should be.
    /// </summary>
    public interface IGeoCoderResult
    {
        /// <summary>
        /// The latitude result.
        /// </summary>
        double Latitude
        {
            get;
        }

        /// <summary>
        /// The longitude result.
        /// </summary>
        double Longitude
        {
            get;
        }

				/// <summary>
				/// Text value of geocoder
				/// </summary>
				string Text { get; }

        /// <summary>
        /// The accuracy of the result.
        /// </summary>
        AccuracyEnum Accuracy
        {
            get;
        }
    }
}
