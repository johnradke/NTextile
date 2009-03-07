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
            ConsoleUi runner = new ConsoleUi();
            Assembly assy = Assembly.GetExecutingAssembly();
            ConsoleOptions options = new ConsoleOptions(new string[] { assy.Location });
            runner.Execute(options);

            Console.WriteLine("Press any key to continue...");
            Console.In.ReadLine();
        }
    }
}
