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
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Osm.Data.PBF;
using System.Diagnostics;
using Osm.Data.Core.Processor.Default;
using Osm.Data.PBF.Raw.Processor;

namespace Osm.Routing.Test.PBF
{
    public static class PBFTest
    {
        public static void TestPBFRead()
        {
            string pbf = @"C:\OSM\bin\albania.osm.pbf";
            FileStream file = File.OpenRead(pbf);

            //PBFReader reader = new PBFReader(file);
            //reader.ReadAll(new Consumer());
            //DataProcessorTargetEmpty target = new DataProcessorTargetEmpty();
            //target.RegisterSource(new PBFDataProcessorSource(file));
            //target.Pull();
        }
    }

    //class Consumer : IPBFPrimitiveBlockConsumer
    //{

    //    public void ProcessPrimitiveBlock(PrimitiveBlock block)
    //    {
    //        foreach(PrimitiveGroup group in block.primitivegroup)
    //        {
    //            DenseNodes dense = group.dense;

    //            foreach (Node node in group.nodes)
    //            {
    //                Trace.Write(node.ToString());
    //            }
    //        }
    //    }
    //}
}
