using System;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*((?<tag><notextile>)\s*$|(?<tag>(notextile.))\s*)")]
    public class NoTextileFormatterState : FormatterState
    {
        bool _shouldExitNextTime = false;

		public override string Consume(FormatterStateConsumeContext context)
        {
            ChangeState(this);

            if (context.Match.Groups["tag"].Value == "<notextile>")
            {
                return string.Empty;
            }

            return context.Input.Substring(context.Match.Length);
        }

        public override void FormatLine(string input)
        {
            if (!_shouldExitNextTime)
            {
                WriteLine(input);
            }
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (_shouldExitNextTime)
            {
                return true;
            }

            _shouldExitNextTime = Regex.IsMatch(input, @"^\s*</notextile>\s*$");
            return false;
        }

        public override bool ShouldFormatBlocks(string input) => false;
        public override bool ShouldParseForNewFormatterState(string input) => false;
        public override Type FallbackFormattingState => null;
    }
}
