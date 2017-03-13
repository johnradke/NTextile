#region License Statement
// Copyright (c) L.A.B.Soft.  All rights reserved.
//
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.
#endregion

using System.Text.RegularExpressions;

namespace Textile.States
{
	/// <summary>
	/// Formatting state for a bulleted list.
	/// </summary>
    [FormatterState(PatternBegin + @"\*+" + PatternEnd)]
	public class UnorderedListFormatterState : ListFormatterState
	{
		protected override void WriteIndent()
		{
			WriteLine($"<ul{FormattedStylesAndAlignment()}>");
		}

        protected override void WriteOutdent()
		{
			WriteLine("</ul>");
		}

        protected override bool IsMatchForMe(string input, int minNestingDepth, int maxNestingDepth)
        {
            return Regex.IsMatch(input, @"^\s*[\*]{" + minNestingDepth + @"," + maxNestingDepth + @"}" + TextileGlobals.BlockModifiersPattern + @"\s");
        }

        protected override bool IsMatchForOthers(string input, int minNestingDepth, int maxNestingDepth)
        {
            return Regex.IsMatch(input, @"^\s*[#]{" + minNestingDepth + @"," + maxNestingDepth + @"}" + TextileGlobals.BlockModifiersPattern + @"\s");
        }
    }
}
