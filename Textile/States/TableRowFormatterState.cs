namespace Textile.States
{
    [FormatterState(@"^\s*(" + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern + @"\.\s?)?" + @"\|(?<content>.*)\|\s*$")]
    public class TableRowFormatterState : FormatterState
    {
        private string _attsInfo;
        private string _alignInfo;

		public override string Consume(FormatterStateConsumeContext context)
        {
            _alignInfo = context.Match.Groups["align"].Value;
            _attsInfo = context.Match.Groups["atts"].Value;
            string input = string.Format("|{0}|", context.Match.Groups["content"].Value);

            if (!(CurrentState is TableFormatterState))
            {
				ChangeState(new TableFormatterState());
            }

            ChangeState(this);

            return input;
        }

        public override bool ShouldFormatBlocks(string input)
        {
            // We'll format each cell separately.
            return false;
        }

        public override bool ShouldPostProcess(string input)
        {
            // We'll post-process each cell separately.
            return false;
        }

        public override void Enter()
        {
            WriteLine($"<tr{FormattedStylesAndAlignment()}>");
        }

        public override void Exit()
        {
            WriteLine("</tr>");
        }

        public override void FormatLine(string input)
        {
            var formattedLine = "";

            var cellsInput = input.Split('|');
            for (int i = 1; i < cellsInput.Length - 1; i++)
            {
                var cellInput = cellsInput[i];
                var tcp = new TableCellParser(cellInput, Formatter);
                formattedLine += tcp.GetLineFragmentFormatting(UseRestrictedMode);
            }

            WriteLine(formattedLine);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            return true;
        }

        protected string FormattedStylesAndAlignment()
        {
            return Blocks.BlockAttributesParser.Parse(_alignInfo + _attsInfo, UseRestrictedMode);
        }
    }
}
