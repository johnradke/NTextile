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
#endregion


namespace DressingRoom
{
	public partial class DressingRoom : Form
	{
		Textile.TextileFormatter m_textileFormatter = null;
		TempHTMLFileOutputter m_outputter = null;
		string m_currentTextFile = null;

		public DressingRoom()
		{
			InitializeComponent();

			m_outputter = new TempHTMLFileOutputter();
			m_textileFormatter = new Textile.TextileFormatter(m_outputter);
			
			System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
			using(Stream css = thisExe.GetManifestResourceStream("DressingRoom.Default.css"))
			using (StreamReader rdr = new StreamReader(css))
			{
				m_outputter.CssStyles = rdr.ReadToEnd();
				rdr.Close();
			}
		}

		private void OnBtnFormatClick(object sender, EventArgs e)
		{
			m_textileFormatter.Format(m_textInput.Text);
			m_webBrowser.Url = new Uri(m_outputter.OutputPath);
			m_webBrowser.Refresh();
			m_statusLabel.Text = "Textile formatted to HTML page: " + m_outputter.OutputPath;
		}

		private void OnFileNewClick(object sender, EventArgs e)
		{
			m_textInput.Text = string.Empty;
		}

		private void OnFileOpenClick(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				StreamReader rdr = new StreamReader(dlg.OpenFile());
				m_textInput.Text = rdr.ReadToEnd();
				rdr.Close();
			}
		}

		private void OnFileSaveClick(object sender, EventArgs e)
		{
			if (m_currentTextFile != null && m_currentTextFile != string.Empty)
			{
				StreamWriter wtr = new StreamWriter(m_currentTextFile);
				wtr.Write(m_textInput.Text);
				wtr.Close();
			}
			else
			{
				OnFileSaveAsClick(sender, e);
			}
		}

		private void OnFileSaveAsClick(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				StreamWriter wtr = new StreamWriter(dlg.OpenFile());
				wtr.Write(m_textInput.Text);
				wtr.Close();
			}
		}

		private void OnFileExitClick(object sender, EventArgs e)
		{
			Close();
		}

        private void OnCutClick(object sender, EventArgs e)
        {
            m_textInput.Cut();
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            m_textInput.Copy();
        }

        private void OnPasteClick(object sender, EventArgs e)
        {
            m_textInput.Paste();
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            m_textInput.SelectAll();
        }

        private void OnUndoClick(object sender, EventArgs e)
        {
            m_textInput.Undo();
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
	}
}