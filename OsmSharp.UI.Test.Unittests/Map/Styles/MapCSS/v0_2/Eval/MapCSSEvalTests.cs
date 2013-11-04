using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Eval;

namespace OsmSharp.UI.Test.Unittests.Map.Styles.MapCSS.v0_2.Eval
{
    /// <summary>
    /// Contains generic MapCSS eval function tests.
    /// </summary>
    [TestFixture]
    public class MapCSSEvalTests
    {
        /// <summary>
        /// Tests a simple tag evaluation.
        /// </summary>
        [Test]
        public void MapCSSEvalTagTest()
        {
            string function = "tag('width')";
            TagsCollectionBase tags = new TagsCollection();
            tags.Add("width", "2");

            Assert.AreEqual(2, EvalInterpreter.Instance.InterpretDouble(function, tags));
        }
    }
}
