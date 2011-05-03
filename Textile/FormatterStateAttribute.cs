using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Textile
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FormatterStateAttribute : Attribute
    {
        private Regex m_regex;
        public Regex Regex
        {
            get { return m_regex; }
            set { m_regex = value; }
        }

        private Regex m_lookAheadRegex;
        public Regex LookAheadRegex
        {
            get { return m_lookAheadRegex; }
            set { m_lookAheadRegex = value; }
        }

        public FormatterStateAttribute()
        {
        }

        public FormatterStateAttribute(string pattern)
        {
            m_regex = new Regex(pattern);
        }

        public FormatterStateAttribute(string pattern, string lookAheadPattern)
        {
            m_regex = new Regex(pattern);
            m_lookAheadRegex = new Regex(lookAheadPattern);
        }

        public static FormatterStateAttribute Get(Type type)
        {
            return (FormatterStateAttribute)Attribute.GetCustomAttribute(type, typeof(FormatterStateAttribute), false);
        }
    }
}
