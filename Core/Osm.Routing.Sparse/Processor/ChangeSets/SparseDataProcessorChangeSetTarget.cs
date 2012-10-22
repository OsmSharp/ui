// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using Osm.Data.Core.Processor.ChangeSets;
using Osm.Core.Simple;
using Osm.Data.Core.Sparse;
using Osm.Routing.Sparse.PreProcessor;

namespace Osm.Routing.Sparse.Processor.ChangeSets
{
    public class SparseDataProcessorChangeSetTarget : DataProcessorChangeSetTarget
    {
        private SparsePreProcessor _data;

        public SparseDataProcessorChangeSetTarget(SparsePreProcessor data)
        {
            _data = data;
        }

        public override void Initialize()
        {

        }

        public override void ApplyChange(SimpleChangeSet changes)
        {
            foreach (SimpleChange change in changes.Changes)
            {
                if (change.OsmGeo != null)
                {
                    foreach (SimpleOsmGeo geo in change.OsmGeo)
                    {
                        if (geo is SimpleNode)
                        {

                        }
                        else if (geo is SimpleWay)
                        {

                        }
                        else if (geo is SimpleRelation)
                        {

                        }
                    }
                }
            }
        }

    }
}
