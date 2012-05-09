using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Textile.States
{
    [FormatterState(@"^\s*(?<tag>table)" +
                    TextileGlobals.SpanPattern +
                    TextileGlobals.AlignPattern +
                    TextileGlobals.BlockModifiersPattern +
                    @"\.\s*$")]
	public class TableFormatterState : FormatterState
	{
        private static readonly Regex NextLineRegex = new Regex(@"^\s*" + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern +
                                                                   @"(\.\s?)?(?<tag>\|)" +
                                                                   @"(?<content>.*)(?=\|)");

        private string m_attsInfo;
        private string m_alignInfo;

		public TableFormatterState()
		{
		}

		public override string Consume(FormatterStateConsumeContext context)
        {
            m_alignInfo = context.Match.Groups["align"].Value;
            m_attsInfo = context.Match.Groups["atts"].Value;

            //TODO: check the state (it could already be a table!)
            Formatter.ChangeState(this);

            return string.Empty;
        }

        public override bool ShouldNestState(FormatterState other)
		{
			return false;
		}

		public override void Enter()
		{
            Formatter.Output.WriteLine("<table" + FormattedStylesAndAlignment() + ">");
		}

		public override void Exit()
		{
			Formatter.Output.WriteLine("</table>");
		}

		public override void FormatLine(string input)
		{
			if (input.Length > 0)
				throw new Exception("The TableFormatter state is not supposed to format any lines!");
		}

		public override bool ShouldExit(string input, string inputLookAhead)
		{
            Match m = NextLineRegex.Match(input);
			return( m.Success == false );
		}

        protected string FormattedStylesAndAlignment()
        {
            return Blocks.BlockAttributesParser.ParseBlockAttributes(m_alignInfo + m_attsInfo, UseRestrictedMode);
        }
	}
}
