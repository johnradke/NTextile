using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Textile
{
    public abstract class GenericFormatter
    {
        private List<ProcessorModifier> _processorModifiers = new List<ProcessorModifier>();
        private Type _defaultFormatterStateType;
        private List<Type> _formatterStates = new List<Type>();
        private List<FormatterStateAttribute> _formatterStateAttributes = new List<FormatterStateAttribute>();
        private List<BlockModifier> _blockModifiers = new List<BlockModifier>();
        private Stack<FormatterState> _states = new Stack<FormatterState>();
        private readonly FormatterStateManager _stateManager;

        public IOutputter Output { get; }
        public int HeaderOffset { get; set; }
        public string Rel { get; set; }
        public bool UseRestrictedMode { get; set; }

        protected GenericFormatter(IOutputter output, Type defaultFormatterStateType)
        {
            Output = output;
            _defaultFormatterStateType = defaultFormatterStateType;
            _stateManager = new FormatterStateManager(this);
		}

        public void RegisterBlockModifier<T>() where T:BlockModifier, new()
        {
            var modifier = new T();
            modifier.Formatter = this;
            _blockModifiers.Add(modifier);
        }
        
        public void RegisterFormatterState<T>() where T : FormatterState, new()
        {
            var attr = typeof(T).GetCustomAttributes(false).Cast<FormatterStateAttribute>().Single();

            _formatterStates.Add(typeof(T));
            _formatterStateAttributes.Add(attr);
        }

        /// <summary>
        /// Formats the given text.
        /// </summary>
        /// <param name="input">The text to format.</param>
        public void Format(string input)
        {
            Output.Begin();

            string str = PrepareInputForFormatting(input);
			foreach (ProcessorModifier modifier in _processorModifiers)
			{
                if (modifier.IsEnabled)
                {
                    str = modifier.PreProcess(str);
                }
			}

            string[] lines = str.Split('\n');
            for (int i = 0; i < lines.Length; ++i)
            {
                string tmp = lines[i];
                string tmpLookAhead = null;
                if (i < lines.Length - 1)
                    tmpLookAhead = lines[i + 1];

                while (_stateManager.ShouldExit(tmp, tmpLookAhead))
                {
                    _stateManager.PopState();
                }

				if (!TextileGlobals.EmptyLineRegex.IsMatch(tmp))
                {
                    // Figure out the new state for this text line, if possible.
                    if (_stateManager.CurrentState?.ShouldParseForNewFormatterState(tmp) ?? true)
                    {
                        tmp = HandleFormattingState(tmp, tmpLookAhead);
                    }
                    // else, the current state doesn't want to be superceded by
                    // a new one. We'll leave him be.

                    // Modify the line with our block modifiers.
                    if (_stateManager?.CurrentState.ShouldFormatBlocks(tmp) ?? true)
                    {
                        tmp = ApplyBlockModifiers(tmp);
                    }

					// Post-process the line.
                    if (_stateManager?.CurrentState.ShouldPostProcess(tmp) ?? true)
                    {
                        tmp = ApplyPostProcessors(tmp);
                    }

                    // Format the current line.
                    _stateManager.CurrentState.FormatLine(tmp);
                }
            }

            _stateManager.PopAll();
            Output.End();
        }

        /// <summary>
        /// Applies the currently enabled block modifiers to a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ApplyBlockModifiers(string str)
        {
            foreach (BlockModifier blockModifier in _blockModifiers)
            {
                if (blockModifier.IsEnabled)
                {
                    str = blockModifier.ModifyLine(str);
                }
            }

            for (int j = _blockModifiers.Count - 1; j >= 0; j--)
            {
                BlockModifier blockModifier = _blockModifiers[j];
                if (blockModifier.IsEnabled)
                {
                    str = blockModifier.Conclude(str);
                }
            }
            return str;
        }

        /// <summary>
        /// Applies the currently enabled post-processors to a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ApplyPostProcessors(string str)
        {
            foreach (ProcessorModifier modifier in _processorModifiers)
            {
                if (modifier.IsEnabled)
                {
                    str = modifier.PostProcessLine(str);
                }
            }
            return str;
        }

        /// <summary>
        /// Cleans up a text before formatting.
        /// </summary>
        /// <param name="input">The text to clean up.</param>
        /// <returns>The clean text.</returns>
        /// This method cleans stuff like line endings, so that
        /// we don't have to bother with it while formatting.
        private string PrepareInputForFormatting(string input)
        {
            input = CleanWhiteSpace(input);
            return input;
        }

        private string CleanWhiteSpace(string text)
        {
            text = text.Replace("\r\n", "\n");
			text = text.Replace("\r", "\n");
            text = text.Replace("\t", "    ");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");
            text = Regex.Replace(text, @"\n *\n", "\n\n");
            text = Regex.Replace(text, "\"$", "\" ");
            return text;
        }

        /// <summary>
        /// Gets whether the current line has a candidate formatter state, i.e. a formatter
        /// state that may be applied.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputLookAhead"></param>
        /// <returns></returns>
        public bool HasCandidateFormatterStateType(string input, string inputLookAhead)
        {
            Match m, m2;
            return GetCandidateFormatterStateType(input, inputLookAhead, out m, out m2) != null;
        }

        /// <summary>
        /// Gets the type of a candidate formatter state for the given input and look-ahead input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputLookAhead"></param>
        /// <param name="match"></param>
        /// <param name="lookAheadMatch"></param>
        /// <returns></returns>
        public Type GetCandidateFormatterStateType(string input, string inputLookAhead, out Match match, out Match lookAheadMatch)
        {
            for (int i = 0; i < _formatterStates.Count; i++)
            {
                Type type = _formatterStates[i];
                FormatterStateAttribute attribute = _formatterStateAttributes[i];

                // Match the current line.
                Match m = null;
                if (attribute.Regex != null)
                {
                    m = attribute.Regex.Match(input);
                }
                if (m == null || m.Success)
                {
                    // If the current line matches, optionally match
                    // the next line (if that formatter state needs the look-ahead
                    // line to know if it's a candidate).
                    Match m2 = null;
                    if (attribute.LookAheadRegex != null)
                    {
                        if (string.IsNullOrEmpty(inputLookAhead))   // This could be null if we're processing the last line.
                            continue;
                        m2 = attribute.LookAheadRegex.Match(inputLookAhead);
                        if (!m2.Success)
                            continue;
                    }

                    // Seems good!
                    match = m;
                    lookAheadMatch = m2;
                    return type;
                }
            }

            match = null;
            lookAheadMatch = null;
            return null;
        }

        /// <summary>
        /// Parses the string and updates the state accordingly.
        /// </summary>
        /// <param name="input">The text to process.</param>
        /// <returns>The text, ready for formatting.</returns>
        /// This method modifies the text because it removes some
        /// syntax stuff. Maybe the states themselves should handle
        /// their own syntax and remove it?
        private string HandleFormattingState(string input, string inputLookAhead)
        {
            // Find an appropriate formatter state.
            Match match;
            Match lookAheadMatch;
            Type type = GetCandidateFormatterStateType(input, inputLookAhead, out match, out lookAheadMatch);
            
            // Got it! Apply it.
            if (type != null)
            {
                FormatterState formatterState = (FormatterState)Activator.CreateInstance(type);
                formatterState.Formatter = this;
                FormatterStateConsumeContext context = new FormatterStateConsumeContext(input, inputLookAhead, match, lookAheadMatch);
                return formatterState.Consume(context);    // This may or may not change the current state.
            }

            // Default, when no block is specified, we ask the current state, or
            // use the default state.
            if (CurrentState != null)
            {
                if (CurrentState.FallbackFormattingState != null)
                {
                    ChangeState(CurrentState.FallbackFormattingState);
                }
                // else, the current state doesn't want to be superceded by
                // a new one. We'll leave him be.
            }
            else
            {
                ChangeState(_defaultFormatterStateType);
            }
            return input;
        }
    }
}
