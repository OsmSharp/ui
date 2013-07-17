using System.IO;
using System.Reflection;
using Antlr.Runtime;
using NUnit.Framework;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2;
using OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain;

namespace OsmSharp.UI.Unittests
{
    /// <summary>
    /// Contains parsing tests using existing mapcss files.
    /// </summary>
    [TestFixture]
    public class MapCSSDomainParserTests
    {
        /// <summary>
        /// Tests simply parsing one of the testcases.
        /// </summary>
        [Test]
        public void TestDomainDefault()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.default.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(54, tree.ChildCount);

            // parse into domain.
            MapCSSFile file = MapCSSDomainParser.Parse(tree);
            Assert.IsNotNull(file);
        }

        /// <summary>
        /// Regression test parsing a named color.
        /// </summary>
        [Test]
        public void TestDomainColorNamed()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.color-named.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(2, tree.ChildCount);

            // parse into domain.
            MapCSSFile file = MapCSSDomainParser.Parse(tree);
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Rules.Count);
            Assert.AreEqual(1, file.Rules[0].Declarations.Count);
            Assert.IsInstanceOf(typeof(DeclarationInt), file.Rules[0].Declarations[0]);

            // get color declaration.
            var declarationInt = file.Rules[0].Declarations[0] as DeclarationInt;
            Assert.IsNotNull(declarationInt);
            Assert.AreEqual(DeclarationIntEnum.Color, declarationInt.Qualifier);

            // instantiate color.
            var simpleColor = new SimpleColor();
            simpleColor.Value = declarationInt.Eval(null);
            Assert.AreEqual("#FFFFFF", simpleColor.HexRgb);
        }

        /// <summary>
        /// Regression test parsing a short color.
        /// </summary>
        [Test]
        public void TestDomainColorShort()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.color-short.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(2, tree.ChildCount);

            // parse into domain.
            MapCSSFile file = MapCSSDomainParser.Parse(tree);
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Rules.Count);
            Assert.AreEqual(1, file.Rules[0].Declarations.Count);
            Assert.IsInstanceOf(typeof(DeclarationInt), file.Rules[0].Declarations[0]);

            // get color declaration.
            var declarationInt = file.Rules[0].Declarations[0] as DeclarationInt;
            Assert.IsNotNull(declarationInt);
            Assert.AreEqual(DeclarationIntEnum.Color, declarationInt.Qualifier);

            // instantiate color.
            var simpleColor = new SimpleColor();
            simpleColor.Value = declarationInt.Eval(null);
            Assert.AreEqual("#665555", simpleColor.HexRgb);
        }

        /// <summary>
        /// Regression test parsing a color.
        /// </summary>
        [Test]
        public void TestDomainColor()
        {
            // parses the MapCSS.
            AstParserRuleReturnScope<object, IToken> result = this.TestMapCSSParsing(
                "OsmSharp.UI.Unittests.Data.MapCSS.color.mapcss");

            // Test the very minimum; no errors during parsing says a lot already!
            var tree = result.Tree as Antlr.Runtime.Tree.CommonTree;
            Assert.NotNull(tree);
            Assert.AreEqual(2, tree.ChildCount);

            // parse into domain.
            MapCSSFile file = MapCSSDomainParser.Parse(tree);
            Assert.IsNotNull(file);
            Assert.AreEqual(1, file.Rules.Count);
            Assert.AreEqual(1, file.Rules[0].Declarations.Count);
            Assert.IsInstanceOf(typeof(DeclarationInt), file.Rules[0].Declarations[0]);

            // get color declaration.
            var declarationInt = file.Rules[0].Declarations[0] as DeclarationInt;
            Assert.IsNotNull(declarationInt);
            Assert.AreEqual(DeclarationIntEnum.Color, declarationInt.Qualifier);

            // instantiate color.
            var simpleColor = new SimpleColor();
            simpleColor.Value = declarationInt.Eval(null);
            Assert.AreEqual("#00FF00", simpleColor.HexRgb);
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
