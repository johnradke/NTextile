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
    public class GlyphBlockModifier : BlockModifier
    {
        private static readonly string[,] Glyphs =
        {
            { @"(\w)\'(\w)", "$1&#8217;$2" },                                       // apostrophe
            { @"(\s)\'(\d+\w?)\b(?!\')", "$1&#8217;$2" },                           // years ("back in '88")
            { @"(\S)\'(?=\s|" + TextileGlobals.PunctuationPattern + @"|<|$)", "$1&#8217;" },                     //  single closing
            { @"\'", "&#8216;" },                                                   //  single opening
            { @"(\S)""(?=\s|" + TextileGlobals.PunctuationPattern + @"|<|$)", "$1&#8221;" },                     //  double closing
            { @"""", "&#8220;" },                                                   //  double opening
            { @"\b([A-Z][A-Z0-9]{2,})\b(?:[(]([^)]*)[)])", "<acronym title=\"$2\">$1</acronym>" },// 3+ uppercase acronym
            { @"\b( )?\.{3}", "$1&#8230;" },                                        //  ellipsis
            { @"(\s)?--(\s)?", "$1&#8212;$2" },                                     //  em dash
            { @"\s-(?:\s|$)", " &#8211; " },                                        //  en dash
            { @"(\d+)( ?)x( ?)(?=\d+)", "$1$2&#215;$3" },                           //  dimension sign
            { @"(?:^|\b) ?[([](TM|tm)[])]", "&#8482;" },                                  //  trademark
            { @"(?:^|\b) ?[([](R|r)[])]", "&#174;" },                                     //  registered
            { @"(?:^|\b) ?[([](C|c)[])]", "&#169;" }                                      //  copyright
        };

        public override string ModifyLine(string line)
        {
            var htmlRegex = new Regex(@"(</?[\w\d]+(?:\s.*)?>)");
            if (!htmlRegex.IsMatch(line))
            {
                // If no HTML, do a simple search & replace.
                for (int i = 0; i < Glyphs.GetLength(0); ++i)
                {
                    line = Regex.Replace(line, Glyphs[i, 0], Glyphs[i, 1]);
                }
                return line;
            }

            var output = new StringBuilder();
            var splits = htmlRegex.Split(line);
            var offtags = "code|pre|notextile";
            var codepre = false;

            foreach (var split in splits)
            {
                var modifiedSplit = split;
                if (modifiedSplit.Length == 0)
                {
                    continue;
                }
                if (Regex.IsMatch(modifiedSplit, $@"<({offtags})>", RegexOptions.IgnoreCase))
                {
                    codepre = true;
                }
                if (Regex.IsMatch(modifiedSplit, $@"<\/({offtags})>", RegexOptions.IgnoreCase))
                {
                    codepre = false;
                }

                if (!htmlRegex.IsMatch(modifiedSplit) && !codepre)
                {
                    for (int i = 0; i < Glyphs.GetLength(0); ++i)
                    {
                        modifiedSplit = Regex.Replace(modifiedSplit, Glyphs[i, 0], Glyphs[i, 1]);
                    }
                }

                // do htmlspecial if between <code>
                if (codepre == true)
                {
                    //TODO: htmlspecialchars(line)
                    //line = Regex.Replace(line, @"&lt;(\/?" + offtags + ")&gt;", "<$1>");
                    //line = line.Replace("&amp;#", "&#");
                }

                output.Append(modifiedSplit);
            }

            return output.ToString();
        }
    }
}
