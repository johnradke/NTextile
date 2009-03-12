using System;
using System.Collections.Generic;
using System.Text;

namespace Textile.UnitTests
{
    class TestOutputter : Textile.IOutputter
    {
        StringBuilder m_builder;

        #region IOutputter Members

        public void Begin()
        {
            m_builder = new StringBuilder();
        }

        public void Write(string text)
        {
            m_builder.Append(text);
        }

        public void WriteLine(string line)
        {
            m_builder.Append(line + "\n");
        }

        public void End()
        {
        }

        #endregion

        public string GetOutput()
        {
            return m_builder.ToString();
        }
    }
}
