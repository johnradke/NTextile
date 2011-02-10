using System;
using System.IO;
using System.Windows.Forms;

namespace DressingRoom
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			string pathToOpen = args.Length > 0 ? Path.GetFullPath(args[0]) : String.Empty;
			DressingRoom form = new DressingRoom();
			form.Show();
			form.DoOpen(pathToOpen);
			Application.Run(form);
		}
	}
}