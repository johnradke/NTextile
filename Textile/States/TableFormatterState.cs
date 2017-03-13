using System;
using System.Text.RegularExpressions;
using Textile.Blocks;

namespace Textile.States
{
    [FormatterState(@"^\s*(?<tag>table)" + TextileGlobals.SpanPattern + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern + @"\.\s*$")]
	public class TableFormatterState : FormatterState
	{
        private static readonly Regex NextLineRegex = new Regex(@"^\s*" +
            TextileGlobals.AlignPattern +
            TextileGlobals.BlockModifiersPattern +
            @"(\.\s?)?(?<tag>\|)" +
            @"(?<content>.*)(?=\|)");

        private string _attsInfo;
        private string _alignInfo;

		public override string Consume(FormatterStateConsumeContext context)
        {
            _alignInfo = context.Match.Groups["align"].Value;
            _attsInfo = context.Match.Groups["atts"].Value;

            ChangeState(this);

            return string.Empty;
        }

		public override void Enter()
		{
            WriteLine($"<table{FormattedStylesAndAlignment()}>");
		}

		public override void Exit()
		{
			WriteLine("</table>");
		}

		public override void FormatLine(string input)
		{
            if (input.Length > 0)
            {
                throw new Exception("The TableFormatter state is not supposed to format any lines!");
            }
		}

		public override bool ShouldExit(string input, string inputLookAhead)
		{
            Match m = NextLineRegex.Match(input);
			return !m.Success;
		}

        protected string FormattedStylesAndAlignment()
        {
            return BlockAttributesParser.Parse(_alignInfo + _attsInfo, UseRestrictedMode);
        }
	}
}
