using NUnit.Framework;
using Textile.Blocks;

namespace Textile.Test
{
    [TestFixture]
    public class ImageFormatterTest
    {
        [Test]
        public void ImagesFormatTestSimple()
        {
            string tmp = "!http://hobix.com/sample.jpg!";
            string expected = "<img src=\"http://hobix.com/sample.jpg\" alt=\"\" />";

            ImageBlockModifier f = new ImageBlockModifier();
            string actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ImagesFormatTestInSentence()
        {
            string tmp = "Here's a sample image: !http://hobix.com/sample.jpg! It's cool!";
            string expected = "Here's a sample image: <img src=\"http://hobix.com/sample.jpg\" alt=\"\" /> It's cool!";

            ImageBlockModifier f = new ImageBlockModifier();
            string actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ImagesFormatTestWithTitle()
        {
            string tmp = "!openwindow1.gif(Bunny.)!";
            string expected = "<img src=\"openwindow1.gif\" title=\"Bunny.\" alt=\"Bunny.\" />";

            ImageBlockModifier f = new ImageBlockModifier();
            string actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ImagesFormatTestWithLink()
        {
            string tmp = "!openwindow1.gif!:http://hobix.com/";
            string expected = "<a href=\"http://hobix.com/\"><img src=\"openwindow1.gif\" alt=\"\" /></a>";

            ImageBlockModifier f = new ImageBlockModifier();
            string actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ImagesFormatTestAlignments()
        {
            string tmp = "!>obake.gif!";
            string expected = "<img src=\"obake.gif\" align=\"right\" alt=\"\" />";

            ImageBlockModifier f = new ImageBlockModifier();
            string actual = f.ModifyLine(tmp);

            tmp = "!<obake.gif!";
            expected = "<img src=\"obake.gif\" align=\"left\" alt=\"\" />";
            actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ImagesFormatTestCheapHack()
        {
            string tmp = "!http://foo.com/fake.png\"/style=\"xss!";
            string expected = "<img src=\"#\" alt=\"\" />";

            ImageBlockModifier f = new ImageBlockModifier();
            string actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }
    }
}
