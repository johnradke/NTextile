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
    [FormatterState(TextilePatternBegin + @"fn[0-9]+" + TextilePatternEnd)]
    public class FootNoteFormatterState : SimpleBlockFormatterState
    {
        private static readonly Regex FootNoteRegex = new Regex(@"^fn(?<id>[0-9]+)");

        private int _noteID = 0;

        public override void Enter()
        {
            Write($"<p id=\"fn{_noteID}\"{FormattedStylesAndAlignment()}><sup>{_noteID}</sup> ");
        }

        public override void Exit()
        {
            WriteLine("</p>");
        }

        public override void FormatLine(string input)
        {
            Write(input);
        }

        protected override void OnContextAcquired()
        {
            Match m = FootNoteRegex.Match(Tag);
            m_noteID = Int32.Parse(m.Groups["id"].Value);
        }

        public override bool ShouldNestState(FormatterState other)
        {
            return false;
        }
    }
}
