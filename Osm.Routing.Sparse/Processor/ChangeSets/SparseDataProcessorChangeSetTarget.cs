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
