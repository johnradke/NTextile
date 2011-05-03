using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*<pre" + TextileGlobals.HtmlAttributesPattern + ">")]
    public class PreFormatterState : FormatterState
    {
        bool m_shouldExitNextTime = false;
        int m_fakeNestingDepth = 0;

        public PreFormatterState()
        {
        }

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

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void FormatLine(string input)
        {
            if (Regex.IsMatch(input, "<pre>"))
                m_fakeNestingDepth++;

            Formatter.Output.WriteLine(input);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (m_shouldExitNextTime)
                return true;
            if (Regex.IsMatch(input, @"</pre>"))
                m_fakeNestingDepth--;
            if (m_fakeNestingDepth <= 0)
                m_shouldExitNextTime = true;
            return false;
        }

        public override bool ShouldFormatBlocks(string input)
        {
            return false;
        }

        public override bool ShouldParseForNewFormatterState(string input)
        {
            // Only allow a child formatting state for <code> tag.
            return Regex.IsMatch(input, @"^\s*<code");
        }

        public override Type FallbackFormattingState
        {
            get { return null; }
        }
    }
}
