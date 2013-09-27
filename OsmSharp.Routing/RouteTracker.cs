// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Routing.Instructions;
using System.Collections.Generic;
using OsmSharp.Routing.Instructions.LanguageGeneration;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Math.Geo;

namespace OsmSharp.Routing
{
	/// <summary>
	/// Tracks progress along an OsmSharp route.
	/// </summary>
	public class RouteTracker
	{
		/// <summary>
		/// Holds the route being tracked.
		/// </summary>
		private Route _route;

		/// <summary>
		/// Holds the list of instructions.
		/// </summary>
		private List<Instruction> _instructions;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Routing.Route.OsmSharpRouteTracker"/> class.
		/// </summary>
		/// <param name="route">Route.</param>
		/// <param name="interpreter">Interpreter.</param>
		/// <param name="language_generator">Language_generator.</param>
		public RouteTracker (Route route, IRoutingInterpreter interpreter, ILanguageGenerator language_generator)
		{
			_route = route;
			_instructions = InstructionGenerator.Generate (route, interpreter, language_generator);
		}


	}

	/// <summary>
	/// Route tracking point.
	/// </summary>
	public class RouteTrackingPoint
	{
		/// <summary>
		/// Gets a value indicating whether the route has to be recalculated.
		/// </summary>
		/// <value><c>true</c> if this instance has to recalculate; otherwise, <c>false</c>.</value>
		public bool HasToRecalculate {
			get;
			private set;
		}

		/// <summary>
		/// Gets the location.
		/// </summary>
		/// <value>The location.</value>
		public GeoCoordinate Location {
			get;
			private set;
		}

		/// <summary>
		/// Gets the location on route.
		/// </summary>
		/// <value>The location on route.</value>
		public GeoCoordinate LocationOnRoute {
			get;
			private set;
		}
	}
}