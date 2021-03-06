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
    public abstract class PhraseBlockModifier : BlockModifier
    {
        public static string GetPhraseModifierPattern(string modifier)
        {
            // All phrase modifiers are one character, or a double character. Sometimes,
            // there's an additional escape character for the regex ('\').
            var compressedModifier = modifier;
            if (modifier.Length == 4)
            {
                compressedModifier = modifier.Substring(0, 2);
            }
            else if (modifier.Length == 2)
            {
                if (modifier[0] != '\\')
                {
                    compressedModifier = modifier.Substring(0, 1);
                }
            }

            // We try to remove the Textile tag used for the formatting from
            // the punctuation pattern, so that we match the end of the formatted
            // zone correctly.
            string punctuationPattern = TextileGlobals.PunctuationPattern.Replace(compressedModifier, "");

            string pattern = @"(?<=\s|" + punctuationPattern + @"|[{\(\[]|^)" +
                                modifier +
                                TextileGlobals.BlockModifiersPattern +
                                @"(:(?<cite>(\S+)))?" +
                                @"(?<content>[^" + compressedModifier + "]*)" +
                                @"(?<end>" + punctuationPattern + @"*)" +
                                modifier +
                                @"(?=[\]\)}]|" + punctuationPattern + @"+|\s|$)";
            return pattern;
        }

        protected PhraseBlockModifier() { }

        protected string PhraseModifierFormat(string input, Regex regex, string tag)
        {
            return regex.Replace(input, m => Eval(m, tag, UseRestrictedMode));
        }

        private static string Eval(Match m, string tag, bool restrictedMode)
        {
            if (m.Groups["content"].Length == 0)
            {
                // It's possible that the "atts" match groups eats the contents
                // when the user didn't want to give block attributes, but the content
                // happens to match the syntax. For example: "*(blah)*".
                if (m.Groups["atts"].Length == 0)
                {
                    return m.ToString();
                }

                return $"<{tag}>{m.Groups["atts"].Value}{m.Groups["end"].Value}</{tag}>";
            }

            var atts = BlockAttributesParser.Parse(m.Groups["atts"].Value, "", restrictedMode);
            if (m.Groups["cite"].Length > 0)
            {
                atts += $" cite=\"{m.Groups["cite"]}\"";
            }

            return $"<{tag}{atts}>{m.Groups["content"].Value}{m.Groups["end"].Value}</{tag}>";
        }
    }
}
