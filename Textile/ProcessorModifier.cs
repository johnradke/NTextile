using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Textile
{
	public abstract class ProcessorModifier
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; private set; }

        /// <summary>
        /// 
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
		/// 
		/// </summary>
		protected ProcessorModifier()
        {
        }

		/// <summary>
		/// 
		/// </summary>
		protected ProcessorModifier(string name)
		{
			Name = name;
		}

        /// <summary>
        /// 
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
