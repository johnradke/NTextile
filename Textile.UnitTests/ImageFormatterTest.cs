using NUnit.Framework;
using Textile.Blocks;

namespace Textile.UnitTests
{
    /// <summary>
    ///This is a test class for ImageFormatter and is intended
    ///to contain all ImageFormatter Unit Tests
    ///</summary>
    [TestFixture]
    public class ImageFormatterTest
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
    }
}
