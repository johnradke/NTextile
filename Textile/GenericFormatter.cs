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

        public IOutputter Output { get; }
        public int HeaderOffset { get; set; }
        public string Rel { get; set; }
        public bool UseRestrictedMode { get; set; }

        protected GenericFormatter(IOutputter output, Type defaultFormatterStateType)
        {
            Output = output;
            _defaultFormatterStateType = defaultFormatterStateType;
		}

		public void RegisterProcessorModifier<T>() where T:ProcessorModifier, new()
		{
            var modifier = new T();
            modifier.Formatter = this;
            _processorModifiers.Add(modifier);
		}

		public T GetProcessorModifier<T>() where T : ProcessorModifier
		{
			return _processorModifiers.OfType<T>().FirstOrDefault();
		}

		public bool UnregisterProcessorModifier(ProcessorModifier processorModifier)
		{
			if (_processorModifiers.Remove(processorModifier))
			{
				processorModifier.Formatter = null;
				return true;
			}
			return false;
		}

        #region Block Modifiers Management

        private List<BlockModifier> m_blockModifiers = new List<BlockModifier>();

        public void RegisterBlockModifier(BlockModifier blockModifer)
        {
            if (blockModifer == null)
                throw new ArgumentNullException("blockModifier");
            m_blockModifiers.Add(blockModifer);
			blockModifer.Formatter = this;
        }

        public void RegisterBlockModifier(Type blockModifierType)
        {
            if (blockModifierType == null)
                throw new ArgumentNullException("blockModifierType");
            if (!blockModifierType.IsSubclassOf(typeof(BlockModifier)))
                throw new ArgumentException("The given type does not inherit BlockModifier.");
            if (blockModifierType.GetConstructor(new Type[] { }) == null)
                throw new ArgumentException("The block modifier must have a parameter-less constructor.");
            RegisterBlockModifier((BlockModifier)Activator.CreateInstance(blockModifierType));
        }

        public bool UnregisterBlockModifier(BlockModifier blockModifier)
        {
			if (m_blockModifiers.Remove(blockModifier))
			{
				blockModifier.Formatter = null;
				return true;
			}
			return false;
        }

        public bool UnregisterBlockModifier(Type type)
        {
            BlockModifier blockModifier = GetBlockModifier(type);
            if (blockModifier != null)
                return UnregisterBlockModifier(blockModifier);
            return false;
        }

        public bool IsBlockModifierRegistered(BlockModifier blockModifier)
        {
            return m_blockModifiers.Contains(blockModifier);
        }

        public bool IsBlockModifierRegistered(Type type)
        {
            return GetBlockModifier(type) != null;
        }

        public bool IsBlockModifierEnabled(Type type)
        {
            BlockModifier blockModifier = GetBlockModifier(type);
            if (blockModifier == null)
                return false;
            return blockModifier.IsEnabled;
        }

        public void SwitchBlockModifier(Type type, bool onOff)
        {
            BlockModifier blockModifier = GetBlockModifier(type);
             if (blockModifier == null)
                 throw new InvalidOperationException(string.Format("No block modifier of type '{0}' was registered.", type));
             blockModifier.IsEnabled = onOff;
        }

        public BlockModifier GetBlockModifier(Type type)
        {
            return m_blockModifiers.FirstOrDefault(bm => bm.GetType() == type);
        }

        #endregion

        #region State Formatters Management


        public void RegisterFormatterState(Type formatterStateType)
        {
            if (formatterStateType == null)
                throw new ArgumentNullException("formatterStateType");
            if (!formatterStateType.IsSubclassOf(typeof(FormatterState)))
                throw new ArgumentException("The given type does not inherit FormatterStateBase.");
			if (formatterStateType.GetConstructor(new Type[] { }) == null)
                throw new ArgumentException("The formatter state must have a parameter-less constructor.");

            FormatterStateAttribute[] atts = (FormatterStateAttribute[])Attribute.GetCustomAttributes(formatterStateType, typeof(FormatterStateAttribute), false);
            if (atts.Length == 0)
                throw new ArgumentException("The formatter state must have the FormatterStateAttribute.");
            
            _formatterStates.Add(formatterStateType);
            _formatterStateAttributes.Add(atts[0]);
        }

        public bool UnregisterFormatterState(Type formatterStateType)
        {
            return _formatterStates.Remove(formatterStateType);
        }

        public bool IsFormatterStateRegistered(Type formatterStateType)
        {
            return _formatterStates.Contains(formatterStateType);
        }

        private Stack<FormatterState> m_stackOfStates = new Stack<FormatterState>();

        /// <summary>
        /// Pushes a new state on the stack.
        /// </summary>
        /// <param name="s">The state to push.</param>
        /// The state will be entered automatically.
        private void PushState(FormatterState state)
        {
			state.Formatter = this;
			m_stackOfStates.Push(state);
			state.Enter();
        }

        /// <summary>
        /// Removes the last state from the stack.
        /// </summary>
        /// The state will be exited automatically.
        private void PopState()
        {
			var state = m_stackOfStates.Pop();
			state.Exit();
			state.Formatter = null;
        }

        /// <summary>
        /// The current state, if any.
        /// </summary>
		public FormatterState CurrentState
        {
            get
            {
                if (m_stackOfStates.Count > 0)
                    return m_stackOfStates.Peek();
                else
                    return null;
            }
        }

		public void ChangeState(Type formatterStateType)
        {
            FormatterState formatterState = (FormatterState)Activator.CreateInstance(formatterStateType);
            ChangeState(formatterState);
        }

        public void ChangeState(FormatterState formatterState)
        {
            if (CurrentState != null && CurrentState.GetType() == formatterState.GetType())
            {
                if (!CurrentState.ShouldNestState(formatterState))
                    return;
            }
            PushState(formatterState);
        }

        #endregion

        #region Formatting Methods

        /// <summary>
        /// Formats the given text.
        /// </summary>
        /// <param name="input">The text to format.</param>
        public void Format(string input)
        {
            Output.Begin();

            // Clean the text...
            string str = PrepareInputForFormatting(input);
			// ...run pre-processing on it...
			foreach (ProcessorModifier modifier in _processorModifiers)
			{
                if (modifier.IsEnabled)
                {
                    str = modifier.PreProcess(str);
                }
			}
            // ...and format each line.
            string[] lines = str.Split('\n');
            for (int i = 0; i < lines.Length; ++i)
            {
                string tmp = lines[i];
                string tmpLookAhead = null;
                if (i < lines.Length - 1)
                    tmpLookAhead = lines[i + 1];

                // Let's see if the current state(s) is(are) finished...
                while (CurrentState != null && CurrentState.ShouldExit(tmp, tmpLookAhead))
                    PopState();

				if (!TextileGlobals.EmptyLineRegex.IsMatch(tmp))
                {
                    // Figure out the new state for this text line, if possible.
                    if (CurrentState == null || CurrentState.ShouldParseForNewFormatterState(tmp))
                    {
                        tmp = HandleFormattingState(tmp, tmpLookAhead);
                    }
                    // else, the current state doesn't want to be superceded by
                    // a new one. We'll leave him be.

                    // Modify the line with our block modifiers.
                    if (CurrentState == null || CurrentState.ShouldFormatBlocks(tmp))
                    {
                        tmp = ApplyBlockModifiers(tmp);
                    }

					// Post-process the line.
                    if (CurrentState == null || CurrentState.ShouldPostProcess(tmp))
                    {
                        tmp = ApplyPostProcessors(tmp);
                    }

                    // Format the current line.
                    CurrentState.FormatLine(tmp);
                }
            }
            // We're done. There might be a few states still on
            // the stack (for example if the text ends with a nested
            // list), so we must pop them all so that they have
            // their "Exit" method called correctly.
            while (m_stackOfStates.Count > 0)
                PopState();

            Output.End();
        }

        /// <summary>
        /// Applies the currently enabled block modifiers to a string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ApplyBlockModifiers(string str)
        {
            foreach (BlockModifier blockModifier in m_blockModifiers)
            {
                if (blockModifier.IsEnabled)
                {
                    str = blockModifier.ModifyLine(str);
                }
            }

            for (int j = m_blockModifiers.Count - 1; j >= 0; j--)
            {
                BlockModifier blockModifier = m_blockModifiers[j];
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

        #endregion

        #region Preparation Methods

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

        #endregion

        #region State Handling

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

        #endregion
    }
}
