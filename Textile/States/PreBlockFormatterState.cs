using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(SimpleBlockFormatterState.TextilePatternBegin + @"pre" + SimpleBlockFormatterState.TextilePatternEnd)]
    public class PreBlockFormatterState : SimpleBlockFormatterState
    {
        public PreBlockFormatterState()
        {
        }

		public override void Enter()
		{
            Formatter.Output.Write("<pre" + FormattedStylesAndAlignment() + ">");
		}

		public override void Exit()
		{
			Formatter.Output.WriteLine("</pre>");
		}

		public override void FormatLine(string input)
		{
			Formatter.Output.Write(input);
		}

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (Regex.IsMatch(input, @"^\s*$"))
                return true;
            Formatter.Output.WriteLine("");
            return false;
        }

        public override Type FallbackFormattingState
        {
            get { return null; }
        }
    }
}
