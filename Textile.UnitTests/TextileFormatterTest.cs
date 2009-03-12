using Textile;
using NUnit.Framework;
namespace Textile.UnitTests
{
    /// <summary>
    ///This is a test class for ComicNET.TextileFormatter and is intended
    ///to contain all ComicNET.TextileFormatter Unit Tests
    ///</summary>
    [TestFixture]
    public class TextileFormatterTest
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
        public void FormatTestOneParagraph()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "A single paragraph.";

            target.Format(input);

            string expected = "<p>A single paragraph.</p>\n";
            string actual = output.GetOutput();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestTwoParagraphs()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "A single paragraph.\n"+
                           "\n"+
                           "Followed by another.";

            target.Format(input);

            string expected = "<p>A single paragraph.</p>\n" +
                              "<p>Followed by another.</p>\n";
            string actual = output.GetOutput();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestOneParagraphWithLineBreak()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "A single paragraph.\n" +
                           "Followed by the rest.";

            target.Format(input);

            string expected = "<p>A single paragraph.<br />\n" +
                              "Followed by the rest.</p>\n";
            string actual = output.GetOutput();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestHeaders()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "h1. A simple header";
            target.Format(input);
            string expected = "<h1>A simple header</h1>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);

            input = "h1. A simple header\n" +
                    "h2. A sub header\n" +
                    "h3. A sub sub header";
            target.Format(input);
            expected = "<h1>A simple header</h1>\n" +
                       "<h2>A sub header</h2>\n" +
                       "<h3>A sub sub header</h3>\n";
            actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestBlockQuote()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "An old text\n\n"+
                           "bq. A block quotation.\n\n"+
                           "Any old text";
            target.Format(input);
            string expected = "<p>An old text</p>\n"+
                              "<blockquote><p>A block quotation.</p></blockquote>\n"+
                              "<p>Any old text</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestFootNote()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "fn1. Down here, in fact.";
            target.Format(input);
            string expected = "<p id=\"fn1\"><sup>1</sup> Down here, in fact.</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestRandomSentences()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "\"Textile\":http://textism.com/tools/textile/ is a \"Humane Web Text Generator\", as they say.";
            target.Format(input);
            string expected = "<p><a href=\"http://textism.com/tools/textile/\">Textile</a> is a &#8220;Humane Web Text Generator&#8221;, as they say.</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestListSimple()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "# A first item\n" +
                            "# A second item\n" +
                            "# A third";
            target.Format(input);
            string expected = "<ol>\n" +
                            "<li>A first item</li>\n" +
                            "<li>A second item</li>\n" +
                            "<li>A third</li>\n" +
                            "</ol>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestListNested()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "# Fuel could be:\n" +
                            "## Coal\n" +
                            "## Gasoline\n" +
                            "## Electricity\n" +
                            "# Humans need only:\n" +
                            "## Water\n" +
                            "## Protein";
            target.Format(input);
            string expected = "<ol>\n" +
                            "<li>Fuel could be:" +
                            "<ol>\n" +
                            "<li>Coal</li>\n" +
                            "<li>Gasoline</li>\n" +
                            "<li>Electricity</li>\n" +
                            "</ol>\n" +
                            "</li>\n" +
                            "<li>Humans need only:" +
                            "<ol>\n" +
                            "<li>Water</li>\n" +
                            "<li>Protein</li>\n" +
                            "</ol>\n" +
                            "</li>\n" +
                            "</ol>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestParagraphWithClass()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "p(example1). An example.";
            target.Format(input);
            string expected = "<p class=\"example1\">An example.</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestParagraphWithID()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "p(#big-red). Red here!";
            target.Format(input);
            string expected = "<p id=\"big-red\">Red here!</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestParagraphWithClassAndID()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "p(example1#big-red2). Red here.";
            target.Format(input);
            string expected = "<p class=\"example1\" id=\"big-red2\">Red here.</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestSytledParagraph()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "p{color:blue;margin:30px}. Spacey blue.";
            target.Format(input);
            string expected = "<p style=\"color:blue;margin:30px;\">Spacey blue.</p>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestTableSimple()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "|name|age|sex|\n" +
                           "| joan arc| 24 | f |";
            target.Format(input);
            string expected = "<table>\n" +
                                "<tr>\n" +
                                "<td>name</td><td>age</td><td>sex</td>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td> joan arc</td><td> 24 </td><td> f </td>\n" +
                                "</tr>\n" +
                                "</table>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestTableWithHeaders()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "|_. name |_.age |_. sex|\n" +
                            "| joan | 24 | f |\n";
            target.Format(input);
            string expected = "<table>\n" +
                                "<tr>\n" +
                                "<th>name </th><th>age </th><th>sex</th>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td> joan </td><td> 24 </td><td> f </td>\n" +
                                "</tr>\n" +
                                "</table>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FormatTestTableWithCellAttributes()
        {
            TestOutputter output = new TestOutputter();
            TextileFormatter target = new TextileFormatter(output);

            string input = "|_. attribute list |\n" +
                            "|<. align left |\n" +
                            "|>. align right|\n" +
                            "|=. center |\n" +
                            "|<>. justify |\n" +
                            "|^. valign top |\n" +
                            "|~. bottom |\n";
            target.Format(input);
            string expected = "<table>\n" +
                                "<tr>\n" +
                                "<th>attribute list </th>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td style=\"text-align:left;\">align left </td>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td style=\"text-align:right;\">align right</td>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td style=\"text-align:center;\">center </td>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td style=\"text-align:justify;\">justify </td>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td style=\"vertical-align:top;\">valign top </td>\n" +
                                "</tr>\n" +
                                "<tr>\n" +
                                "<td style=\"vertical-align:bottom;\">bottom </td>\n" +
                                "</tr>\n" +
                                "</table>\n";
            string actual = output.GetOutput();
            Assert.AreEqual(expected, actual);
        }
    }
}
