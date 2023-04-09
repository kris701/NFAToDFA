using CommandLine;
using CommandLine.Text;
using NFAToDFA.Models;
using NFAToDFA.PowersetConstructors;

namespace NFAToDFA
{
    internal class Program
    {
        public class Options
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            [Option('n', "nfa", Required = true, HelpText = "Path to the input nfa file")]
            public string NFAFile { get; set; }
            [Option('d', "dfa", Required = true, HelpText = "Path to the output dfa file")]
            public string DFAFile { get; set; }

            [Usage(ApplicationAlias = "NFAToDFA")]
            public static IEnumerable<Example> Examples
            {
                get
                {
                    return new List<Example>() {
                        new Example("Convert a given NFA into a DFA", new Options { NFAFile = "file1.nfa", DFAFile = "file2.dfa" })
                      };
                }
            }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }

        static void RunOptions(Options opts)
        {
            if (!File.Exists(opts.NFAFile))
                throw new FileNotFoundException("The given NFA file was not found!");

            Console.WriteLine("Converting NFA to DFA...");
            NFAProcess nfa = new NFAProcess(opts.NFAFile);
            IPowersetConstructor constructor = new PowersetConstructor();
            DFAProcess dfa = constructor.ConstructDFA(nfa);
            dfa.Write(opts.DFAFile);
            Console.WriteLine("Done!");
            Console.WriteLine();

            Console.WriteLine("Result info:");
            Console.Write("The NFA have ");
            WriteColor($"{nfa.States.Count}", ConsoleColor.Blue);
            Console.WriteLine(" states.");
            Console.Write("Is the NFA valid?: ");
            if (nfa.Validate())
                WriteLineColor("YES", ConsoleColor.Green);
            else
                WriteLineColor("NO", ConsoleColor.Red);

            Console.Write("The DFA have ");
            WriteColor($"{dfa.States.Count}", ConsoleColor.Blue);
            Console.WriteLine(" states.");
            Console.Write("Is the DFA valid?: ");
            if (dfa.Validate())
                WriteLineColor("YES", ConsoleColor.Green);
            else
                WriteLineColor("NO", ConsoleColor.Red);

            Console.WriteLine();
            Console.WriteLine($"The resulting DFA was written to: {opts.DFAFile}");
        }

        static void WriteLineColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        static void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var error in errs)
                Console.WriteLine(error);
        }

    }
}