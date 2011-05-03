using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.States
{
    [FormatterState(@"^\s*(" + 
                    TextileGlobals.AlignPattern + 
                    TextileGlobals.BlockModifiersPattern + 
                    @"\.\s?)?" +
                    @"\|(?<content>.*)\|\s*$")]
    public class TableRowFormatterState : FormatterState
    {
        private string m_attsInfo;
        private string m_alignInfo;

        public TableRowFormatterState()
        {
        }

		public override string Consume(FormatterStateConsumeContext context)
        {
            m_alignInfo = context.Match.Groups["align"].Value;
            m_attsInfo = context.Match.Groups["atts"].Value;
            string input = string.Format("|{0}|", context.Match.Groups["content"].Value);

            if (!(this.Formatter.CurrentState is TableFormatterState))
            {
				this.Formatter.ChangeState(new TableFormatterState());
            }

            Formatter.ChangeState(this);

            return input;
        }

        public override bool ShouldNestState(FormatterState other)
        {
            return false;
        }

        public override void Enter()
        {
            Formatter.Output.WriteLine("<tr" + FormattedStylesAndAlignment() + ">");
        }

        public override void Exit()
        {
            Formatter.Output.WriteLine("</tr>");
        }

        public override void FormatLine(string input)
        {
            // can get: Align & Classes

            string formattedLine = "";

            string[] cellsInput = input.Split('|');
            for (int i = 1; i < cellsInput.Length - 1; i++)
            {
                string cellInput = cellsInput[i];
                TableCellParser tcp = new TableCellParser(cellInput);
                formattedLine += tcp.GetLineFragmentFormatting();
            }

            Formatter.Output.WriteLine(formattedLine);
        }

		public override bool ShouldExit(string input, string inputLookAhead)
        {
            return true;
        }

        protected string FormattedStylesAndAlignment()
        {
            return Blocks.BlockAttributesParser.ParseBlockAttributes(m_alignInfo + m_attsInfo);
        }
    }
}
