//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Graphs;
//using Oracle.DataAccess.Client;
//using Osm.Routing.Raw.Graphs;

//namespace Osm.Data.Oracle.HH
//{
//    internal class GraphVertexEnumerable : IEnumerable<GraphVertex>
//    {
//        private OracleDataReader _reader;

//        private IEnumerator<GraphVertex> _enumerator;

//        public GraphVertexEnumerable(OracleDataReader reader)
//        {
//            _reader = reader;
//        }

//        #region IEnumerable<GraphVertex> Members

//        public IEnumerator<GraphVertex> GetEnumerator()
//        {
//            if (_enumerator == null)
//            {
//                _enumerator = new GraphVertexEnumerator(_reader);
//            }
//            else
//            {
//                _enumerator.Reset();
//            }
//            return _enumerator;
//        }

//        #endregion

//        #region IEnumerable Members

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

//    internal class GraphVertexEnumerator : IEnumerator<GraphVertex>
//    {
//        private OracleDataReader _reader;

//        public GraphVertexEnumerator(OracleDataReader reader)
//        {
//            _reader = reader;
//        }
        
//        public GraphVertex Current
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        object System.Collections.IEnumerator.Current
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public bool MoveNext()
//        {
//            throw new NotImplementedException();
//        }

//        public void Reset()
//        {

//        }
//    }
//}
