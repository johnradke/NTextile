using System;
using System.Collections.Generic;
using System.Text;
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

		private string m_lineFragment;
        private GenericFormatter m_formatter;

		public TableCellParser(string input, GenericFormatter formatter)
		{
			m_lineFragment = input;
            m_formatter = formatter;
		}

		public string GetLineFragmentFormatting(bool restrictedMode)
		{
			Match m = CellHeaderRegex.Match(m_lineFragment);
			if (!m.Success)
				throw new Exception("Couldn't parse table cell.");

            string content = m.Groups["content"].Value;
            // Format/post-process the content of the cell.
            content = m_formatter.ApplyBlockModifiers(content);
            content = m_formatter.ApplyPostProcessors(content);

            // Figure out the HTML tag to use for the cell.
            string htmlTag = "td";
			if (m.Groups["head"].Value == "_")
				htmlTag = "th";

            string opts = Blocks.BlockAttributesParser.ParseBlockAttributes(m.Groups["span"].Value + m.Groups["align"].Value + m.Groups["atts"].Value, "td", restrictedMode);

			string res = "<" + htmlTag + opts + ">";
            // It may be possible the user actually intended to have a dot at the beginning of
            // this cell's text, without any formatting (header tag or options).
            if (string.IsNullOrEmpty(opts) && htmlTag == "td" && !string.IsNullOrEmpty(m.Groups["dot"].Value))
                res += ".";
			res += content;
			res += "</" + htmlTag + ">";

			return res;
		}
	}
}
