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
        private static Dictionary<string, Vehicle> VehiclesByName = null;

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
            VehiclesByName[this.UniqueName.ToLowerInvariant()] = this;
        }

        /// <summary>
        /// Returns the vehicle with the given name.
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        public static Vehicle GetByUniqueName(string uniqueName)
        {
            Vehicle vehicle = null;
            if(VehiclesByName == null)
            { // no vehicles have been registered.
                Vehicle.RegisterVehicles();
            }
            uniqueName = uniqueName.ToLowerInvariant();
            if(!VehiclesByName.TryGetValue(uniqueName, out vehicle))
            { // vehicle name not registered.
                throw new ArgumentOutOfRangeException(string.Format("Vehicle profile with name {0} not found or not registered.", uniqueName));
            }
            return vehicle;
        }

        /// <summary>
        /// Returns true if the given tag is relevant for any registered vehicle, false otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsRelevantForOneOrMore(string key)
        {
            // register at least the default vehicles.
            if (VehiclesByName == null)
            { // no vehicles have been registered.
                Vehicle.RegisterVehicles();
            }

            foreach(var vehicle in VehiclesByName)
            {
                if(vehicle.Value.IsRelevant(key))
                { // ok, key is relevant.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given key-value pair is relevant for any registered vehicle, false otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsRelevantForOneOrMore(string key, string value)
        {
            // register at least the default vehicles.
            if (VehiclesByName == null)
            { // no vehicles have been registered.
                Vehicle.RegisterVehicles();
            }

            foreach (var vehicle in VehiclesByName)
            {
                if (vehicle.Value.IsRelevant(key, value))
                { // ok, key is relevant.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Holds names of generic vehicle types like 'pedestrian', 'horse', 'carriage',...
        /// </summary>
        /// <remarks>This is used to interpret restrictions.
        /// See issue: https://github.com/OsmSharp/OsmSharp/issues/192
        /// And OpenStreetMap-wiki: http://wiki.openstreetmap.org/wiki/Key:access#Transport_mode_restrictions</remarks>
        public readonly HashSet<string> VehicleTypes = new HashSet<string>();

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
        public abstract KilometerPerHour MaxSpeedAllowed(string highwayType);

        /// <summary>
        /// Returns the max speed this vehicle can handle.
        /// </summary>
        /// <returns></returns>
        public abstract KilometerPerHour MaxSpeed();

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
            if(tags.TryGetMaxSpeed(out speed))
            {
                return speed;
            }

            string highwayType;
            if (TryGetHighwayType(tags, out highwayType))
            {
                speed = this.MaxSpeedAllowed(highwayType);
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
            KilometerPerHour maxSpeedAllowed = this.MaxSpeedAllowed(tags);
            KilometerPerHour maxSpeed = this.MaxSpeed();
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
            if (this.GetName(tags1) != this.GetName(tags2))
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
            return this.Weight(tags, distance);
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
            if(intermediate != null)
            {
                for(int idx = 0; idx < intermediate.Length; idx++)
                {
                    var current = intermediate[idx];
                    distance = distance + this.Weight(tags, previous, current);
                    previous = current;
                }
            }
            return (float)(distance + this.Weight(tags, previous, to));
        }

        /// <summary>
        /// Returns the weight between points based on the tags and distance.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public virtual float Weight(TagsCollectionBase tags, float distance)
        {
            return (float)(distance / (this.ProbableSpeed(tags).Value) * 3.6);
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

        /// <summary>
        /// Returns true if the key is relevant for this vehicle profile.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool IsRelevant(string key)
        {
            if(AccessibleTags != null &&
                AccessibleTags.ContainsKey(key))
            { // definetly relevant!
                return true;
            }
            if(VehicleTypes.Contains(key))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the key-value pair is relevant for this vehicle profile.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool IsRelevant(string key, string value)
        { // keep all values, override in specific profiles to keep only relevant values.
            return this.IsRelevant(key); 
        }
    }

    /// <summary>
    /// Represents a pedestrian
    /// </summary>
    public class Pedestrian : Vehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Pedestrian()
        {
            AccessibleTags.Add("footway", string.Empty);
            AccessibleTags.Add("cycleway", string.Empty);
            AccessibleTags.Add("path", string.Empty);
            AccessibleTags.Add("road", string.Empty);
            AccessibleTags.Add("track", string.Empty);
            AccessibleTags.Add("pedestrian", string.Empty);
            AccessibleTags.Add("living_street", string.Empty);
            AccessibleTags.Add("residential", string.Empty);
            AccessibleTags.Add("unclassified", string.Empty);
            AccessibleTags.Add("secondary", string.Empty);
            AccessibleTags.Add("secondary_link", string.Empty);
            AccessibleTags.Add("primary", string.Empty);
            AccessibleTags.Add("primary_link", string.Empty);
            AccessibleTags.Add("tertiary", string.Empty);
            AccessibleTags.Add("tertiary_link", string.Empty);

            VehicleTypes.Add("pedestrian");
        }

        /// <summary>
        /// Returns true if the vehicle is allowed on the way represented by these tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
        {
            if (tags.ContainsKey("foot"))
            {
                if (tags["foot"] == "designated")
                {
                    return true; // designated foot
                }
                if (tags["foot"] == "yes")
                {
                    return true; // yes for foot
                }
                if (tags["foot"] == "no")
                {
                    return false; // no for foot
                }
            }
            return AccessibleTags.ContainsKey(highwayType);
        }

        /// <summary>
        /// Returns the Max Speed for the highwaytype in Km/h.
        /// 
        /// This does not take into account how fast this vehicle can go just the max possible speed.
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeedAllowed(string highwayType)
        {
            switch (highwayType)
            {
                case "services":
                case "proposed":
                case "cycleway":
                case "pedestrian":
                case "steps":
                case "path":
                case "footway":
                case "living_street":
                    return 5;
                case "track":
                case "road":
                    return 30;
                case "residential":
                case "unclassified":
                    return 50;
                case "motorway":
                case "motorway_link":
                    return 120;
                case "trunk":
                case "trunk_link":
                case "primary":
                case "primary_link":
                    return 90;
                default:
                    return 70;
            }
        }

        /// <summary>
        ///     Returns true if the edge is one way forward, false if backward, null if bidirectional.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override bool? IsOneWay(TagsCollectionBase tags)
        {
            return null;
        }

        /// <summary>
        /// Returns the maximum possible speed this vehicle can achieve.
        /// </summary>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeed()
        {
            return 5;
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Pedestrian"; }
        }
    }

    /// <summary>
    /// Represents a bicycle
    /// </summary>
    public class Bicycle : Vehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Bicycle()
        {
            AccessibleTags.Add("cycleway", string.Empty);
            AccessibleTags.Add("path", string.Empty);
            AccessibleTags.Add("road", string.Empty);
            AccessibleTags.Add("track", string.Empty);
            AccessibleTags.Add("living_street", string.Empty);
            AccessibleTags.Add("residential", string.Empty);
            AccessibleTags.Add("unclassified", string.Empty);
            AccessibleTags.Add("secondary", string.Empty);
            AccessibleTags.Add("secondary_link", string.Empty);
            AccessibleTags.Add("primary", string.Empty);
            AccessibleTags.Add("primary_link", string.Empty);
            AccessibleTags.Add("tertiary", string.Empty);
            AccessibleTags.Add("tertiary_link", string.Empty);

            VehicleTypes.Add("bicycle");
        }

        /// <summary>
        /// Returns true if the vehicle is allowed on the way represented by these tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
        {
            // do the designated tags.
            if (tags.ContainsKey("bicycle"))
            {
                if (tags["bicycle"] == "designated")
                {
                    return true; // designated bicycle
                }
                if (tags["bicycle"] == "yes")
                {
                    return true; // yes for bicycle
                }
                if (tags["bicycle"] == "no")
                {
                    return false; //  no for bicycle
                }
            }
            return AccessibleTags.ContainsKey(highwayType);
        }

        /// <summary>
        /// Returns the Max Speed for the highwaytype in Km/h.
        /// 
        /// This does not take into account how fast this vehicle can go just the max possible speed.
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeedAllowed(string highwayType)
        {
            switch (highwayType)
            {
                case "services":
                case "proposed":
                case "cycleway":
                case "pedestrian":
                case "steps":
                case "path":
                case "footway":
                case "living_street":
                    return this.MaxSpeed();
                case "track":
                case "road":
                    return 30;
                case "residential":
                case "unclassified":
                    return 50;
                case "motorway":
                case "motorway_link":
                    return 120;
                case "trunk":
                case "trunk_link":
                case "primary":
                case "primary_link":
                    return 90;
                default:
                    return 70;
            }
        }

        /// <summary>
        /// Returns true if the edge is one way forward, false if backward, null if bidirectional.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public override bool? IsOneWay(TagsCollectionBase tags)
        {
            return null;
        }

        /// <summary>
        /// Returns the maximum possible speed this vehicle can achieve.
        /// </summary>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeed()
        {
            return 15;
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Bicycle"; }
        }
    }

    /// <summary>
    /// Represents a MotorVehicle
    /// </summary>
    public abstract class MotorVehicle : Vehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        protected MotorVehicle()
        {
            AccessibleTags.Add("road", string.Empty);
            AccessibleTags.Add("living_street", string.Empty);
            AccessibleTags.Add("residential", string.Empty);
            AccessibleTags.Add("unclassified", string.Empty);
            AccessibleTags.Add("secondary", string.Empty);
            AccessibleTags.Add("secondary_link", string.Empty);
            AccessibleTags.Add("primary", string.Empty);
            AccessibleTags.Add("primary_link", string.Empty);
            AccessibleTags.Add("tertiary", string.Empty);
            AccessibleTags.Add("tertiary_link", string.Empty);
            AccessibleTags.Add("trunk", string.Empty);
            AccessibleTags.Add("trunk_link", string.Empty);
            AccessibleTags.Add("motorway", string.Empty);
            AccessibleTags.Add("motorway_link", string.Empty);

            VehicleTypes.Add("vehicle"); // a motor vehicle is a generic vehicle.
            VehicleTypes.Add("motor_vehicle"); // ... and also a generic motor vehicle.
        }

        /// <summary>
        /// Returns true if the vehicle is allowed on the way represented by these tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        protected override bool IsVehicleAllowed(TagsCollectionBase tags, string highwayType)
        {
            if (tags.ContainsKey("motor_vehicle"))
            {
                if (tags["motor_vehicle"] == "no")
                {
                    return false;
                }
            }
            return AccessibleTags.ContainsKey(highwayType);
        }

        /// <summary>
        /// Returns the Max Speed for the highwaytype in Km/h.
        /// 
        /// This does not take into account how fast this vehicle can go just the max possible speed.
        /// </summary>
        /// <param name="highwayType"></param>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeedAllowed(string highwayType)
        {
            switch (highwayType)
            {
                case "services":
                case "proposed":
                case "cycleway":
                case "pedestrian":
                case "steps":
                case "path":
                case "footway":
                case "living_street":
                    return 5;
                case "track":
                case "road":
                    return 30;
                case "residential":
                case "unclassified":
                    return 50;
                case "motorway":
                case "motorway_link":
                    return 120;
                case "trunk":
                case "trunk_link":
                case "primary":
                case "primary_link":
                    return 90;
                default:
                    return 70;
            }
        }

        /// <summary>
        /// Returns the maximum possible speed this vehicle can achieve.
        /// </summary>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeed()
        {
            return 200;
        }
    }

    /// <summary>
    /// Represents a moped
    /// </summary>
    public class Moped : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Moped()
        {
            AccessibleTags.Remove("motorway");
            AccessibleTags.Remove("motorway_link");

            VehicleTypes.Add("moped");
        }

        /// <summary>
        /// Returns the maximum possible speed this vehicle can achieve.
        /// </summary>
        /// <returns></returns>
        public override KilometerPerHour MaxSpeed()
        {
            return 40;
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Moped"; }
        }
    }

    /// <summary>
    /// Represents a MotorCycle
    /// </summary>
    public class MotorCycle : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MotorCycle()
        {
            VehicleTypes.Add("motorcycle");
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "MotorCycle"; }
        }
    }

    /// <summary>
    /// Represents a Car
    /// </summary>
    public class Car : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Car()
        {
            VehicleTypes.Add("motorcar");
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Car"; }
        }
    }

    /// <summary>
    /// Represents a SmallTruck
    /// </summary>
    public class SmallTruck : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SmallTruck()
        {
            // http://wiki.openstreetmap.org/wiki/Key:goods
            VehicleTypes.Add("goods");
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "SmallTruck"; }
        }
    }

    /// <summary>
    /// Represents a BigTruck
    /// </summary>
    public class BigTruck : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public BigTruck()
        {
            // http://wiki.openstreetmap.org/wiki/Key:hgv#Land-based_transportation
            VehicleTypes.Add("hgv");
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "BigTruck"; }
        }
    }

    /// <summary>
    /// Represents a bus that is not acting as a public service.
    /// </summary>
    /// <remarks>http://wiki.openstreetmap.org/wiki/Key:tourist_bus</remarks>
    public class Bus : MotorVehicle
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Bus()
        {
            // http://wiki.openstreetmap.org/wiki/Key:tourist_bus
            VehicleTypes.Add("tourist_bus");
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Bus"; }
        }
    }
}