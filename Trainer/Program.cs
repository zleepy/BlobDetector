using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Options;

namespace Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            Arguments a = ParseArguments(args);

            if (a == null || a.Help)
            {
                Console.ReadLine();
                return;
            }

            if (string.IsNullOrEmpty(a.Root))
            {
                a.Root = System.Environment.CurrentDirectory;
            }

            if (!System.IO.Directory.Exists(a.Root))
            {
                Console.WriteLine("Katalogen till träningsfilerna finns inte.");
                Console.WriteLine("Root='{0}'", a.Root);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Root='{0}'", a.Root);
            Console.WriteLine("Verbose='{0}'", a.Verbose ? "sant" : "falskt");

            var trainer = new Trainer();
            trainer.ReportProgress += (s, arg) => Console.WriteLine(arg.Message);
            trainer.Run(a);

            Console.ReadLine();
        }


        private static Arguments ParseArguments(string[] args)
        {
            var result = new Arguments();

            var p = new OptionSet()
            {
                { "?|h|help", "visar det här meddelandet", v => result.Help = (v != null) },
                { "v|verbose", "visar extra information under körningen", v => result.Verbose = (v != null) },
                //{ "s|swi|save-work-images", "visar extra information under körningen", v => result.SaveWorkImagesToDisk = (v != null) },
                { "r|root=", "sökväg till den katalog som innehåller träningsdatan, anges ingen säkväg så används programmets körkatalog", v => result.Root = v }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("trainer: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Pröva 'trainer --help' för mer information.");
                return null;
            }

            if (result.Help)
                ShowHelp(p);

            return result;
        }


        private static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Användning: trainer [ARGUMENT]+ message");
            Console.WriteLine("Bygger en teckendatabas utifrån träningsfiler.");
            Console.WriteLine();
            Console.WriteLine("Argument:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
