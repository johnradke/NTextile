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


namespace Textile.States
{
    [FormatterState(SimpleBlockFormatterState.TextilePatternBegin + @"bq" + SimpleBlockFormatterState.TextileDoubleDotPatternEnd)]
	public class BlockQuoteFormatterState : SimpleBlockFormatterState
	{
        private bool m_isDoubleDot = false;
        private bool m_lastLineWasBlank = false;

        public BlockQuoteFormatterState()
        {
        }

		public override void Enter()
		{
            Formatter.Output.Write("<blockquote" + FormattedStylesAndAlignment() + "><p>");
		}

        public override string Consume(FormatterStateConsumeContext context)
        {
            m_isDoubleDot = context.Match.Groups["ddot"].Success;
            return base.Consume(context);
        }

		public override void Exit()
		{
			Formatter.Output.WriteLine("</p></blockquote>");
		}

		public override void FormatLine(string input)
		{
			Formatter.Output.Write(input);
		}

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            bool lastLineWasBlank = m_lastLineWasBlank;
            bool isBlankLine = Regex.IsMatch(input, @"^\s*$");
            m_lastLineWasBlank = isBlankLine;

            // If the user specified the double-dot syntax, we exit only if a valid
            // state pattern is given after a blank line.
            if (m_isDoubleDot)
            {
                bool isLookAheadNewState = Formatter.HasCandidateFormatterStateType(input, inputLookAhead);
                if (lastLineWasBlank && isLookAheadNewState)
                    return true;

                if (lastLineWasBlank)
                {
                    // New paragraph in the blockquote.
                    Formatter.Output.WriteLine("</p>");
                    Formatter.Output.Write("<p>");
                }
                return false;
            }

            // If we're using the regular syntax, we exit after a blank line. Otherwise,
            // we just insert a line break.
            if (isBlankLine)
                return true;
            Formatter.Output.WriteLine("<br />");
            return false;
        }

        public override Type FallbackFormattingState
        {
            get { return null; }
        }
    }
}
