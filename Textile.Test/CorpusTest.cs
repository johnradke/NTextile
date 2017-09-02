using NUnit.Framework;
using System;
using System.Drawing;
using System.IO;

namespace Textile.Test
{
    public class CorpusTest
    {
        [TestCaseSource(typeof(CorpusTestData))]
        public void TestStuff(string input, string expected)
        {
            var outputter = new StringBuilderOutputter();
            var formatter = new TextileFormatter(outputter);
            formatter.Format(input);
            var actual = outputter.GetFormattedText();

            Bitmap diff;
            if (!actual.RendersEqual(expected, out diff))
            {
                var path = Path.Combine(Path.GetTempPath(), "Textile.Test");

                if (Directory.Exists(path))
                {
                    
                }

                Directory.CreateDirectory(path);
                var filename = Path.Combine(path, $"{TestContext.CurrentContext.Test.Name}.png");

                diff.Save(filename);

                Assert.Fail($"HTML does not match.\r\nExpected:\r\n{expected}\r\n\r\nActual:\r\n{actual}\r\n\r\nDiff image: {filename}");
            }
        }
    }
}
