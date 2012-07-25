using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Core.Simple
{
    public class SimpleRelationMember
    {
        public SimpleRelationMemberType? MemberType { get; set; }

        public long? MemberId { get; set; }

        public string MemberRole { get; set; }
    }
}
