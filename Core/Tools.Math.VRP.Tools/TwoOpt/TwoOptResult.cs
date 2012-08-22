using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.TwoOpt
{
    public class TwoOptResult
    {
        /// <summary>
        /// The best possible change when doing two opt.
        /// </summary>
        public float Change { get; internal set; }

        /// <summary>
        /// The first customer of the first new edge.
        /// </summary>
        public int Edge1Customer1 { get; set; }

        /// <summary>
        /// The second customer of the first new edge.
        /// </summary>
        public int Edge1Customer2 { get; set; }

        /// <summary>
        /// The first customer of the second new edge.
        /// </summary>
        public int Edge2Customer1 { get; set; }

        /// <summary>
        /// The second customer of the second new edge.
        /// </summary>
        public int Edge2Customer2 { get; set; }
    }
}
