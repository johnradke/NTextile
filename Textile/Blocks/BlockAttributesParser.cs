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

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#endregion


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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string ParseBlockAttributes(string input)
        {
            return ParseBlockAttributes(input, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        static public string ParseBlockAttributes(string input, string element)
        {
            return ParseBlockAttributes(input, element, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="restrictedMode"></param>
        /// <returns></returns>
        static public string ParseBlockAttributes(string input, bool restrictedMode)
        {
            return ParseBlockAttributes(input, "", restrictedMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="element"></param>
        /// <param name="restrictedMode"></param>
        /// <returns></returns>
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
				// column span
                m = ColumnSpanRegex.Match(matched);
                if (m.Success)
                    colspan = m.Groups[1].Value;
				// row span
                m = RowSpanRegex.Match(matched);
                if (m.Success)
                    rowspan = m.Groups[1].Value;
				// vertical align
                m = VerticalAlignRegex.Match(matched);
                if (m.Success)
                    style += "vertical-align:" + TextileGlobals.VerticalAlign[m.Captures[0].Value] + ";";
            }

            // First, match custom styles
            m = CustomStylesRegex.Match(matched);
            if (m.Success)
            {
                style += m.Groups[1].Value + ";";
                matched = matched.Replace(m.ToString(), "");
            }

            // Then match the language
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
                return (lang.Length > 0 ? " lang=\"" + lang + "\"" : "");
            }

            // Match classes and IDs after that
            m = CssClassOrIdRegex.Match(matched);
            if (m.Success)
            {
                cssClass = m.Groups[1].Value;
                matched = matched.Replace(m.ToString(), "");

                // Separate the public class and the ID
                m = CssClassAndIdRegex.Match(cssClass);
                if (m.Success)
                {
                    cssClass = m.Groups[1].Value;
                    id = m.Groups[2].Value;
                }
            }

            // Get the padding on the left
            m = PaddingLeftRegex.Match(matched);
            if (m.Success)
            {
                style += "padding-left:" + m.Groups[1].Length + "em;";
                matched = matched.Replace(m.ToString(), "");
            }

            // Get the padding on the right
            m = PaddingRightRegex.Match(matched);
            if (m.Success)
            {
                style += "padding-right:" + m.Groups[1].Length + "em;";
                matched = matched.Replace(m.ToString(), "");
            }

            // Get the text alignment
            m = TextAlignRegex.Match(matched);
            if (m.Success)
                style += "text-align:" + TextileGlobals.HorizontalAlign[m.Groups[1].Value] + ";";

            return (
                    (style.Length > 0 ? " style=\"" + style + "\"" : "") +
                    (cssClass.Length > 0 ? " class=\"" + cssClass + "\"" : "") +
                    (lang.Length > 0 ? " lang=\"" + lang + "\"" : "") +
                    (id.Length > 0 ? " id=\"" + id + "\"" : "") +
                    (colspan.Length > 0 ? " colspan=\"" + colspan + "\"" : "") +
                    (rowspan.Length > 0 ? " rowspan=\"" + rowspan + "\"" : "")
                   );
        }
    }
}
