using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace Textile.Test
{
    public class CorpusTestData : IEnumerable
    {
        private static Assembly Assembly => typeof(CorpusTest).Assembly;

        public IEnumerator GetEnumerator()
        {
            return GetTestCasesImpl(null).GetEnumerator();
        }

        private static TestCaseData Deserialize(YamlDocument document, string filename, int index)
        {
            var node = document.RootNode;

            var nodeName = node.Get("name");
            filename = string.Join(".", filename.Split('.').Reverse().Take(3).Reverse());

            return new TestCaseData(node.Get("in"), node.Get("html"))
                .SetName($"{filename}.{nodeName ?? (index + 1).ToString()}");
        }

        private static IEnumerable<string> GetEmbeddedYamlFilenames()
        {
            return Assembly.GetManifestResourceNames().Where(s => s.EndsWith(".yml"));
        }

        private static StreamReader GetYaml(string resourceName)
        {
            return new StreamReader(Assembly.GetManifestResourceStream(resourceName));
        }

        private static IEnumerable<TestCaseData> GetTestCasesImpl(Func<string, bool> filter)
        {
            var filenames = GetEmbeddedYamlFilenames();
            if (filter != null)
            {
                filenames = filenames.Where(filter);
            }

            foreach (var filename in filenames)
            {
                var yaml = new YamlStream();
                yaml.Load(GetYaml(filename));

                for (var i = 0; i < yaml.Documents.Count; i++)
                {
                    yield return Deserialize(yaml.Documents[i], filename, i);
                }
            }
        }
    }

    internal static class YamlExtensions
    {
        public static string Get(this YamlNode node, string key)
        {
            var map = ((YamlMappingNode)node).Children;
            var nodeKey = new YamlScalarNode(key);

            return !map.ContainsKey(nodeKey) ? null : map[nodeKey].ToString();
        }
    }
}
