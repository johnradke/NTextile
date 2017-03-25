using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Textile;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UnitTestProject1
{
    public class CorpusTest
    {
        private static Assembly Assembly => typeof(CorpusTest).Assembly;

        private static IEnumerable<string> GetEmbeddedYamlFilenames()
        {
            return Assembly.GetManifestResourceNames().Where(s => s.EndsWith(".yml"));
        }

        private static StreamReader GetYaml(string resourceName)
        {
            return new StreamReader(Assembly.GetManifestResourceStream(resourceName));
        }

        private static IEnumerable<IEnumerable<CorpusTestCase>> GetCorpusTestCases()
        {
            var yaml = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            foreach (var filename in GetEmbeddedYamlFilenames())
            {
                IEnumerable<CorpusTestCase> cases = null;
                try
                {
                    cases = yaml.DeserializeMultiple<CorpusTestCase>(GetYaml(filename)).ToList();
                }
                catch (Exception e)
                {
                    throw new YamlException($"Error in file {filename}", e);
                }

                yield return cases;
            }
        }

        private static IEnumerable<TestCaseData> GetTestCases()
        {
            return GetCorpusTestCases().SelectMany(c => c).Select(c => new TestCaseData(c.In)
                .Returns(c.Html)
                .SetName(c.Name)
                .SetDescription(c.Description ?? ""));
        }

        [TestCaseSource(nameof(GetTestCases))]
        public string TestStuff(string input)
        {
            var outputter = new StringBuilderOutputter();
            var formatter = new TextileFormatter(outputter);
            formatter.Format(input);

            return outputter.GetFormattedText();
        }
    }

    public class CorpusTestCase
    {
        public string Name { get; set; }
        public string Desc { set { Description = value; } get { throw new NotImplementedException(); } }
        public string Description { get; set; }
        public string In { get; set; }
        public string Html { get; set; }
        public string HtmlNoBreaks { get; set; }
        public bool ValidHtml { get; set; }
        public string LiteModeHtml { get; set; }
        public string Latex { get; set; }
        public string NoSpanCapsHtml { get; set; }
        public string SanitizedHtml { get; set; }
        public string FilteredHtml { get; set; }
        public string StyleFilteredHtml { get; set; }
        public string ClassFilteredHtml { get; set; }
        public string IdFilteredHtml { get; set; }
        public string Comment { get; set; }
        public string Comments { get; set; }
        public string Note { get; set; }
    }

    public static class YamlExtensions
    {
        public static IEnumerable<T> DeserializeMultiple<T>(this Deserializer deserializer, StreamReader input)
        {
            // adapted from https://github.com/aaubry/YamlDotNet/wiki/Samples.DeserializingMultipleDocuments
            var parser = new Parser(input);
            parser.Expect<StreamStart>();
            
            while (parser.Accept<DocumentStart>())
            {
                yield return deserializer.Deserialize<T>(parser);
            }
        }
    }
}
