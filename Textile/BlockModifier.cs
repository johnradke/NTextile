using System;
using System.Collections.Generic;
using System.Text;

namespace Textile
{
    /// <summary>
    /// A class that modifies parts of a sentense ("blocks") like words in bold or italics.
    /// </summary>
    public abstract class BlockModifier
    {
        /// <summary>
        /// Gets or sets whether this block modifier is enabled.
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
		/// Creates a new instance of the block modifier.
		/// </summary>
        protected BlockModifier()
        {
            IsEnabled = true;
        }

		/// <summary>
		/// Initializes the block modifier.
		/// </summary>
		/// <param name="formatter"></param>
		public virtual void Initialize()
		{
		}

        /// <summary>
        /// Runs the block modifier on the given line, and returns the formatted line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
		public abstract string ModifyLine(string line);

        /// <summary>
        /// Gives a chance to further modify a formatted line to the block modifier.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual string Conclude(string line)
        {
            return line;
        }
	}
}
