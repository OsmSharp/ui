using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core.Simple;

namespace Osm.Data.Core.Processor.Default
{
    public class DataProcessorTargetEmpty : DataProcessorTarget
    {
        public override void Initialize()
        {

        }

        public override void ApplyChange(SimpleChangeSet change)
        {

        }

        public override void AddNode(SimpleNode node)
        {

        }

        public override void AddWay(SimpleWay way)
        {

        }

        public override void AddRelation(SimpleRelation relation)
        {

        }
    }
}
