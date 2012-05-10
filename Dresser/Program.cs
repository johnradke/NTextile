using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Dresser
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return 2;
            }

            string[] parameters = args.Where(a => a.StartsWith("--") == false).ToArray();
            string[] options = args.Where(a => a.StartsWith("--") == true).ToArray();
            try
            {
                string input = parameters[0];
                if (!File.Exists(input))
                    throw new FileNotFoundException("Can't open input file: " + input);

                var outputter = new Textile.StringBuilderOutputter();
                var formatter = new Textile.TextileFormatter(outputter);
                if (options.Contains("--restricted"))
                    formatter.UseRestrictedMode = true;

                formatter.Format(File.ReadAllText(input));
                string output = outputter.GetFormattedText();

                if (parameters.Length >= 2)
                {
                    File.WriteAllText(parameters[1], output);
                }
                else
                {
                    Console.WriteLine(output);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Dresser:");
            Console.WriteLine("");
            Console.WriteLine("  dresser.exe <options> [input] <[output]>");
            Console.WriteLine("");
            Console.WriteLine("    Options:");
            Console.WriteLine("    --restricted: Use the 'restricted' mode.");
            Console.WriteLine("");
            Console.WriteLine("    [input]:  The path to an input text file.");
            Console.WriteLine("    [output]: The path to an output text file.");
            Console.WriteLine("              If unspecified, the output will be printed to the console.");
            Console.WriteLine("");
        }
    }
}
