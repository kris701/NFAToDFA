using CommandLine;
using CommandLine.Text;
using NFAToDFA.Models;
using NFAToDFA.PowersetConstructors;
using System.Runtime.InteropServices;

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
            Console.WriteLine($"The NFA had {nfa.States.Count} states, and the DFA has {dfa.States.Count} states");
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var error in errs)
                Console.WriteLine(error);
        }

    }
}