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
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;

namespace OsmSharp.Test.Unittests.Collections.Tags
{
    /// <summary>
    /// Contains tests for the SimpleTagsCollectionIndexTests.
    /// </summary>
    [TestFixture]
    public class TagsTableCollectionIndexTests : TagsCollectionIndexBaseTests
    {
        /// <summary>
        /// Tests the simple tags collection index.
        /// </summary>
        [Test]
        public void SimpleTestTagsCollectionIndex()
        {
            this.TestTagsCollectionIndex(new TagsTableCollectionIndex());
        }
    }
}