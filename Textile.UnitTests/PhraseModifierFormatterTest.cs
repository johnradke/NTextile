using NUnit.Framework;
using Textile.Blocks;

namespace Textile.UnitTests
{
    /// <summary>
    ///This is a test class for PhraseModifierFormatter and is intended
    ///to contain all PhraseModifierFormatter Unit Tests
    ///</summary>
    [TestFixture]
    public class PhraseModifierFormatterTest
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
        public void PhraseModifierFormatTestEmphasis()
        {
            string input = "I _believe_ every word.";
            string expected = "I <em>believe</em> every word.";
            EmphasisPhraseBlockModifier f = new EmphasisPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestMultipleEmphasis()
        {
            string input = "I _believe_ every word from _you_ my friend.";
            string expected = "I <em>believe</em> every word from <em>you</em> my friend.";
            EmphasisPhraseBlockModifier f = new EmphasisPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestStrong()
        {
            string input = "And then? She *fell*!";
            string expected = "And then? She <strong>fell</strong>!";
            StrongPhraseBlockModifier f = new StrongPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestItalic()
        {
            string input = "I __know__.";
            string expected = "I <i>know</i>.";
            ItalicPhraseBlockModifier f = new ItalicPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestBold()
        {
            string input = "I **know**.";
            string expected = "I <b>know</b>.";
            BoldPhraseBlockModifier f = new BoldPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestCitation()
        {
            string input = "??Cat's Cradle?? by Vonnegut.";
            string expected = "<cite>Cat's Cradle</cite> by Vonnegut.";
            CitePhraseBlockModifier f = new CitePhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestCode()
        {
            string input = "Convert with @r.to_html@";
            string expected = "Convert with <code>r.to_html</code>";
            CodeBlockModifier f = new CodeBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestDeleted()
        {
            string input = "I'm -sure- not sure.";
            string expected = "I'm <del>sure</del> not sure.";
            DeletedPhraseBlockModifier f = new DeletedPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestInsertion()
        {
            string input = "You are a +pleasant+ child.";
            string expected = "You are a <ins>pleasant</ins> child.";
            InsertedPhraseBlockModifier f = new InsertedPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestSuperScript()
        {
            string input = "a ^2^ + b";
            string expected = "a <sup>2</sup> + b";
            SuperScriptPhraseBlockModifier f = new SuperScriptPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestSubScript()
        {
            string input = "log ~2~ x";
            string expected = "log <sub>2</sub> x";
            SubScriptPhraseBlockModifier f = new SubScriptPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestSimpleSpan()
        {
            string input = "I'm %unaware% of most soft drinks.";
            string expected = "I'm <span>unaware</span> of most soft drinks.";
            SpanPhraseBlockModifier f = new SpanPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestStyledSpan()
        {
            string input = "I'm %{color:red}unaware% of most soft drinks.";
            string expected = "I'm <span style=\"color:red;\">unaware</span> of most soft drinks.";
            SpanPhraseBlockModifier f = new SpanPhraseBlockModifier();
            string actual = FormatLine(f, input);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PhraseModifierFormatTestAllMixed()
        {
            string input = "I seriously *{color:red}blushed* when I _(big)sprouted_ that corn stalk from my %[es]cabeza%.";
            string expected = "<p>I seriously <strong style=\"color:red;\">blushed</strong> when I <em class=\"big\">sprouted</em> that corn stalk from my <span lang=\"es\">cabeza</span>.</p>\r\n";
            string actual = TextileFormatter.FormatString(input);

            Assert.AreEqual(expected, actual);
        }

        private string FormatLine(BlockModifier m, string input)
        {
            input = m.ModifyLine(input);
            return m.Conclude(input);
        }
    }
}
