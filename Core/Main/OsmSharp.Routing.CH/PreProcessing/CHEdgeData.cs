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
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Graph.DynamicGraph;

namespace OsmSharp.Routing.CH.PreProcessing
{
    public class CHEdgeData : IDynamicGraphEdgeData
    {
        public bool Forward { get; set; }

        public bool Backward { get; set; }

        public float Weight { get; set; }

        public bool HasContractedVertex
        {
            get
            {
                return this.ContractedVertexId > 0;
            }
        }

        public uint ContractedVertexId { get; set; }

        public bool HasTags { get; set; }

        public uint Tags { get; set; }

        public bool IsVirtual
        {
            get
            {
                return this.HasContractedVertex;
            }
        }


        double IDynamicGraphEdgeData.Weight
        {
            get { return this.Weight; }
        }
    }
}
