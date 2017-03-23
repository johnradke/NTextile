namespace Textile
{
    /// <summary>
    /// A class that implement a text modifier that can be applied
    /// as a pre- or post-process.
    /// </summary>
	public abstract class ProcessorModifier
	{
        /// <summary>
        /// Gets or sets whether the modifier is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

		private GenericFormatter m_formatter;
		/// <summary>
		/// Gets the formatter this state belongs to.
		/// </summary>
		public GenericFormatter Formatter
		{
			get { return m_formatter; }
			internal set { m_formatter = value; }
		}

		/// <summary>
		/// Constructs a new instance of <see cref="ProcessorModifier"/>.
		/// </summary>
		protected ProcessorModifier()
        {
        }

        /// <summary>
        /// Pre-processing the entire input text.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public abstract string PreProcess(string input);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public virtual string PostProcessLine(string line)
		{
			return line;
		}
	}
}
