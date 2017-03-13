namespace Textile.States
{
    [FormatterState(@"^\s*<(h[0-9]|p|pre|blockquote)" + TextileGlobals.HtmlAttributesPattern + ">")]
    public class PassthroughFormatterState : FormatterState
    {
		public override string Consume(FormatterStateConsumeContext context)
        {
            Formatter.ChangeState(this);
            return context.Input;
        }

        public override void FormatLine(string input)
        {
            WriteLine(input);
        }

        public override bool ShouldExit(string input, string inputLookAhead) => true;
    }
}
