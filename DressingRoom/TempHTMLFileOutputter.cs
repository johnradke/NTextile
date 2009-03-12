#region License Statement
// Copyright (c) L.A.B.Soft.  All rights reserved.
//
// The use and distribution terms for this software are covered by the 
// Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file CPL.TXT at the root of this distribution.
// By using this software in any fashion, you are agreeing to be bound by 
// the terms of this license.
//
// You must not remove this notice, or any other, from this software.
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion


namespace DressingRoom
{
	class TempHTMLFileOutputter : Textile.IOutputter
	{
		FileInfo m_outputFile;
		TextWriter m_writer;
		
		string m_outputPath;
		public string OutputPath
		{
			get { return m_outputPath; }
			set { m_outputPath = value; }
		}

		string m_cssStyles;
		public string CssStyles
		{
			get { return m_cssStyles; }
			set { m_cssStyles = value; }
		}

		public TempHTMLFileOutputter()
		{
			m_outputPath = Path.GetTempFileName().Replace(".tmp", ".html");
			m_outputFile = new FileInfo(m_outputPath);
		}

		#region IOutputter Members

		public void Begin()
		{
			m_writer = new StreamWriter(m_outputPath);
            m_writer.NewLine = "\r\n";
			m_writer.WriteLine("<html>");
			m_writer.WriteLine("<head>");

			m_writer.WriteLine("<style type=\"text/css\">");
			m_writer.Write(CssStyles);
			m_writer.WriteLine("</style>");

			m_writer.WriteLine("</head>");
			m_writer.WriteLine("<body>");
		}

		public void Write(string text)
		{
			m_writer.Write(text);	
		}

		public void WriteLine(string line)
		{
			m_writer.WriteLine(line);
		}

		public void End()
		{
			m_writer.WriteLine("</body>");
			m_writer.WriteLine("</html>");
			m_writer.Flush();
			m_writer.Close();
		}

		#endregion
    }
}
