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

		public TableCellParser(string input)
		{
			m_lineFragment = input;
		}

		public string GetLineFragmentFormatting(bool restrictedMode)
		{
			string htmlTag = "td";

            Match m = CellHeaderRegex.Match(m_lineFragment);
			if (!m.Success)
				throw new Exception("Couldn't parse table cell.");

			if (m.Groups["head"].Value == "_")
				htmlTag = "th";
            //string opts = BlockAttributesParser.ParseBlockAttributes(m.Groups["span"].Value, "td") +
            //              BlockAttributesParser.ParseBlockAttributes(m.Groups["align"].Value, "td") +
            //              BlockAttributesParser.ParseBlockAttributes(m.Groups["atts"].Value, "td");
            string opts = Blocks.BlockAttributesParser.ParseBlockAttributes(m.Groups["span"].Value + m.Groups["align"].Value + m.Groups["atts"].Value, "td", restrictedMode);

			string res = "<" + htmlTag + opts + ">";
            // It may be possible the user actually intended to have a dot at the beginning of
            // this cell's text, without any formatting (header tag or options).
            if (string.IsNullOrEmpty(opts) && htmlTag == "td" && !string.IsNullOrEmpty(m.Groups["dot"].Value))
                res += ".";
			res += m.Groups["content"].Value;
			res += "</" + htmlTag + ">";

			return res;
		}
	}
}
