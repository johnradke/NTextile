using System;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*<pre" + TextileGlobals.HtmlAttributesPattern + ">")]
    public class PreFormatterState : FormatterState
    {
        bool _shouldExitNextTime = false;
        int _fakeNestingDepth = 0;

		public override string Consume(FormatterStateConsumeContext context)
        {
            if (!Regex.IsMatch(context.Input, "</pre>"))
            {
                Formatter.ChangeState(this);
            }
            else
            {
                Formatter.ChangeState(new PassthroughFormatterState());
            }

            return context.Input;
        }

        public override bool ShouldNestState(FormatterState other)
        {
            return false;
        }

        public override void FormatLine(string input)
        {
            if (Regex.IsMatch(input, "<pre>"))
            {
                _fakeNestingDepth++;
            }

            WriteLine(input);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (_shouldExitNextTime)
            {
                return true;
            }

            if (Regex.IsMatch(input, @"</pre>"))
            {
                _fakeNestingDepth--;
            }

            if (_fakeNestingDepth <= 0)
            {
                _shouldExitNextTime = true;
            }

            return false;
        }

        public override bool ShouldFormatBlocks(string input) => false;

        public override bool ShouldParseForNewFormatterState(string input)
        {
            // Only allow a child formatting state for <code> tag.
            return Regex.IsMatch(input, @"^\s*<code");
        }

        public override Type FallbackFormattingState => null;
    }
}
