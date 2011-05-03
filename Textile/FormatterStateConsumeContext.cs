using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile
{
	public class FormatterStateConsumeContext
	{
		public string Input { get; private set; }
		public string LookAheadInput { get; private set; }
		public Match Match { get; private set; }
		public Match LookAheadMatch { get; private set; }

		public FormatterStateConsumeContext(string input, string lookAheadInput, Match match, Match lookAheadMatch)
		{
			Input = input;
			LookAheadInput = lookAheadInput;
			Match = match;
			LookAheadMatch = lookAheadMatch;
		}
	}
}
