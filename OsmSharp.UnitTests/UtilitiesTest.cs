// OsmSharp - OpenStreetMap tools & library.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Contains tests for the (extension) methods in the utilities class.
    /// </summary>
    [TestFixture]
    public class UtilitiesTest
    {
        /// <summary>
        /// Tests the unixtime conversion.
        /// </summary>
        [Test]
        public void TestUnixTime()
        {
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);

            long unixTime = time.ToUnixTime();
            DateTime timeAfter = unixTime.FromUnixTime();

            Assert.AreEqual(time, timeAfter);

            unixTime = 1374842318000;
            time = unixTime.FromUnixTime();
            long unixTimeAfter = time.ToUnixTime();

            Assert.AreEqual(unixTime, unixTimeAfter);
        }
    }
}
