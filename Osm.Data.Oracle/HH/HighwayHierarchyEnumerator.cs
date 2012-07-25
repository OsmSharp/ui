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
//    class HighwayHierarchyEnumerable : IEnumerable<GraphVertex>
//    {
//        private int _level;

//        private OracleConnection _connection;

//        private IDataSourceReadOnly _source;

//        public HighwayHierarchyEnumerable(OracleConnection connection,
//            IDataSourceReadOnly source, int level)
//        {
//            _level = level;
//            _connection = connection;
//            _source = source;
//        }

//        public IEnumerator<GraphVertex> GetEnumerator()
//        {
//            return new HighwayHierarchyEnumerator(_connection, _source, _level);
//        }

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return new HighwayHierarchyEnumerator(_connection, _source, _level);
//        }
//    }

//    class HighwayHierarchyEnumerator : IEnumerator<GraphVertex>
//    {
//        private int _level;

//        private IDataSourceReadOnly _source;

//        private OracleConnection _connection;

//        private OracleDataReader _reader;

//        private GraphVertex _current;

//        public HighwayHierarchyEnumerator(OracleConnection connection,
//            IDataSourceReadOnly source, int level)
//        {
//            _level = level;
//            _connection = connection;
//            _source = source;

//            this.CreateReader();
//        }

//        private void CreateReader()
//        {
//            string sql = "select distinct n.* " +
//                "from hh_node hn " +
//                "inner join node n " +
//                "on n.id = hn.id " +
//                "where hn.level_idx = :level_idx and hn.core = 1 ";
//            OracleCommand com = new OracleCommand(sql);
//            com.Connection = _connection;
//            com.Parameters.Add("level_idx", _level);
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
//                //TODO: improve on this by getting the data from this connection more quickly!
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
