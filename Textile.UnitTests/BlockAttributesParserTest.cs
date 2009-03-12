using NUnit.Framework;
using Textile.Blocks;

namespace Textile.UnitTests
{
    /// <summary>
    ///This is a test class for BlockAttributesParser and is intended
    ///to contain all BlockAttributesParser Unit Tests
    ///</summary>
    [TestFixture]
    public class BlockAttributesParserTest
    {
        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [SetUp]
        public void Initialize()
        {
            //  TODO: Add test initialization code
        }

        /// <summary>
        ///Cleanup() is called once during test execution after
        ///test methods in this class have executed unless
        ///this test class' Initialize() method throws an exception.
        ///</summary>
        [TearDown]
        public void Cleanup()
        {
            //  TODO: Add test cleanup code
        }


        [Test]
        public void ParseBlockAttributesTestClass()
        {
            string input = "(example1)"; // TODO: Initialize to an appropriate value
            string element = ""; // TODO: Initialize to an appropriate value

            string expected = " class=\"example1\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesTestId()
        {
            string input = "(#big-red)"; // TODO: Initialize to an appropriate value
            string element = ""; // TODO: Initialize to an appropriate value

            string expected = " id=\"big-red\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesTestClassAndId()
        {
            string input = "(example1#big-red2)"; // TODO: Initialize to an appropriate value
            string element = ""; // TODO: Initialize to an appropriate value

            string expected = " class=\"example1\" id=\"big-red2\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesTestCustomStyle()
        {
            string input = "{color:blue;margin:30px}"; // TODO: Initialize to an appropriate value
            string element = ""; // TODO: Initialize to an appropriate value

            string expected = " style=\"color:blue;margin:30px;\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesTestLanguage()
        {
            string input = "[fr]"; // TODO: Initialize to an appropriate value
            string element = ""; // TODO: Initialize to an appropriate value

            string expected = " lang=\"fr\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesBlockAlign()
        {
            string element = "";

            string input = "<";
            string expected = " style=\"text-align:left;\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            input = ">";
            expected = " style=\"text-align:right;\"";
            actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            input = "=";
            expected = " style=\"text-align:center;\"";
            actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            input = "<>";
            expected = " style=\"text-align:justify;\"";
            actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesTestBlockIndent()
        {
            string element = "";

            string input = "(";
            string expected = " style=\"padding-left:1em;\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            input = "((";
            expected = " style=\"padding-left:2em;\"";
            actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            input = ")))";
            expected = "style=\"padding-right:3em;\"";
            actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            input = ")";
            expected = " style=\"padding-right:1em;\"";
            actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesCombinedAlignsAndIndents()
        {
            string element = "";

            string input = "()>";
            string expected = " style=\"padding-left:1em;padding-right:1em;text-align:right;\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseBlockAttributesCombinedAll()
        {
            string element = "";

            string input = "()>[no]{color:red}";
            string expected = " style=\"color:red;padding-left:1em;padding-right:1em;text-align:right;\" lang=\"no\"";
            string actual = BlockAttributesParser.ParseBlockAttributes(input, element);

            Assert.AreEqual(expected, actual);
        }
    }
}
