using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Resolving;
using Osm.Routing.Core;
using Osm.Routing.Core.Interpreter;
using Osm.Routing.Core.Constraints;
using System.IO;
using System.Reflection;
using Tools.Math.Geo;

namespace Osm.Routing.Test.ManyToMany
{
    /// <summary>
    /// Does some basic many-to-many performance tests.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public abstract class ManyToManyTests<ResolvedType> 
        where ResolvedType : IResolvedPoint
    {
        /// <summary>
        /// Executes many-to-many test on embedded files with the given name.
        /// </summary>
        /// <param name="name"></param>
        public float[][] TestFor(string name)
        {
            string xml_embedded = string.Format("Osm.Routing.Test.TestData.{0}.osm", name);
            string csv_embedded = string.Format("Osm.Routing.Test.TestData.{0}.csv", name);

            // build the router.
            IRouter<ResolvedType> router = this.CreateRouter(Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded),
                new Osm.Routing.Core.Interpreter.Default.DefaultVehicleInterpreter(VehicleEnum.Car),
                new Osm.Routing.Core.Constraints.Cars.DefaultCarConstraints());

            // read points.
            IList<GeoCoordinate> coordinates = this.ReadPoints(Assembly.GetExecutingAssembly().GetManifestResourceStream(csv_embedded));

            // resolve the points.
            ResolvedType[] resolved = router.Resolve(coordinates.ToArray());

            // calculate the many-to-many.
            return router.CalculateManyToManyWeight(resolved, resolved);
        }

        /// <summary>
        /// Creates a router.
        /// </summary>
        /// <param name="data">The data to use for routing.</param>
        /// <param name="interpreter">The interpreter.</param>
        /// <param name="constraints">The routing constraints.</param>
        /// <returns></returns>
        protected abstract IRouter<ResolvedType> CreateRouter(Stream data,
            RoutingInterpreterBase interpreter, IRoutingConstraints constraints);

        /// <summary>
        /// Reads all test points from the data stream.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IList<GeoCoordinate> ReadPoints(Stream data)
        {            
            // read matrix points.
            List<GeoCoordinate> coordinates = new List<GeoCoordinate>();
            string[][] lines = Tools.Core.DelimitedFiles.DelimitedFileHandler.ReadDelimitedFileFromStream(
                data, Tools.Core.DelimitedFiles.DelimiterType.DotCommaSeperated);
            foreach (string[] row in lines)
            {
                // be carefull with the parsing and the number formatting for different cultures.
                double latitude = double.Parse(row[2].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double longitude = double.Parse(row[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                GeoCoordinate point = new GeoCoordinate(latitude, longitude);
                coordinates.Add(point);
            }
            return coordinates;
        }
    }
}
