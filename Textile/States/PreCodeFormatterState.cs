using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(TextilePatternBegin + @"bc" + TextilePatternEnd)]
    public class PreCodeFormatterState : SimpleBlockFormatterState
    {
        public override void Enter()
        {
            Write("<pre><code>");
        }

        public override void Exit()
        {
            WriteLine("</code></pre>");
        }

        public override void FormatLine(string input)
        {
            WriteLine(FixEntities(input));
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            if (Regex.IsMatch(input, @"^\s*$"))
            {
                return true;
            }

            WriteLine("<br />");

            return false;
        }

        public override bool ShouldFormatBlocks(string input) => false;
        public override bool ShouldNestState(FormatterState other) => false;
        public override bool ShouldParseForNewFormatterState(string input) => false;

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
