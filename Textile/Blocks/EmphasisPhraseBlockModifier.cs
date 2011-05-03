using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.Blocks
{
	public class EmphasisPhraseBlockModifier : PhraseBlockModifier
	{
        private static readonly Regex BlockRegex = new Regex(PhraseBlockModifier.GetPhraseModifierPattern("_"), TextileGlobals.BlockModifierRegexOptions);

		public override string ModifyLine(string line)
		{
			return PhraseModifierFormat(line, BlockRegex, "em");
		}
	}
}
