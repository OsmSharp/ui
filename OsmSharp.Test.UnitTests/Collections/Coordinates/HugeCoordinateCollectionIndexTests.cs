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

using NUnit.Framework;
using OsmSharp.Collections.Coordinates;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Test.Unittests.Collections.Coordinates
{
    /// <summary>
    /// Testclass with facilities to test the huge dictionary.
    /// </summary>
    [TestFixture]
    public class HugeCoordinateCollectionIndexTests
    {
        /// <summary>
        /// Tests the small huge coordinate collection index.
        /// </summary>
        [Test]
        public void TestHugeCoordinateCollectionIndexSmall()
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(90, 180),
                new GeoCoordinate(-90, -180));
            var size = 100;
            var maxCollectionSize = 4;
            var referenceDictionary = new Dictionary<long, ICoordinateCollection>();
            var coordinates = new HugeCoordinateCollectionIndex(size);
            for(int idx = 0; idx < size; idx++)
            {
                var currentSize = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(maxCollectionSize) + 1;
                var coordinatesArray = new GeoCoordinate[currentSize];
                while(currentSize > 0)
                {
                    coordinatesArray[currentSize - 1] =  box.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
                    currentSize--;
                }
                var coordinatesCollection = new CoordinateArrayCollection<GeoCoordinate>(coordinatesArray);
                referenceDictionary[idx] = coordinatesCollection;
                coordinates[idx] = coordinatesCollection;
            }

            // check result.
            for (int idx = 0; idx < size; idx++)
            {
                var referenceCollection = referenceDictionary[idx];
                var collection = coordinates[idx];

                referenceCollection.Reset();
                collection.Reset();

                while(referenceCollection.MoveNext())
                {
                    Assert.IsTrue(collection.MoveNext());
                    Assert.AreEqual(referenceCollection.Latitude, collection.Latitude);
                    Assert.AreEqual(referenceCollection.Longitude, collection.Longitude);
                }
                Assert.IsFalse(collection.MoveNext());
            }
        }
    }
}