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
using System.IO;
using OsmSharp.Osm.Data.PBF;
using System.Diagnostics;
using OsmSharp.Osm.Data.Core.Processor.Default;
using OsmSharp.Osm.Data.PBF.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Progress;

namespace OsmSharp.Routing.Osm.Test.PBF
{
    public static class PBFTest
    {

        public static void Execute()
        {
            PBFTest.TestPBFPerformance(@"c:\OSM\bin\belgium.osm.pbf");
        }

        public static void TestPBFPerformance(string pbf)
        {
            PBFDataProcessorSource source = new PBFDataProcessorSource((new FileInfo(pbf)).OpenRead());
            ProgressDataProcessorSource progress_source = new ProgressDataProcessorSource(source);
            DataProcessorTargetEmpty target = new DataProcessorTargetEmpty();
            target.RegisterSource(progress_source);
            target.Pull();
        }
    }
}
