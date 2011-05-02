using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NUnit.ConsoleRunner;

namespace Textile.UnitTests
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
			var stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();

            Assembly assy = Assembly.GetExecutingAssembly();
            Runner.Main(new string[] { assy.Location });

			stopwatch.Stop();
			Console.WriteLine();
			Console.WriteLine("Ran unit-tests in {0} seconds", stopwatch.Elapsed.TotalSeconds);

			if (System.Diagnostics.Debugger.IsAttached)
			{
				Console.WriteLine("Press any key to continue...");
				Console.In.ReadLine();
			}
        }
    }
}
