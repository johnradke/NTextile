using NUnit.Framework;
using Textile.Blocks;

namespace Textile.UnitTests
{
    /// <summary>
    ///This is a test class for Textile.FootNotesReferencesFormatter and is intended
    ///to contain all Textile.FootNotesReferencesFormatter Unit Tests
    ///</summary>
    [TestFixture]
    public class FootNotesReferencesFormatterTest
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


        /// <summary>
        ///A test case for FootNotesReferences (string)
        ///</summary>
        [Test]
        public void FootNotesReferencesTest()
        {
            string tmp = "This is covered elsewhere[1].";
            string expected = "This is covered elsewhere<sup><a href=\"#fn1\">1</a></sup>.";

            FootNoteReferenceBlockModifier f = new FootNoteReferenceBlockModifier();
            string actual = f.ModifyLine(tmp);

            Assert.AreEqual(expected, actual);
        }
    }
}
