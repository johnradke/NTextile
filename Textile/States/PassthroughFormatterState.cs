using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*<(h[0-9]|p|pre|blockquote)" + TextileGlobals.HtmlAttributesPattern + ">")]
    public class PassthroughFormatterState : FormatterState
    {
        public PassthroughFormatterState()
        {
        }

		public override string Consume(FormatterStateConsumeContext context)
        {
            Formatter.ChangeState(this);
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
            Formatter.Output.WriteLine(input);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            return true;
        }
    }
}
