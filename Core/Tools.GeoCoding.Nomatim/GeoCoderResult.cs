using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.GeoCoding.Nomatim
{
    public class GeoCoderResult : IGeoCoderResult
    {
        #region IGeoCoderResult Members

        public double Latitude
        {
            get;
            set;
        }

        public double Longitude
        {
            get;
            set;
        }

    		public string Text { get; set;  }

        public AccuracyEnum Accuracy
        {
            get;
            set;
        }

        #endregion
    }
}
