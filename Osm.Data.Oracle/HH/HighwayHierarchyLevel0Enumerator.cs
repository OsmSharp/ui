//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Graphs;
//using Oracle.DataAccess.Client;
//using Osm.Data.Oracle;
//using Osm.Data;
//using Osm.Routing.Raw.Graphs;

//namespace Osm.Routing.Oracle.HH
//{
//    class HighwayHierarchyLevel0Enumerable : IEnumerable<GraphVertex>
//    {
//        private OracleConnection _connection;

//        private IDataSourceReadOnly _source;

//        public HighwayHierarchyLevel0Enumerable(OracleConnection connection,
//            IDataSourceReadOnly source)
//        {
//            _connection = connection;
//            _source = source;
//        }

//        public IEnumerator<GraphVertex> GetEnumerator()
//        {
//            return new HighwayHierarchyLevel0Enumerator(_connection, _source);
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return new HighwayHierarchyLevel0Enumerator(_connection, _source);
//        }
//    }

//    class HighwayHierarchyLevel0Enumerator : IEnumerator<GraphVertex>
//    {
//        private IDataSourceReadOnly _source;

//        private OracleConnection _connection;

//        private OracleDataReader _reader;

//        private GraphVertex _current;

//        public HighwayHierarchyLevel0Enumerator(OracleConnection connection,
//            IDataSourceReadOnly source)
//        {
//            _connection = connection;
//            _source = source;

//            this.CreateReader();
//        }

//        private void CreateReader()
//        {
//            string sql
//                = "select distinct n.*, wn.way_id "
//                + "from node n "
//                + "inner join way_nodes wn "
//                + "on wn.node_id = n.id "
//                + "inner join way_tags wt "
//                + "on wt.way_id = wn.way_id "
//                + "where key = 'highway' "
//                + "order by wn.way_id "
//                ;
//            OracleCommand com = new OracleCommand(sql);
//            com.Connection = _connection;
//            _reader = com.ExecuteReader();
//        }

//        public GraphVertex Current
//        {
//            get 
//            {
//                return _current;
//            }
//        }

//        public void Dispose()
//        {
//            _reader.Dispose();
//        }

//        object System.Collections.IEnumerator.Current
//        {
//            get 
//            { 
//                return this.Current; 
//            }
//        }

//        public bool MoveNext()
//        {
//            if (_reader.Read())
//            {
//                long returned_id = _reader.GetInt64(0);

//                // get the node from the data source.
//                // TODO: improve on this by getting the data from this connection more quickly!
//                _current = new GraphVertex(_source.GetNode(returned_id));
//                return true;
//            }
//            return false;
//        }

//        public void Reset()
//        {
//            _reader.Dispose();

//            this.CreateReader();
//        }
//    }
//}
