using NUnit.Framework;
using Textile.Blocks;

namespace Textile.Test
{
    [TestFixture]
    public class HyperLinksFormatterTest
    {
        [Test]
        public void HyperLinksFormatSimpleTest()
        {
            string input = "\"Google\":http://google.com";
            string expected = "<a href=\"http://google.com\">Google</a>";

            HyperLinkBlockModifier f = new HyperLinkBlockModifier();
            string actual = f.ModifyLine(input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void HyperLinksFormatTestInSentence()
        {
            string input = "I searched \"Google\":http://google.com. I found lots of stuff.";
            string expected = "I searched <a href=\"http://google.com\">Google</a>. I found lots of stuff.";

            HyperLinkBlockModifier f = new HyperLinkBlockModifier();
            string actual = f.ModifyLine(input);

            Assert.AreEqual(expected, actual);
        }

		[Test]
		public void HyperLinksFormatTestWithTitle()
		{
			string input = "\"SourceForge (Repository of Open Source software)\":http://www.sourceforge.net";
			string expected = "<a href=\"http://www.sourceforge.net\" title=\"Repository of Open Source software\">SourceForge</a>";

            HyperLinkBlockModifier f = new HyperLinkBlockModifier();
			string actual = f.ModifyLine(input);

			Assert.AreEqual(expected, actual);
		}

        [Test]
        public void HyperLinksFormatTestCheapHack()
        {
            string input = "\"A cheap hack\":http://example.org\"/style=\"attack";
            string expected = "<a href=\"#\">A cheap hack</a>";

            HyperLinkBlockModifier f = new HyperLinkBlockModifier();
            string actual = f.ModifyLine(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
