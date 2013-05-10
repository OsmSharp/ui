using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map.Styles;
using OsmSharp.UI.Renderer;
using OsmSharp.Xml.Gpx;
using OsmSharp.Xml.Sources;
using OsmSharp.Osm.Data.Raw.XML.GpxSource;
using OsmSharp.Osm.Filters;
using OsmSharp.Osm;
using System.IO;
using OsmSharp.Math.Geo.Projections;

namespace OsmSharp.UI.Map.Layers
{
	/// <summary>
	/// A layer drawing GPX data.
	/// </summary>
	public class GpxLayer : ILayer
	{		
		/// <summary>
		/// Holds the projection.
		/// </summary>
		private IProjection _projection;

		/// <summary>
		/// Creates a new OSM data layer.
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="styleInterpreter"></param>
		public GpxLayer(IProjection projection)
		{			
			_projection = projection;

			this.Scene = new Scene2D();
		}
		
		/// <summary>
		/// Gets the minimum zoom.
		/// </summary>
		public float? MinZoom { get; private set; }
		
		/// <summary>
		/// Gets the maximum zoom.
		/// </summary>
		public float? MaxZoom { get; private set; }
		
		/// <summary>
		/// Gets the scene of this layer containing the objects already projected.
		/// </summary>
		public Scene2D Scene { get; private set; }
		
		/// <summary>
		/// Called when the view on the map containing this layer has changed.
		/// </summary>
		/// <param name="map"></param>
		/// <param name="zoomFactor"></param>
		/// <param name="center"></param>
		/// <param name="view"></param>
		public void ViewChanged(Map map, float zoomFactor, GeoCoordinate center, View2D view)
		{
			// all data is pre-loaded for now.

			// when displaying huge amounts of GPX-data use another approach.
		}
		
		/// <summary>
		/// Event raised when this layer's content has changed.
		/// </summary>
		public event Map.LayerChanged LayerChanged;
		
		#region Scene Building

		/// <summary>
		/// Adds a new GPX.
		/// </summary>
		/// <param name="stream">Stream.</param>
		public void AddGpx(Stream stream)
		{
			GpxDocument gpxDocument = new GpxDocument(
				new XmlStreamSource(stream));
			GpxDataSource gpxDataSource = new GpxDataSource(
				gpxDocument);

			// query all objects.
			IList<OsmBase> objects = gpxDataSource.Get(
				Filter.Any());

			foreach(var osmBase in objects)
			{
				if(osmBase is Node)
				{

				}
				else if(osmBase is Way)
				{ // the actual route.
					Way way = (osmBase as Way);

					// get x/y.
					float[] x = new float[way.Nodes.Count];
					float[] y = new float[way.Nodes.Count];
					for (int idx = 0; idx < way.Nodes.Count; idx++)
					{
						x[idx] = (float)_projection.LongitudeToX(
							way.Nodes[idx].Coordinate.Longitude);
						y[idx] = (float)_projection.LatitudeToY(
							way.Nodes[idx].Coordinate.Latitude);
					}

					this.Scene.AddLine(float.MinValue, float.MaxValue, x, y, 
					                   SimpleColor.FromKnownColor(KnownColor.Blue).Value, 5);
				}
				else if(osmBase is Relation)
				{
					// hmm relations in a GPX are impossible.
				}
			}
		}
		
		#endregion
	}
}