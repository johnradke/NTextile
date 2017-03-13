using System;
using System.Text.RegularExpressions;

namespace Textile.States
{
	public class TableCellParser
	{
        private static readonly Regex CellHeaderRegex = new Regex(@"^((?<head>_?)" +
            TextileGlobals.SpanPattern +
            TextileGlobals.AlignPattern +
            TextileGlobals.BlockModifiersPattern +
            @"(?<dot>\.)\s?)?" +
            @"(?<content>.*)");

		private string _lineFragment;
        private GenericFormatter _formatter;

		public TableCellParser(string input, GenericFormatter formatter)
		{
			_lineFragment = input;
            _formatter = formatter;
		}

		public string GetLineFragmentFormatting(bool restrictedMode)
		{
			var m = CellHeaderRegex.Match(_lineFragment);
            if (!m.Success)
            {
                throw new Exception("Couldn't parse table cell.");
            }

            var content = m.Groups["content"].Value;

            // Format/post-process the content of the cell.
            content = _formatter.ApplyBlockModifiers(content);
            content = _formatter.ApplyPostProcessors(content);

            // Figure out the HTML tag to use for the cell.
            var htmlTag = "td";
            if (m.Groups["head"].Value == "_")
            {
                htmlTag = "th";
            }

            var opts = Blocks.BlockAttributesParser.Parse(m.Groups["span"].Value + m.Groups["align"].Value + m.Groups["atts"].Value, "td", restrictedMode);

			var res = $"<{htmlTag}{opts}>";

            // It may be possible the user actually intended to have a dot at the beginning of
            // this cell's text, without any formatting (header tag or options).
            if (string.IsNullOrEmpty(opts) && htmlTag == "td" && !string.IsNullOrEmpty(m.Groups["dot"].Value))
            {
                res += ".";
            }

			res += content;
			res += $"</{htmlTag}>";

			return res;
		}
	}
}
