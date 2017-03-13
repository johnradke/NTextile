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

namespace Textile.States
{
    /// <summary>
    /// Formatting state for a standard text (i.e. just paragraphs).
    /// </summary>
    [FormatterState(TextilePatternBegin + @"p" + TextilePatternEnd)]
    public class ParagraphFormatterState : SimpleBlockFormatterState
    {
        public override void Enter()
        {
            Write("<p" + FormattedStylesAndAlignment() + ">");
        }

        public override void Exit()
        {
            WriteLine("</p>");
        }

        public override void FormatLine(string input)
        {
            Write(input);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (Regex.IsMatch(input, @"^\s*$"))
            {
                return true;
            }

            WriteLine("<br />");

            return false;
        }

        public override bool ShouldNestState(FormatterState other) => false;
    }
}
