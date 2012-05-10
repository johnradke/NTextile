using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile.States
{
    public abstract class SimpleBlockFormatterState : FormatterState
    {
        internal const string TextilePatternBegin = @"^\s*(?<tag>";
        internal const string TextilePatternEnd = @")" + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern + @"\.(?:\s+)?(?<content>.*)$";
        internal const string TextileDoubleDotPatternEnd = @")" + TextileGlobals.AlignPattern + TextileGlobals.BlockModifiersPattern + @"\.(?<ddot>\.)?(?:\s+)?(?<content>.*)$";

        private string m_tag = null;
        public string Tag
        {
            get { return m_tag; }
        }

        private string m_alignNfo = null;
        public string AlignInfo
        {
            get { return m_alignNfo; }
        }

        private string m_attNfo = null;
        public string AttInfo
        {
            get { return m_attNfo; }
        }

        protected SimpleBlockFormatterState()
        {
        }

		public override string Consume(FormatterStateConsumeContext context)
        {
            m_tag = context.Match.Groups["tag"].Value;
            m_alignNfo = context.Match.Groups["align"].Value;
            m_attNfo = context.Match.Groups["atts"].Value;
            string input = context.Match.Groups["content"].Value;

            OnContextAcquired();

            this.Formatter.ChangeState(this);

            return input;
        }

        public override bool ShouldNestState(FormatterState other)
        {
            SimpleBlockFormatterState blockFormatterState = (SimpleBlockFormatterState)other;
            return (blockFormatterState.m_tag != m_tag ||
                    blockFormatterState.m_alignNfo != m_alignNfo ||
                    blockFormatterState.m_attNfo != m_attNfo);
        }

        protected virtual void OnContextAcquired()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string FormattedAlignment()
        {
            return Blocks.BlockAttributesParser.ParseBlockAttributes(m_alignNfo, UseRestrictedMode);
        }

        protected string FormattedStyles()
        {
            return Blocks.BlockAttributesParser.ParseBlockAttributes(m_attNfo, UseRestrictedMode);
        }

        protected string FormattedStylesAndAlignment()
        {
            return Blocks.BlockAttributesParser.ParseBlockAttributes(m_alignNfo + m_attNfo, UseRestrictedMode);
        }
    }
}
