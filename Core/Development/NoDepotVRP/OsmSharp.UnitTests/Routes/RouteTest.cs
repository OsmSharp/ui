using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.UnitTests.Routes
{
    /// <summary>
    /// General tests for routes.
    /// </summary>
    public abstract class RouteTest
    {
        /// <summary>
        /// Creates the IRoute implementation to perform tests on.
        /// </summary>
        /// <returns></returns>
        protected abstract IRoute BuildRoute();


        public void DoTestIRoute()
        {

        }
    }
}