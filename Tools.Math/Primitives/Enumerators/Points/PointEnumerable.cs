using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Primitives.Enumerators.Points
{
    public sealed class PointEnumerable<PointType> : IEnumerable<PointType>
        where PointType : PointF2D
    {
        private PointEnumerator<PointType> _enumerator;

        internal PointEnumerable(PointEnumerator<PointType> enumerator)
        {
            _enumerator = enumerator;
        }

        #region IEnumerable<PointType> Members

        public IEnumerator<PointType> GetEnumerator()
        {
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }

        #endregion
    }
}
