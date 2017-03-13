using NUnit.Framework;
using Textile.Blocks;

namespace Textile.UnitTests
{
    [TestFixture]
    public class GlyphsFormatterTest
    {
        [Test]
        public void GlyphsFormatTestDoubleQuotes()
        {
            string text = "Go and \"Observe!\", youg man.";
            string expected = "Go and &#8220;Observe!&#8221;, youg man.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestSimpleQuotes()
        {
            string text = "'Observe!'";
            string expected = "&#8216;Observe!&#8217;";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestEllipsis()
        {
            string text = "Observe...";
            string expected = "Observe&#8230;";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestAcronym()
        {
            string text = "We use CSS(Cascading Style Sheets).";
            string expected = "We use <acronym title=\"Cascading Style Sheets\">CSS</acronym>.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestEmDash()
        {
            string text = "Observe -- very nice!";
            string expected = "Observe &#8212; very nice!";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestEnDash()
        {
            string text = "Observe - tiny and brief.";
            string expected = "Observe &#8211; tiny and brief.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestDimensionSign()
        {
            string text = "Observe: 2 x 2.";
            string expected = "Observe: 2 &#215; 2.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestTrademark()
        {
            string text = "one(TM).";
            string expected = "one&#8482;.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestRegistered()
        {
            string text = "two(R).";
            string expected = "two&#174;.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestCopyright()
        {
            string text = "three(C).";
            string expected = "three&#169;.";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphFormatTestCopyrightAtBeginningOfLine()
        {
            string text = "(c) Company";
            string expected = "&#169; Company";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GlyphsFormatTestMultipleGlyphs()
        {
            string text = "\"'I swear, captain,' replied I.\"";
            string expected = "&#8220;&#8216;I swear, captain,&#8217; replied I.&#8221;";

            GlyphBlockModifier f = new GlyphBlockModifier();
            string actual = f.ModifyLine(text);

            Assert.AreEqual(expected, actual);
        }
    }
}
