using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tools.Xml.Sources;
using Tools.Xml.Gpx;
using Tools.Xml.Gpx.v1_1;

namespace Osm.Routing.Core.Route.Gpx
{
    /// <summary>
    /// Converts an OsmSharpRoute into a gpx.
    /// </summary>
    internal class OsmSharpRouteGpx
    {
        /// <summary>
        /// Saves the route to a gpx file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="route"></param>
        internal static void Save(FileInfo file, OsmSharpRoute route)
        {
            XmlFileSource source = new XmlFileSource(file);
            GpxDocument output_document = new GpxDocument(source);
            gpxType output_gpx = new gpxType();
            output_gpx.trk = new trkType[1];

            // initialize all objects.
            List<wptType> segments = new List<wptType>();
            trkType track = new trkType();
            List<wptType> poi_gpx = new List<wptType>();

            track.trkseg = new trksegType[1];

            // ============= CONSTRUCT TRACK SEGMENT ==============
            trksegType track_segment = new trksegType();

            // loop over all points.
            for (int idx = 0; idx < route.Entries.Length; idx++)
            {
                // get the current entry.
                RoutePointEntry entry = route.Entries[idx];

                // ================== INITIALIZE A NEW SEGMENT IF NEEDED! ========
                wptType waypoint;
                if (entry.Points != null)
                { // loop over all points and create a waypoint for each.
                    for (int p_idx = 0; p_idx < entry.Points.Length; p_idx++)
                    {
                        RoutePoint point = entry.Points[p_idx];

                        waypoint = new wptType();
                        waypoint.lat = (decimal)point.Latitude;
                        waypoint.lon = (decimal)point.Longitude;
                        waypoint.name = point.Name;
                        poi_gpx.Add(waypoint);
                    }
                }

                // insert poi's.
                double longitde = entry.Longitude;
                double latitude = entry.Latitude;

                waypoint = new wptType();
                waypoint.lat = (decimal)entry.Latitude;
                waypoint.lon = (decimal)entry.Longitude;

                segments.Add(waypoint);
            }

            // put the segment in the track.
            track_segment.trkpt = segments.ToArray();
            track.trkseg[0] = track_segment;

            // set the track to the output.
            output_gpx.trk[0] = track;            
            output_gpx.wpt = poi_gpx.ToArray();

            // save the ouput.
            output_document.Gpx = output_gpx;
            output_document.Save();
        }
    }
}
