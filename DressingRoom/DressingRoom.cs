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
using ScintillaNet;
using Textile;
#endregion


namespace DressingRoom
{
	public partial class DressingRoom : Form
	{
		Textile.TextileFormatter m_textileFormatter = null;
		Textile.StringBuilderOutputter m_textileOutput = null;
		string m_currentTextFile = null;
		HtmlElement m_bodyElement = null;
		string m_cachedHtml = null;
		string m_cachedBaseHref = null;
	    string m_searchText = null;

        internal static int Colour(byte red, byte green, byte blue)
        {
            return red | (green << 8) | (blue << 16);
        }

		public DressingRoom()
		{
			InitializeComponent();

            m_textileOutput = new Textile.StringBuilderOutputter();
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

			m_webBrowser.DocumentText = String.Format(@"<html>
	<head>
		<base id='base' />
		<style>
			{0}
		</style>
	</head>
	<body id='body'>
	</body>
</html>", styleContent);

            var ni = m_textInput.NativeInterface;
            ni.SetCaretLineBack(Colour(0xdc, 0xff, 0xff));
            ni.SetCaretLineVisible(true);
            m_textInput.Selection.ForeColor = Color.Transparent;
            m_textInput.Selection.ForeColorUnfocused = Color.Transparent;
            m_textInput.Selection.BackColor = Color.FromArgb(224, 224, 224);
            m_textInput.Selection.BackColorUnfocused = Color.FromArgb(240, 240, 240);
            ni.SetWrapMode((int)WrapMode.Word);
			SetWrapAwareHomeEndKeys();
			// disable links, since we operate on the assumption we can simply substitute the contents of <body>
			m_webBrowser.AllowNavigation = false;
			UpdateWindowTitle();
		}

		public delegate void Action<T1, T2, T3> (T1 arg1, T2 arg2, T3 arg3);

		private void SetWrapAwareHomeEndKeys ()
		{
			var commands = m_textInput.Commands;
			var setBinding = new Action<Keys, Keys, BindableCommand> ((key, mod, command) =>
				{
					commands.RemoveBinding(key, mod);
					commands.AddBinding(key, mod, command);
				}
			);
			setBinding (Keys.Home, Keys.None, BindableCommand.VCHomeWrap);
			setBinding (Keys.Home, Keys.Shift, BindableCommand.VCHomeWrapExtend);
			setBinding (Keys.Home, Keys.Shift | Keys.Alt, BindableCommand.VCHomeRectExtend);
			setBinding (Keys.End, Keys.None, BindableCommand.LineEndWrap);
			setBinding (Keys.End, Keys.Shift, BindableCommand.LineEndWrapExtend);
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
            UpdateCursorPosition();
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
                    UpdateCursorPosition();
				}
				UpdateBaseHref();
				DoTextileToHtml();
				m_textInput.Modified = false;
				UpdateWindowTitle();
			}
		}

		internal void UpdateBaseHref()
		{
			m_cachedBaseHref = new Uri(Path.GetDirectoryName(m_currentTextFile) + "/").ToString();
			var baseElement = m_webBrowser.Document.GetElementById("base");
			if (baseElement == null)
			{
				// the browser was not ready for us yet
				m_webBrowser.DocumentCompleted += DocumentCompleted_UpdateBaseHref;
			}
			else
			{
				baseElement.SetAttribute("href", m_cachedBaseHref);
			}
		}

		void DocumentCompleted_UpdateBaseHref(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var baseElement = m_webBrowser.Document.GetElementById("base");
			baseElement.SetAttribute("href", m_cachedBaseHref);
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
			this.Text = String.Format("{0} {1} The Dressing Room", fileName, modified);
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
			UpdateBaseHref();
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

        private void OnRedoClick (object sender, EventArgs e)
        {
            m_textInput.NativeInterface.Redo();
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
            System.Diagnostics.Process.Start ("http://redcloth.org/hobix.com/textile/");
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

        private void UpdateCursorPosition()
        {
            var caret = m_textInput.Caret;
            var column = m_textInput.NativeInterface.GetColumn (caret.Position);
            m_statusLabel.Text = String.Format ("li={0} co={1}", 1 + caret.LineNumber, 1 + column);
        }

        private void m_textInput_Click (object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }

        private void m_textInput_KeyUp (object sender, KeyEventArgs e)
        {
            UpdateCursorPosition();
        }

        private void m_textInput_KeyDown(object sender, KeyEventArgs e)
        {
            var lines = m_textInput.Lines;
            var currentLine = lines.Current;
            if (Keys.F2 == e.KeyCode)
            {
                if (e.Control)
                {
                    // toggle bookmark
                    if (m_textInput.Markers.GetMarkerMask(currentLine) == 0)
                    {
                        currentLine.AddMarker(0);
                    }
                    else
                    {
                        currentLine.DeleteMarker(0);
                    }
                }
                else if (e.Shift)
                {
                    // go to previous bookmark
                    var l = currentLine.FindPreviousMarker(1) ?? lines[lines.Count - 1].FindPreviousMarker(1);
                    if (l != null)
                    {
                        l.Goto();
                    }
                }
                else if (!e.Alt)
                {
                    // go to next bookmark 
                    var l = currentLine.FindNextMarker(1) ?? lines[0].FindNextMarker(1);
                    if (l != null)
                    {
                        l.Goto();
                    }
                }
            }
            else if (Keys.F3 == e.KeyCode)
            {
                if (e.Control)
                {
                    if (e.Shift)
                    {
                        // start reverse find of selected text
                        m_searchText = m_textInput.Selection.Text;
                        Select(m_textInput.FindReplace.FindPrevious(m_searchText, true));
                    }
                    else if (!e.Alt)
                    {
                        // start forward find of selected text
                        m_searchText = m_textInput.Selection.Text;
                        Select(m_textInput.FindReplace.FindNext(m_searchText, true));
                    }
                }
                else
                {
                    if (e.Shift)
                    {
                       // continue reverse find (search up) 
                        Select(m_textInput.FindReplace.FindPrevious(m_searchText, true));
                    }
                    else if (!e.Alt)
                    {
                        // continue forward find (search down)
                        Select(m_textInput.FindReplace.FindNext(m_searchText, true));
                    }
                }
            }
        }

        private void Select(Range range)
        {
            m_textInput.Selection.Range = range;
        }
	}
}