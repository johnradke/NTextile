using NUnit.Framework;
using Textile.Blocks;

namespace Textile.Test
{
    [TestFixture]
    public class FootNotesReferencesFormatterTest
    {
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
