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

using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Units.Speed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
    /// <summary>
    ///     Vehicle class contains routing info
    /// </summary>
    public abstract class Vehicle
    {
        /// <summary>
        /// Default Car
        /// </summary>
        public static readonly Vehicle Car = new Car();

        /// <summary>
        /// Default Pedestrian
        /// </summary>
        public static readonly Vehicle Pedestrian = new Pedestrian();

        /// <summary>
        /// Default Bicycle
        /// </summary>
        public static readonly Vehicle Bicycle = new Bicycle();

        /// <summary>
        /// Default Moped
        /// </summary>
        public static readonly Vehicle Moped = new Moped();

        /// <summary>
        /// Default MotorCycle
        /// </summary>
        public static readonly Vehicle MotorCycle = new MotorCycle();

        /// <summary>
        /// Default SmallTruck
        /// </summary>
        public static readonly Vehicle SmallTruck = new SmallTruck();

        /// <summary>
        /// Default BigTruck
        /// </summary>
        public static readonly Vehicle BigTruck = new BigTruck();

        /// <summary>
        /// Default BigTruck
        /// </summary>
        public static readonly Vehicle Bus = new Bus();

        /// <summary>
        /// Registers all default vehicles.
        /// </summary>
        public static void RegisterVehicles()
        {
            Car.Register();
            Pedestrian.Register();
            Bicycle.Register();
            Moped.Register();
            MotorCycle.Register();
            SmallTruck.Register();
            BigTruck.Register();
            Bus.Register();
        }

        /// <summary>
        /// Holds the vehicles by name.
        /// </summary>
        private static Dictionary<string, Vehicle> VehiclesByName;

        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        public Vehicle()
        {

        }

        /// <summary>
        /// Registers this vehicle by name.
        /// </summary>
        public void Register()
        {
            if (VehiclesByName == null)
            { // initialize the vehicle by name dictionary.
                VehiclesByName = new Dictionary<string, Vehicle>();
            }
            VehiclesByName[UniqueName.ToLowerInvariant()] = this;
        }

        /// <summary>
        /// Returns the vehicle with the given name.
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public static Vehicle GetByUniqueName(string uniqueName)
        {
            Vehicle vehicle = null;
            if (VehiclesByName == null)
            { // no vehicles have been registered.
                Vehicle.RegisterVehicles();
            }
            uniqueName = uniqueName.ToLowerInvariant();
            if (!VehiclesByName.TryGetValue(uniqueName, out vehicle))
            { // vehicle name not registered.
                throw new ArgumentOutOfRangeException(string.Format("Vehicle profile with name {0} not found or not registered.", uniqueName));
            }
            return vehicle;
        }

        /// <summary>
        /// Contains Accessiblity Rules
        /// </summary>
        protected readonly Dictionary<string, string> AccessibleTags = new Dictionary<string, string>();
        /// <summary>
        /// Trys to return the highwaytype from the tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected bool TryGetHighwayType(TagsCollectionBase tags, out string highwayType)
        {
            highwayType = string.Empty;
            return tags != null && tags.TryGetValue("highway", out highwayType);
        }

        /// <summary>
        ///     Returns true if the edge with the given tags can be traversed by the vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual bool CanTraverse(TagsCollectionBase tags)
        {
            string highwayType;
            if (TryGetHighwayType(tags, out highwayType))
            {
                return IsVehicleAllowed(tags, highwayType);
            }
            return false;
        }

        /// <summary>
        /// Returns the Max Speed for the highwaytype in Km/h
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected abstract KilometerPerHour MaxSpeedAllowed(string highwayType);

        /// <summary>
        /// Returns the max speed this vehicle can handle.
        /// </summary>
        /// <returns></returns>
        protected abstract KilometerPerHour MaxSpeed();

        /// <summary>
        /// Returns the maximum speed.
        /// </summary>
        /// <returns></returns>
        public virtual KilometerPerHour MaxSpeedAllowed(TagsCollectionBase tags)
        {
            // THESE ARE THE MAX SPEEDS FOR BELGIUM. 
            // TODO: Find a way to make this all configurable.
            KilometerPerHour speed = 5;

            // get max-speed tag if any.
            if (tags.TryGetMaxSpeed(out speed))
            {
                return speed;
            }

            string highwayType;
            if (TryGetHighwayType(tags, out highwayType))
            {
                speed = MaxSpeedAllowed(highwayType);
            }

            return speed;
        }

        /// <summary>
        /// Estimates the probable speed of this vehicle on a way with the given tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual KilometerPerHour ProbableSpeed(TagsCollectionBase tags)
        {
            KilometerPerHour maxSpeedAllowed = MaxSpeedAllowed(tags);
            KilometerPerHour maxSpeed = MaxSpeed();
            if (maxSpeed.Value < maxSpeedAllowed.Value)
            {
                return maxSpeed;
            }
            return maxSpeedAllowed;
        }

        /// <summary>
        /// Returns true if the edges with the given properties are equal for the vehicle.
        /// </summary>
        /// <param name="tags1"></param>
        /// <param name="tags2"></param>
        /// <returns></returns>
        public virtual bool IsEqualFor(TagsCollectionBase tags1, TagsCollectionBase tags2)
        {
            if (GetName(tags1) != GetName(tags2))
            {
                // the name have to be equal.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the weight between two points on an edge with the given tags for the vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual float Weight(TagsCollectionBase tags, GeoCoordinate from, GeoCoordinate to)
        {
            var distance = (float)from.DistanceEstimate(to).Value;
            return Weight(tags, distance);
        }

        /// <summary>
        /// Returns the weight between points on an edge with the given tags for the vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="from"></param>
        /// <param name="intermediate"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual float Weight(TagsCollectionBase tags, GeoCoordinate from, GeoCoordinate[] intermediate, GeoCoordinate to)
        {
            double distance = 0;
            var previous = from;
            if (intermediate != null)
            {
                for (int idx = 0; idx < intermediate.Length; idx++)
                {
                    var current = intermediate[idx];
                    distance = distance + Weight(tags, previous, current);
                    previous = current;
                }
            }
            return (float)(distance + Weight(tags, previous, to));
        }

        /// <summary>
        /// Returns the weight between points based on the tags and distance.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public virtual float Weight(TagsCollectionBase tags, float distance)
        {
            return (float)(distance / (ProbableSpeed(tags).Value) * 3.6);
        }

        /// <summary>
        ///     Returns true if the edge is one way forward, false if backward, null if bidirectional.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public virtual bool? IsOneWay(TagsCollectionBase tags)
        {
            string oneway;
            if (tags.TryGetValue("oneway", out oneway))
            {
                if (oneway == "yes")
                {
                    return true;
                }
                return false;
            }
            string junction;
            if (tags.TryGetValue("junction", out junction))
            {
                if (junction == "roundabout")
                {
                    return true;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the name of a given way.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        private string GetName(TagsCollectionBase tags)
        {
            var name = string.Empty;
            if (tags.ContainsKey("name"))
            {
                name = tags["name"];
            }
            return name;
        }

        /// <summary>
        /// Returns true if the vehicle is allowed on the way represented by these tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected abstract bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType);

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public abstract string UniqueName
        {
            get;
        }
    }
}