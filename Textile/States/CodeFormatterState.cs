using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*<code" + TextileGlobals.HtmlAttributesPattern + ">")]
    public class CodeFormatterState : FormatterState
    {
        bool _shouldExitNextTime = false;
        bool _shouldFixHtmlEntities = false;

		public override string Consume(FormatterStateConsumeContext context)
        {
            if (!Regex.IsMatch(context.Input, "</code>"))
            {
                ChangeState(this);
            }
            else
            {
                ChangeState(new PassthroughFormatterState());
            }
            return context.Input;
        }

        public override bool ShouldNestState(FormatterState other) => true;

        public override void Enter()
        {
            _shouldFixHtmlEntities = false;
        }

        public override void FormatLine(string input)
        {
            if (_shouldFixHtmlEntities)
            {
                input = FixEntities(input);
            }

            WriteLine(input);
            _shouldFixHtmlEntities = true;
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (_shouldExitNextTime)
            {
                return true;
            }

            _shouldExitNextTime = Regex.IsMatch(input, @"</code>");
            _shouldFixHtmlEntities = !_shouldExitNextTime;
            return false;
        }

        public override bool ShouldParseForNewFormatterState(string input) => false;
        public override bool ShouldFormatBlocks(string input) => false;

        private string FixEntities(string text)
        {
            // de-entify any remaining angle brackets or ampersands
            text = text.Replace("&", "&amp;");
            text = text.Replace(">", "&gt;");
            text = text.Replace("<", "&lt;");
            //Regex.Replace(text, @"\b&([#a-z0-9]+;)", "x%x%");
            return text;
        }
    }
}
