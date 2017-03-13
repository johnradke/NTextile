#region License Statement
// Copyright (c) L.A.B.Soft.  All rights reserved.
//
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.
#endregion

using System.Text;
using System.Text.RegularExpressions;

namespace Textile.Blocks
{
    public class ImageBlockModifier : BlockModifier
    {
        private static readonly Regex HrefRegex = new Regex(@"(.*)(?<end>\.|,|;|\))$");

        public override string ModifyLine(string line)
        {
            line = Regex.Replace(line,
                @"\!" +                   // opening !
                @"(?<algn>\<|\=|\>)?" +   // optional alignment atts
                TextileGlobals.BlockModifiersPattern + // optional style, public class atts
                @"(?:\. )?" +             // optional dot-space
                @"(?<url>[^\s(!]+)" +     // presume this is the src
                @"\s?" +                  // optional space
                @"(?:\((?<title>([^\)]+))\))?" +// optional title
                @"\!" +                   // closing
                @"(?::(?<href>(\S+)))?" +     // optional href
                @"(?=\s|\.|,|;|\)|\||$)",               // lookahead: space or simple punctuation or end of string
                new MatchEvaluator(ImageFormatMatchEvaluator));

            return line;
        }

        private string ImageFormatMatchEvaluator(Match m)
        {
            var atts = new StringBuilder(BlockAttributesParser.ParseBlockAttributes(m.Groups["atts"].Value, "", UseRestrictedMode));
            if (m.Groups["algn"].Length > 0)
            {
                atts.Append($" align=\"{TextileGlobals.ImageAlign[m.Groups["algn"].Value]}\"");
            }

            if (m.Groups["title"].Length > 0)
            {
                atts.Append($" title=\"{m.Groups["title"].Value}\"");
                atts.Append($" alt=\"{m.Groups["title"].Value}\"");
            }
            else
            {
                atts.Append(" alt=\"\"");
            }

            // Validate the URL.
            var url = m.Groups["url"].Value;
            if (url.Contains("\""))
            {   
                // Disable the URL if someone's trying a cheap hack.
                url = "#";
            }

            string res = $"<img src=\"{url}\"{atts} />";

            if (m.Groups["href"].Length > 0)
            {
                var href = m.Groups["href"].Value;
                var end = string.Empty;
                var endMatch = HrefRegex.Match(href);
                if (m.Success && !string.IsNullOrEmpty(endMatch.Groups["end"].Value))
                {
                    href = href.Substring(0, href.Length - 1);
                    end = endMatch.Groups["end"].Value;
                }

                res = $"<a href=\"{TextileGlobals.EncodeHTMLLink(href)}\">{res}</a>{end}";
            }

            return res;
        }
    }
}
