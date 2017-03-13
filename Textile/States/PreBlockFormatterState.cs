using System;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(TextilePatternBegin + @"pre" + TextilePatternEnd)]
    public class PreBlockFormatterState : SimpleBlockFormatterState
    {
		public override void Enter()
		{
            Write("<pre" + FormattedStylesAndAlignment() + ">");
		}

		public override void Exit()
		{
			WriteLine("</pre>");
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

            WriteLine();

            return false;
        }

        public override Type FallbackFormattingState => null;
    }
}
