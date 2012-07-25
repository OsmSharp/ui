using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Primitives.Enumerators.Lines;

namespace Tools.Math.Primitives.Enumerators.Points
{
    public sealed class LineEnumerable<PointType> : IEnumerable<GenericLineF2D<PointType>>
        where PointType : PointF2D
    {
        private LineEnumerator<PointType> _enumerator;

        internal LineEnumerable(LineEnumerator<PointType> enumerator)
        {
            _enumerator = enumerator;
        }

        #region IEnumerable<PointType> Members

        public IEnumerator<GenericLineF2D<PointType>> GetEnumerator()
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
