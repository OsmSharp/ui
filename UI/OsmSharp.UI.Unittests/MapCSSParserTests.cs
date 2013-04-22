using System.IO;
using System.Reflection;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using NUnit.Framework;

namespace OsmSharp.UI.Unittests
{
    /// <summary>
    /// Contains parsing tests using existing mapcss files.
    /// </summary>
    [TestFixture]
    public class MapCSSParserTests
    {
        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestColouredAddresses()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.coloured-addresses.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(2698, tree.ChildCount);
        }

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestDefault()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.default.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(54, tree.ChildCount);
        }

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestGisrussa()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.gisrussa.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(67, tree.ChildCount);
        }

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestHideNodes()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.Hide_nodes.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(4, tree.ChildCount);
        }

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestLanduses()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.landuses.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(24, tree.ChildCount);
        }

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestOamStyle()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.oam-style.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(14, tree.ChildCount);
        }

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestOpenstreetinfo()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.openstreetinfo.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(16, tree.ChildCount);
        }

        ///// <summary>
        ///// Tests simply parsing one of the testcases.
        ///// </summary>
        //[Test]
        //public void Test_osmosnimki_hybrid()
        //{
        //    // TODO: this css will not parse; out-of-memory exception!
        //    // TODO: found why not: there is an unrecognized string 'Москва' (probably some encoding or Antlr thing!).

        //    // parses the MapCSS.
        //    AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
        //        "OsmSharp.UI.Unittests.Data.MapCSS.osmosnimki-hybrid.mapcss");

        //    // Test the very minimum; no errors during parsing says a lot already!
        //    Antlr.Runtime.Tree.CommonTree tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
        //    Assert.NotNull(tree);
        //    Assert.AreEqual(16, tree.ChildCount);
        //}

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestPotlatch2()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.potlatch2.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(252, tree.ChildCount);
        }

        ///// <summary>
        ///// Tests simply parsing one of the testcases.
        ///// </summary>
        //[Test]
        //public void Test_opencyclemap()
        //{
        //    //TODO: this css will not parse; the meta section is a problem!

        //    // parses the MapCSS.
        //    AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
        //        "OsmSharp.UI.Unittests.Data.MapCSS.opencyclemap.mapcss");

        //    // Test the very minimum; no errors during parsing says a lot already!
        //    Antlr.Runtime.Tree.CommonTree tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
        //    Assert.NotNull(tree);
        //    Assert.AreEqual(36, tree.ChildCount);
        //}

        ///// <summary>
        ///// Tests simply parsing one of the testcases.
        ///// </summary>
        //[Test]
        //public void Test_sport_styles()
        //{
        //    //TODO: this css will not parse; the meta section is a problem!

        //    // parses the MapCSS.
        //    AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
        //        "OsmSharp.UI.Unittests.Data.MapCSS.sport_styles.mapcss");

        //    // Test the very minimum; no errors during parsing says a lot already!
        //    Antlr.Runtime.Tree.CommonTree tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
        //    Assert.NotNull(tree);
        //    Assert.AreEqual(252, tree.ChildCount);
        //}

        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestTrivial()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.trivial.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(1, tree.ChildCount);
        }

        /// <summary>
        /// Test-parses the MapCSS.
        /// </summary>
        /// <param name="embeddedPath"></param>
        private AstParserRuleReturnScope<object, IToken> TestMapCSSParsing(string embeddedPath)
        {
            // get the text from the embedded test file.
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedPath);
            Assert.IsNotNull(stream);
            var reader = new StreamReader(stream);
            string s = reader.ReadToEnd();

            var input = new ANTLRStringStream(s);
            var lexer = new MapCSSLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new MapCSSParser(tokens);

            return parser.stylesheet();
        }
    }
}
