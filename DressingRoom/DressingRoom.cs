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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Textile;
#endregion


namespace DressingRoom
{
	public partial class DressingRoom : Form
	{
		Textile.TextileFormatter m_textileFormatter = null;
		StringBuilderTextileFormatter m_textileOutput = null;
		string m_currentTextFile = null;
		HtmlElement m_bodyElement = null;
		string m_cachedHtml = null;

		public DressingRoom()
		{
			InitializeComponent();

			m_textileOutput = new StringBuilderTextileFormatter();
			m_textileFormatter = new Textile.TextileFormatter(m_textileOutput);

			System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
		    var absolutePathToCustomCss = Path.ChangeExtension(Application.ExecutablePath, ".css");
		    Stream css;
		    if (File.Exists(absolutePathToCustomCss))
		    {
		        css = new FileStream(absolutePathToCustomCss, FileMode.Open, FileAccess.Read, FileShare.Read);
		    }
		    else
		    {
		        css = thisExe.GetManifestResourceStream("DressingRoom.Default.css");
		    }
            string styleContent;
            using (css)
			{
				using (StreamReader rdr = new StreamReader(css))
				{
					styleContent = rdr.ReadToEnd();
				}
			}

			// TODO: it may eventually be useful to allow users to specify their own stylesheet/CSS
			m_webBrowser.DocumentText = String.Format(@"<html>
	<head>
		<style>
			{0}
		</style>
	</head>
	<body id='body'>
	</body>
</html>", styleContent);

			// disable links, since we operate on the assumption we can simply substitute the contents of <body>
			m_webBrowser.AllowNavigation = false;
			UpdateWindowTitle();
		}

		public DressingRoom(string path)
			: this()
		{
			DoOpen(path);
		}

		private void OnBtnFormatClick(object sender, EventArgs e)
		{
			DoTextileToHtml();
		}

		private void DoTextileToHtml()
		{
			m_textileFormatter.Format(m_textInput.Text);
			m_cachedHtml = m_textileOutput.GetFormattedText();
			if (m_bodyElement == null)
			{
				m_bodyElement = m_webBrowser.Document.GetElementById("body");
				if (m_bodyElement == null)
				{
					// the browser was not ready for us yet
					m_webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(m_webBrowser_DocumentCompleted);
				}
				else
				{
					m_bodyElement.InnerHtml = m_cachedHtml;
				}
			}
			else
			{
				m_bodyElement.InnerHtml = m_cachedHtml;
			}
		}

		void m_webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			m_bodyElement = m_webBrowser.Document.GetElementById("body");
			m_bodyElement.InnerHtml = m_cachedHtml;
		}

		private void OnFileNewClick(object sender, EventArgs e)
		{
			m_textInput.Text = string.Empty;
			DoTextileToHtml();
			m_textInput.Modified = false;
			UpdateWindowTitle();
		}

		private void OnFileOpenClick(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				DoOpen(dlg.FileName);
			}
		}

		public void DoOpen(string path)
		{
			if (!String.IsNullOrEmpty(path))
			{
				m_currentTextFile = path;
				using (StreamReader rdr = File.OpenText(m_currentTextFile))
				{
					m_textInput.Text = rdr.ReadToEnd();
				}
				DoTextileToHtml();
				m_textInput.Modified = false;
				UpdateWindowTitle();
			}
		}

		private void OnFileSaveClick(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(m_currentTextFile))
			{
				DoSave(m_currentTextFile);
			}
			else
			{
				DoSaveAs();
			}
		}

		private void UpdateWindowTitle()
		{
			string fileName = FormatPathNicely(m_currentTextFile);
			char modified = m_textInput.Modified ? '*' : '-';
			this.Text = String.Format("{0} {1} The Dressing Room - A Textile Test Tool", fileName, modified);
		}

		internal static string FormatPathNicely(string path)
		{
			if (String.IsNullOrEmpty(path))
			{
				return "(Untitled)";
			}
			string filename = Path.GetFileName(path);
			string folderName = Path.GetDirectoryName(path);
			string result = String.Format("{0} in {1}", filename, folderName);
			return result;
		}

		private void DoSave(string path)
		{
			m_currentTextFile = path;
			DoTextileToHtml();
			using (StreamWriter wtr = new StreamWriter(m_currentTextFile))
			{
				wtr.Write(m_textInput.Text);
			}
			m_textInput.Modified = false;
			UpdateWindowTitle();
		}

		private void OnFileSaveAsClick(object sender, EventArgs e)
		{
			DoSaveAs();
		}

		private void DoSaveAs()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				DoSave(dlg.FileName);
			}
		}

		private void OnFileExitClick(object sender, EventArgs e)
		{
			Close();
		}

        private void OnCutClick(object sender, EventArgs e)
        {
            m_textInput.NativeInterface.Cut();
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            m_textInput.NativeInterface.Copy();
        }

        private void OnPasteClick(object sender, EventArgs e)
        {
            m_textInput.NativeInterface.Paste();
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            m_textInput.NativeInterface.SelectAll();
        }

        private void OnUndoClick(object sender, EventArgs e)
        {
            m_textInput.NativeInterface.Undo();
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            AboutDressingRoom dlg = new AboutDressingRoom();
            dlg.ShowDialog(this);
        }

        private void OnNoImagesClick(object sender, EventArgs e)
        {
            this.noImagesToolStripMenuItem.Checked = !this.noImagesToolStripMenuItem.Checked;
            m_textileFormatter.FormatImages = !this.noImagesToolStripMenuItem.Checked;
        }

        private void OnNoLinksClick(object sender, EventArgs e)
        {
            this.noLinksToolStripMenuItem.Checked = !this.noLinksToolStripMenuItem.Checked;
            m_textileFormatter.FormatLinks = !this.noLinksToolStripMenuItem.Checked;
        }

        private void OnNoListsClick(object sender, EventArgs e)
        {
            this.noListsToolStripMenuItem.Checked = !this.noListsToolStripMenuItem.Checked;
            m_textileFormatter.FormatLists = !this.noListsToolStripMenuItem.Checked;
        }

        private void OnNoTablesClick(object sender, EventArgs e)
        {
            this.noTablesToolStripMenuItem.Checked = !this.noTablesToolStripMenuItem.Checked;
            m_textileFormatter.FormatTables = !this.noTablesToolStripMenuItem.Checked;
        }

        private void OnNoFootnotesClick(object sender, EventArgs e)
        {
            this.noFootnotesToolStripMenuItem.Checked = !this.noFootnotesToolStripMenuItem.Checked;
            m_textileFormatter.FormatFootNotes = !this.noFootnotesToolStripMenuItem.Checked;
        }

        private void textileReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://hobix.com/textile/");
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoTextileToHtml();
        }

        private void m_textInput_ModifiedChanged(object sender, EventArgs e)
        {
            UpdateWindowTitle();
        }

        private void DressingRoom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_textInput.Modified)
            {
                // TODO: show a suitable confirmation dialog before auto-saving
                if (!String.IsNullOrEmpty(m_currentTextFile))
                {
                    DoSave(m_currentTextFile);
                }
                else
                {
                    DoSaveAs();
                }
            }
        }
	}
}