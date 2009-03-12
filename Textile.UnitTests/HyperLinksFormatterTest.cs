using NUnit.Framework;
using Textile.Blocks;

namespace Textile.UnitTests
{
    /// <summary>
    ///This is a test class for HyperLinksFormatter and is intended
    ///to contain all HyperLinksFormatter Unit Tests
    ///</summary>
    [TestFixture]
    public class HyperLinksFormatterTest
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
    }
}
