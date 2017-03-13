using System.Text.RegularExpressions;

namespace Textile.Blocks
{
	public class StrongPhraseBlockModifier : PhraseBlockModifier
	{
        private static readonly Regex BlockRegex = new Regex(GetPhraseModifierPattern(@"\*"), TextileGlobals.BlockModifierRegexOptions);

		public override string ModifyLine(string line)
		{
			return PhraseModifierFormat(line, BlockRegex, "strong");
		}
	}
}
