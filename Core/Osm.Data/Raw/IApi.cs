using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;

namespace Osm.Data
{
    /// <summary>
    /// Represents a full osm api interface.
    /// 
    /// Used For: OSM Api v0.6.
    /// </summary>
    public interface IApi : IDataSource
    {
        #region User Authentication

        /// <summary>
        /// Authenticates a users; allowing for modifications.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        bool Authenticate(User user, string pass);

        /// <summary>
        /// Returns the authenticated user.
        /// </summary>
        User AuthenticatedUser
        {
            get;
        }

        #endregion
    }
}
