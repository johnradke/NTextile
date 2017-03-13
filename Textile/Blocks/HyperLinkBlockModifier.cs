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

using System.Text.RegularExpressions;

namespace Textile.Blocks
{
    public class HyperLinkBlockModifier : BlockModifier
    {
        private string _rel = string.Empty;

        public override string ModifyLine(string line)
        {
            line = Regex.Replace(line,
                @"(?<pre>[\s[{(]|" + TextileGlobals.PunctuationPattern + @")?" +       // $pre
                "\"" +									// start
                TextileGlobals.BlockModifiersPattern +			// attributes
                "(?<text>[^\"(]+)" +					// text
                @"\s?" +
			    @"(?:\((?<title>[^)]+)\)(?=""))?" +		// title
                "\":" +
                @"(?<url>\S+\b)" +						// url
                @"(?<slash>\/)?" +						// slash
                @"(?<post>[^\w\/;]*)" +					// post
                @"(?=\s|$)",
                new MatchEvaluator(HyperLinksFormatMatchEvaluator));

            return line;
        }

        private string HyperLinksFormatMatchEvaluator(Match m)
        {
            var atts = BlockAttributesParser.ParseBlockAttributes(m.Groups["atts"].Value, "", UseRestrictedMode);
            if (m.Groups["title"].Length > 0)
            {
                atts += $" title=\"{m.Groups["title"].Value}\"";
            }

            var linkText = m.Groups["text"].Value.Trim(' ');

            var url = TextileGlobals.EncodeHTMLLink(m.Groups["url"].Value) + m.Groups["slash"].Value;
            if (url.Contains("\""))
            {
                // Disable the URL if someone's trying a cheap hack.
                url = "#";
            }

            var str = $"{m.Groups["pre"].Value}<a ";
            if (_rel != null && _rel != string.Empty)
            {
                str += $"ref=\"{_rel}\" ";
            }

			str += $"href=\"{url}\"{atts}>{linkText}</a>{m.Groups["post"].Value}";

            return str;
        }
    }
}
