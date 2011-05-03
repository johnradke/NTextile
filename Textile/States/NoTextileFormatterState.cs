using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*((?<tag><notextile>)\s*$|(?<tag>(notextile.))\s*)")]
    public class NoTextileFormatterState : FormatterState
    {
        bool m_shouldExitNextTime = false;

        public NoTextileFormatterState()
        {
        }

		public override string Consume(FormatterStateConsumeContext context)
        {
            Formatter.ChangeState(this);

            if (context.Match.Groups["tag"].Value == "<notextile>")
                return string.Empty;
            return context.Input.Substring(context.Match.Length);
        }

        public override bool ShouldNestState(FormatterState other)
        {
            return false;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void FormatLine(string input)
        {
            if (!m_shouldExitNextTime)
                Formatter.Output.WriteLine(input);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (m_shouldExitNextTime)
                return true;
            m_shouldExitNextTime = Regex.IsMatch(input, @"^\s*</notextile>\s*$");
            return false;
        }

        public override bool ShouldFormatBlocks(string input)
        {
            return false;
        }

        public override bool ShouldParseForNewFormatterState(string input)
        {
            return false;
        }

        public override Type FallbackFormattingState
        {
            get
            {
                return null;
            }
        }
    }
}
