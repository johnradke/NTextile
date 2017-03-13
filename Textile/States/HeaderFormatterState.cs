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
    /// Formatting state for headers and titles.
    /// </summary>
    [FormatterState(TextilePatternBegin + @"h[0-9]+" + TextilePatternEnd)]
    public class HeaderFormatterState : SimpleBlockFormatterState
    {
        private static readonly Regex HeaderRegex = new Regex(@"^h(?<lvl>[0-9]+)");

        private int _headerLevel;

        public override void Enter()
        {
            Write($"<h{_headerLevel}{FormattedStylesAndAlignment()}>");
        }

        public override void Exit()
        {
            WriteLine($"</h{_headerLevel}>");
        }

        protected override void OnContextAcquired()
        {
            var m = HeaderRegex.Match(Tag);
            _headerLevel = int.Parse(m.Groups["lvl"].Value);
        }

        public override void FormatLine(string input)
        {
            Write(input);
        }

        public override bool ShouldExit(string intput, string inputLookAhead) => true;

        public override bool ShouldNestState(FormatterState other) => false;
    }
}
