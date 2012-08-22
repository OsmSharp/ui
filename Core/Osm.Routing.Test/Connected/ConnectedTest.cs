using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tools.Math.Geo;
using System.Data;
using Osm.Data;
using Tools.Xml.Sources;
using Osm.Routing.Raw;
using Osm.Data.Raw.XML.OsmSource;
using Osm.Core.Xml;

namespace Osm.Routing.Test.Connected
{
    class ConnectedTest
    {
        public static void TestMatrix()
        {
            List<bool> connectivity = ConnectedTest.Test("matrix");
        }

        public static void TestMatrixBad()
        {
            List<bool> connectivity = ConnectedTest.Test("matrix_bad");
        }

        public static void TestSteendorp()
        {
            List<bool> connectivity = ConnectedTest.Test("error_steendorp");
        }

        public static List<bool> Test(string name)
        {
            DirectoryInfo info = new FileInfo("dummy.csv").Directory;

            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
			string csv_path = Path.Combine(info.FullName, "Connected",string.Format("{0}.csv",name));
            DataSet data = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFile(null,
                new System.IO.FileInfo(csv_path), Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated, true, true);
            foreach (DataRow row in data.Tables[0].Rows)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }

            // initialize data.
			string osm_path = Path.Combine(info.FullName, "Connected",string.Format("{0}.osm",name));
            IDataSource data_source = new OsmDataSource(
                new OsmDocument(new XmlFileSource(osm_path)));

            // create router.
            Router router = new Router(data_source);

            // resolve all points
            ResolvedPoint[] points = router.Resolve(coordinates.ToArray());

            // check connectivity for all points.
            bool[] connectivity = router.CheckConnectivity(points, 1000);

            router.CalculateManyToManyWeight(points, points);

            return new List<bool>(connectivity);
        }
    }
}
