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
    public class BlockAttributesParser
    {
        private static readonly Regex ColumnSpanRegex = new Regex(@"\\(\d+)", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex RowSpanRegex = new Regex(@"/(\d+)", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex VerticalAlignRegex = new Regex(@"(" + TextileGlobals.VerticalAlignPattern + @")", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex CustomStylesRegex = new Regex(@"\{([^}]*)\}", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex LanguageRegex = new Regex(@"\[([^()]+)\]", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex CssClassOrIdRegex = new Regex(@"\(([^()]+)\)", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex CssClassAndIdRegex = new Regex(@"^(.*)#(.*)$", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex PaddingLeftRegex = new Regex(@"([(]+)", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex PaddingRightRegex = new Regex(@"([)]+)", TextileGlobals.BlockModifierRegexOptions);
        private static readonly Regex TextAlignRegex = new Regex("(" + TextileGlobals.HorizontalAlignPattern + ")", TextileGlobals.BlockModifierRegexOptions);

        static public string ParseBlockAttributes(string input)
        {
            return ParseBlockAttributes(input, "");
        }

        static public string ParseBlockAttributes(string input, string element)
        {
            return ParseBlockAttributes(input, element, false);
        }

        static public string ParseBlockAttributes(string input, bool restrictedMode)
        {
            return ParseBlockAttributes(input, "", restrictedMode);
        }

        static public string ParseBlockAttributes(string input, string element, bool restrictedMode)
        {
            if (input.Length == 0)
                return string.Empty;

            string style = string.Empty;
            string cssClass = string.Empty;
            string lang = string.Empty;
            string colspan = string.Empty;
            string rowspan = string.Empty;
            string id = string.Empty;
            string atts = string.Empty;

            Match m;
            string matched = input;
            if (element == "td")
            {
                m = ColumnSpanRegex.Match(matched);
                if (m.Success)
                {
                    colspan = m.Groups[1].Value;
                }
                
                m = RowSpanRegex.Match(matched);
                if (m.Success)
                {
                    rowspan = m.Groups[1].Value;
                }

                m = VerticalAlignRegex.Match(matched);
                if (m.Success)
                {
                    style += "vertical-align: " + TextileGlobals.VerticalAlign[m.Captures[0].Value] + ";";
                }
            }

            m = CustomStylesRegex.Match(matched);
            if (m.Success)
            {
                style += m.Groups[1].Value + ";";
                matched = matched.Replace(m.ToString(), "");
            }

            m = LanguageRegex.Match(matched);
            if (m.Success)
            {
                lang = m.Groups[1].Value;
                matched = matched.Replace(m.ToString(), "");
            }

            // If we're in restricted mode, stop here and only return the language. Anything
            // else is disabled.
            if (restrictedMode)
            {
                return lang.Length > 0 ? $" lang=\"{lang}\"" : "";
            }

            m = CssClassOrIdRegex.Match(matched);
            if (m.Success)
            {
                cssClass = m.Groups[1].Value;
                matched = matched.Replace(m.ToString(), "");

                // Separate the class and the ID
                m = CssClassAndIdRegex.Match(cssClass);
                if (m.Success)
                {
                    cssClass = m.Groups[1].Value;
                    id = m.Groups[2].Value;
                }
            }

            m = PaddingLeftRegex.Match(matched);
            if (m.Success)
            {
                style += $"padding-left: {m.Groups[1].Length}em;";
                matched = matched.Replace(m.ToString(), "");
            }

            m = PaddingRightRegex.Match(matched);
            if (m.Success)
            {
                style += $"padding-right: {m.Groups[1].Length}em;";
                matched = matched.Replace(m.ToString(), "");
            }

            m = TextAlignRegex.Match(matched);
            if (m.Success)
            {
                style += $"text-align: {TextileGlobals.HorizontalAlign[m.Groups[1].Value]};";
            }

            return (style.Length > 0 ? $" style=\"{style}\"" : "") +
                (cssClass.Length > 0 ? $" class=\"{cssClass}\"" : "") +
                (lang.Length > 0 ? $" lang=\"{lang}\"" : "") +
                (id.Length > 0 ? $" id=\"{id}\"" : "") +
                (colspan.Length > 0 ? $" colspan=\"{colspan}\"" : "") +
                (rowspan.Length > 0 ? $" rowspan=\"{rowspan}\"" : "");
        }
    }
}
