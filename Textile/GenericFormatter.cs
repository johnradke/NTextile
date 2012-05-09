using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile
{
    public abstract class GenericFormatter
    {
		public static readonly Regex EmptyLineRegex = new Regex(@"^\s*$", RegexOptions.CultureInvariant);

        /// <summary>
        /// Public constructor, where the formatter is hooked up
        /// to an outputter.
        /// </summary>
        /// <param name="output">The outputter to be used.</param>
        protected GenericFormatter(IOutputter output, Type defaultFormatterStateType)
        {
            m_output = output;
            m_defaultFormatterStateType = defaultFormatterStateType;
		}

		#region Properties for Output

		private IOutputter m_output = null;
        /// <summary>
        /// The ouputter to which the formatted text
        /// is sent to.
        /// </summary>
        public IOutputter Output
        {
            get { return m_output; }
		}

		private int m_headerOffset = 0;
		/// <summary>
		/// The offset for the header tags.
		/// </summary>
		/// When the formatted text is inserted into another page
		/// there might be a need to offset all headers (h1 becomes
		/// h4, for instance). The header offset allows this.
		public int HeaderOffset
		{
			get { return m_headerOffset; }
			set { m_headerOffset = value; }
		}

        string m_rel = string.Empty;
		/// <summary>
		/// Attribute to add to all links.
		/// </summary>
        public string Rel
        {
            get { return m_rel; }
            set { m_rel = value; }
        }

        private bool mUseRestrictedMode = false;
        /// <summary>
        /// Gets or sets the 'restricted' mode where formatting features are limited.
        /// </summary>
        public bool UseRestrictedMode
        {
            get { return mUseRestrictedMode; }
            set { mUseRestrictedMode = true; }
        }

        #endregion

		#region Processor Modifiers Management

		private List<ProcessorModifier> m_processorModifiers = new List<ProcessorModifier>();

		public void RegisterProcessorModifier(ProcessorModifier processorModifier)
		{
			if (processorModifier == null)
				throw new ArgumentNullException("preProcessorModifier");
			m_processorModifiers.Add(processorModifier);
			processorModifier.Formatter = this;
		}

		public void RegisterProcessorModifier(Type processorModifierType)
		{
			if (processorModifierType == null)
				throw new ArgumentNullException("preProcessorModifierType");
			if (!processorModifierType.IsSubclassOf(typeof(ProcessorModifier)))
				throw new ArgumentException("The given type does not inherit PreProcessorModifier.");
			if (processorModifierType.GetConstructor(new Type[] { }) == null)
				throw new ArgumentException("The pre-processor modifier must have a parameter-less constructor.");
			RegisterProcessorModifier((ProcessorModifier)Activator.CreateInstance(processorModifierType));
		}

		public ProcessorModifier GetProcessorModifier(string name)
		{
			return m_processorModifiers.FirstOrDefault(m => m.Name == name);
		}

		public T GetProcessorModifier<T>() where T : ProcessorModifier
		{
			return m_processorModifiers.OfType<T>().FirstOrDefault();
		}

		public bool UnregisterProcessorModifier(ProcessorModifier processorModifier)
		{
			if (m_processorModifiers.Remove(processorModifier))
			{
				processorModifier.Formatter = null;
				return true;
			}
			return false;
		}

		#endregion

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

        private Type m_defaultFormatterStateType;
        private List<Type> m_formatterStates = new List<Type>();
        private List<FormatterStateAttribute> m_formatterStateAttributes = new List<FormatterStateAttribute>();

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
            
            m_formatterStates.Add(formatterStateType);
            m_formatterStateAttributes.Add(atts[0]);
        }

        public bool UnregisterFormatterState(Type formatterStateType)
        {
            return m_formatterStates.Remove(formatterStateType);
        }

        public bool IsFormatterStateRegistered(Type formatterStateType)
        {
            return m_formatterStates.Contains(formatterStateType);
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
            m_output.Begin();

			// Initialize stuff.
			foreach (BlockModifier blockModifier in m_blockModifiers)
			{
                if (blockModifier.IsEnabled)
                {
                    blockModifier.Initialize();
                }
			}

            // Clean the text...
            string str = PrepareInputForFormatting(input);
			// ...run pre-processing on it...
			foreach (ProcessorModifier modifier in m_processorModifiers)
			{
				str = modifier.PreProcess(str);
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

				if (!EmptyLineRegex.IsMatch(tmp))
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
                        foreach (BlockModifier blockModifier in m_blockModifiers)
                        {
                            if (blockModifier.IsEnabled)
                            {
                                tmp = blockModifier.ModifyLine(tmp);
                            }
                        }

                        for (int j = m_blockModifiers.Count - 1; j >= 0; j--)
                        {
                            BlockModifier blockModifier = m_blockModifiers[j];
                            if (blockModifier.IsEnabled)
                            {
                                tmp = blockModifier.Conclude(tmp);
                            }
                        }
                    }

					// Post-process the line.
					foreach (ProcessorModifier modifier in m_processorModifiers)
					{
						tmp = modifier.PostProcessLine(tmp);
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

            m_output.End();
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
        /// Parses the string and updates the state accordingly.
        /// </summary>
        /// <param name="input">The text to process.</param>
        /// <returns>The text, ready for formatting.</returns>
        /// This method modifies the text because it removes some
        /// syntax stuff. Maybe the states themselves should handle
        /// their own syntax and remove it?
        private string HandleFormattingState(string input, string inputLookAhead)
        {
            for (int i = 0; i < m_formatterStates.Count; i++)
            {
                Type type = m_formatterStates[i];
                FormatterStateAttribute attribute = m_formatterStateAttributes[i];
				Match m = null;
				if (attribute.Regex != null)
				{
					m = attribute.Regex.Match(input);
				}
                if (m == null || m.Success)
                {
                    Match m2 = null;
					if (attribute.LookAheadRegex != null)
                    {
                        if (string.IsNullOrEmpty(inputLookAhead))   // This could be null if we're processing the last line.
                            continue;
						m2 = attribute.LookAheadRegex.Match(inputLookAhead);
                        if (!m2.Success)
                            continue;
                    }
                    FormatterState formatterState = (FormatterState)Activator.CreateInstance(type);
					formatterState.Formatter = this;
					FormatterStateConsumeContext context = new FormatterStateConsumeContext(input, inputLookAhead, m, m2);
                    return formatterState.Consume(context);    // This may or may not change the current state.
                }
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
                ChangeState(m_defaultFormatterStateType);
            }
            return input;
        }

        #endregion
    }
}
