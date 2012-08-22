using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Sources
{
    /// <summary>
    /// Represents any source of relations.
    /// </summary>
    public interface IRelationSource
    {
        /// <summary>
        /// Returns a relation with the given id from this source.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Relation GetRelation(long id);
    }
}
