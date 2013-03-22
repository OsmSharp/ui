using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data.Core.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.XML.Processor;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using OsmSharp.Osm;

namespace TiledDataService
{
    /// <summary>
    /// Data operation service implementation.
    /// </summary>
    public class DataOperationService : Service
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AddHeader(ContentType = "text/plain")]
        public string Get(DataOperation request)
        {
            string data_path = ConfigurationManager.AppSettings["datapath"];

            // parse the zoom, x and y.
            int x, y, zoom;
            if (!int.TryParse(request.X, out x))
            { // invalid x.
                throw new InvalidCastException("Cannot parse x-coordinate!");
            }
            if (!int.TryParse(request.Y, out y))
            { // invalid y.
                throw new InvalidCastException("Cannot parse y-coordinate!");
            }
            if (!int.TryParse(request.Zoom, out zoom))
            { // invalid zoom.
                throw new InvalidCastException("Cannot parse zoom!");
            }

            // check of the file exists.
            var pbf_file = new FileInfo(data_path + request.File + ".pbf");
            var xml_file = new FileInfo(data_path + request.File);

            DataProcessorSource source = null;
            if (pbf_file.Exists)
            { // create PBF source.
                source = new PBFDataProcessorSource(pbf_file.OpenRead());
            }
            else if (xml_file.Exists)
            { // create XML source.
                source = new XmlDataProcessorSource(xml_file.OpenRead());
            }
            else 
            { // oeps! file not found!
                throw new FileNotFoundException("File not found!", xml_file.Name);
            }

            // create the filter.
            var tile = new Tile(x,y, zoom);
            DataProcessorFilter filter = new DataProcessorFilterBoundingBox(tile.Box);

            // create the target.
            var result = new StringBuilder();
            var writer = new StringWriter(result);
            var target = new XmlDataProcessorTarget(writer);

            // execute the processing.
            target.RegisterSource(filter);
            filter.RegisterSource(source);
            target.Pull();

            //// close the target.
            //target.Close();

            return result.ToString();
        }
    }
}
