using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Routing.Core.Roads;

namespace Osm.Routing.Raw.Graphs.Resolver
{
    internal class SimpleGraphResolver : IGraphResolverMatcher
    {
        private IResolveMatcher _resolver; 

        public SimpleGraphResolver(IResolveMatcher resolver)
        {
            _resolver = resolver;
        }

        #region IGraphResolverMatcher Members

        public bool MatchWay(Way way)
        {
            RoadInterpreter interpreter = new RoadInterpreter(way);
            return _resolver.MatchName(interpreter.TagsInterpreter.Name());
        }

        #endregion
    }
}
