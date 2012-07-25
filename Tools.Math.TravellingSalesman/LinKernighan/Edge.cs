using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.TSP.LK
{
    struct Edge
    {
        public int From { get; set; }
        public int To { get; set; }
        public float Weight { get; set; }

        public override string ToString()
        {
            return string.Format("{0}->{1}:{2}",
                this.From,
                this.To,
                this.Weight);
        }
    }
}
