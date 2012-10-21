// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.ChangeSets
{
    public abstract class DataProcessorChangeSetSource
    {
        /// <summary>
        /// Initializes this source.
        /// </summary>
        /// <param name="param"></param>
        public abstract void Initialize();

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();

        /// <summary>
        /// Returns the current item in the stream.
        /// </summary>
        /// <returns></returns>
        public abstract SimpleChangeSet Current();

        /// <summary>
        /// Resets the source to the beginning.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Closes this source.
        /// </summary>
        public abstract void Close();

    }
}
