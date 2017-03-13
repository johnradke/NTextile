using NUnit.Framework;
using System.Collections.Generic;
using Textile.Blocks;

namespace Textile.UnitTests
{
    [TestFixture]
    public class BlockAttributesParserTest
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData("(example1)").Returns(" class=\"example1\"").SetName("Class");
                yield return new TestCaseData("(#big-red)").Returns(" id=\"big-red\"").SetName("Id");
                yield return new TestCaseData("(example1#big-red2)").Returns(" class=\"example1\" id=\"big-red2\"").SetName("Class And Id");
                yield return new TestCaseData("{color:blue;margin:30px}").Returns(" style=\"color:blue;margin:30px;\"").SetName("Custom Style");
                yield return new TestCaseData("[fr]").Returns(" lang=\"fr\"").SetName("Language");
                yield return new TestCaseData("<").Returns(" style=\"text-align: left;\"").SetName("Align Left");
                yield return new TestCaseData(">").Returns(" style=\"text-align: right;\"").SetName("Align Right");
                yield return new TestCaseData("=").Returns(" style=\"text-align: center;\"").SetName("Align Center");
                yield return new TestCaseData("<>").Returns(" style=\"text-align: justify;\"").SetName("Align Justify");
                yield return new TestCaseData("(").Returns(" style=\"padding-left: 1em;\"").SetName("Indent Left 1");
                yield return new TestCaseData("((").Returns(" style=\"padding-left: 2em;\"").SetName("Indent Left 2");
                yield return new TestCaseData(")))").Returns(" style=\"padding-right: 3em;\"").SetName("Indent Right 3");
                yield return new TestCaseData(")").Returns(" style=\"padding-right: 1em;\"").SetName("Indent Right 1");
                yield return new TestCaseData("()>").Returns(" style=\"padding-left: 1em;padding-right: 1em;text-align: right;\"").SetName("Align & Indent");
                yield return new TestCaseData("()>[no]{color:red}").Returns(" style=\"color:red;padding-left: 1em;padding-right: 1em;text-align: right;\" lang=\"no\"").SetName("All");
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public string ParseBlockAttribute(string input)
        {
            return BlockAttributesParser.Parse(input);
        }

        private static IEnumerable<TestCaseData> RestrictedTestCases
        {
            get
            {
                yield return new TestCaseData("[fr]").Returns(" lang=\"fr\"").SetName("Restricted-Language");
                yield return new TestCaseData("()>[no]{color:red}").Returns(" lang=\"no\"").SetName("Restricted-All");
                yield return new TestCaseData("()>{color:red}").Returns("").SetName("Restricted-NoLang");
            }
        }

        [TestCaseSource(nameof(RestrictedTestCases))]
        public string ParseRestrictedAttributes(string input)
        {
            return BlockAttributesParser.Parse(input, true);
        }
    }
}
