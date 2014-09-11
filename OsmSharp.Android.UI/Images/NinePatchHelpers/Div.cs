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
using System.IO;

namespace OsmSharp.Android.UI.Images.NinePatchHelpers
{
    class Div 
    {
        /// <summary>
        /// Starting div point
        /// </summary>
	    public int start;

        /// <summary>
        /// Finish div point. For stretchable areas it will point on next pixel after last Color.BLACK pixel found for this div.
        /// </summary>
        public int stop;

        /// <summary>
        /// Creates a new div.
        /// </summary>
	    public Div() {}

        /// <summary>
        /// Creates a new div.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
	    public Div(int start, int stop) {
		    this.start = start;
		    this.stop = stop;
	    }

	    public void readExternal(Stream input)
        {
		    start = input.ReadByte();
            stop = input.ReadByte();
	    }

	    public void writeExternal(Stream output)
        {
		    output.WriteByte((byte)start);
            output.WriteByte((byte)stop);
	    }
    }
}