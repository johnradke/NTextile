using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Textile.UnitTests
{
    public class TrimmingOutputter : Textile.IOutputter
    {
        StringBuilder m_output = new StringBuilder();

        #region IOutputter Members

        public void Begin()
        {
            m_output.Length = 0;
        }

        public void Write(string text)
        {
            text = text.Trim(' ');
            m_output.Append(text);
        }

        public void WriteLine(string line)
        {
            line = line.Trim(' ');
            m_output.AppendLine(line);
        }

        public void End()
        {
        }

        #endregion

        public override string ToString()
        {
            return m_output.ToString();
        }
    }

    [TestFixture]
    public class DataDrivenTest
    {
        private enum TestDataStep
        {
            Outside,
            ReadingIn,
            ReadingMultilineIn,
            ReadingOut,
            ReadingMultilineOut,
        }

        [Test]
        public void TestDataFiles()
        {
            string[] dataFiles = System.IO.Directory.GetFiles("..\\..\\TestData", "*.yml");
            foreach (string dataFile in dataFiles)
            {
                Console.Write(".");
                TestDataFiles(dataFile);
            }
        }

        private const string TestDataPreamblePattern = @"^[\-]{3,}\s*(#.*)?$";

        private void TestDataFiles(string dataFile)
        {
            string[] dataLines = System.IO.File.ReadAllLines(dataFile);

            bool didTest = false;
            StringBuilder inputData = new StringBuilder();
            StringBuilder outputData = new StringBuilder();
            TestDataStep step = TestDataStep.Outside;
            foreach (string dataLine in dataLines)
            {
                if (step == TestDataStep.Outside)
                {
                    Assert.IsTrue(Regex.IsMatch(dataLine, TestDataPreamblePattern), "Expected to find '---' before each test. File {0} misses one.", dataFile);
                    step = TestDataStep.ReadingIn;
                    continue;
                }
                
                if (step == TestDataStep.ReadingIn)
                {
                    if (Regex.IsMatch(dataLine, @"^in:\s*\|-\s*$"))
                    {
                        // multiline input data
                        step = TestDataStep.ReadingMultilineIn;
                        continue;
                    }
                    else
                    {
                        // single line input data
                        Match m = Regex.Match(dataLine, @"^in:\s*(?<input>.*)$");
                        Assert.IsTrue(m.Success, "Couldn't parse single line input data in file {0}: {1}", dataFile, dataLine);
                        string data = m.Groups["input"].Value.Trim(new char[] { ' ', '\'' });
                        inputData.Append(data);
                        step = TestDataStep.ReadingOut;
                        continue;
                    }
                }

                if (step == TestDataStep.ReadingMultilineIn)
                {
                    if (Regex.IsMatch(dataLine, @"^out:"))
                    {
                        // end of input data.
                        step = TestDataStep.ReadingOut;
                        // and we let the code continue into executing
                        // the output data parsing section below.
                    }
                    else
                    {
                        inputData.AppendLine(dataLine);
                        continue;
                    }
                }

                if (step == TestDataStep.ReadingOut)
                {
                    if (Regex.IsMatch(dataLine, @"^out:\s*\|-\s*$"))
                    {
                        // multiline output data
                        step = TestDataStep.ReadingMultilineOut;
                        continue;
                    }
                    else
                    {
                        // single line output data
                        Match m = Regex.Match(dataLine, @"^out:\s*(?<output>.*)$");
                        Assert.IsTrue(m.Success, "Couldn't parse single line output data in file {0}: {1}", dataFile, dataLine);
                        string data = m.Groups["output"].Value.Trim(new char[] { ' ', '\'' });
                        outputData.Append(data);

                        didTest = true;
                        DoTestFormatting(inputData.ToString(), outputData.ToString(), dataFile);
                        inputData.Length = 0;
                        outputData.Length = 0;

                        step = TestDataStep.Outside;
                        continue;
                    }
                }

                if (step == TestDataStep.ReadingMultilineOut)
                {
                    if (Regex.IsMatch(dataLine, TestDataPreamblePattern))
                    {
                        // end of output data.
                        step = TestDataStep.ReadingIn;

                        didTest = true;
                        DoTestFormatting(inputData.ToString(), outputData.ToString(), dataFile);
                        inputData.Length = 0;
                        outputData.Length = 0;

                        continue;
                    }
                    else
                    {
                        // Trim the spaces at the beginning and end of the line.
                        string trimmedDataLine = dataLine.Trim(' ');
                        outputData.AppendLine(trimmedDataLine);
                        continue;
                    }
                }
            }
            if (inputData.Length > 0 && outputData.Length > 0)
            {
                // Process the last test
                didTest = true;
                DoTestFormatting(inputData.ToString(), outputData.ToString(), dataFile);
            }

            Assert.IsTrue(didTest, "File {0} doesn't have any tests in it.", dataFile);
        }

        private void DoTestFormatting(string input, string expected, string dataFile)
        {
            TrimmingOutputter outputter = new TrimmingOutputter();
            Textile.TextileFormatter.FormatString(input, outputter);
            string result = outputter.ToString();
            result = CleanUpForAssertion(result);
            expected = CleanUpForAssertion(expected);
            Assert.AreEqual(expected, result, "Failed for file {0}, with input: {1}", dataFile, input);
        }

        private string CleanUpForAssertion(string input)
        {
            input = input.TrimEnd('\r', '\n');
            input = Regex.Replace(input, @"\\r|\r|\\n|\n|\\t|\t", "");
            return input;
        }
    }
}
