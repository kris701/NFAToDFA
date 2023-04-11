using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.Models
{
    public class DFAProcess
    {
        public List<string> Labels { get; internal set; }
        public Dictionary<Set<string>, DFAState> States { get; internal set; }

        public DFAProcess(Dictionary<Set<string>, DFAState> states, List<string> labels)
        {
            States = states;
            Labels = labels;
        }

        public DFAProcess(string file)
        {
            Labels = new List<string>();
            States = new Dictionary<Set<string>, DFAState>();
            Read(file);
        }

        public void Read(string file)
        {
            var states = new Dictionary<Set<string>, DFAState>();
            var labels = new List<string>();

            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (line != "")
                {
                    if (!line.Trim().StartsWith("//"))
                    {
                        var toLowerLine = line.ToLower().Trim();
                        if (toLowerLine.StartsWith("{"))
                        {
                            var labelString = toLowerLine.Replace("{", "").Replace("}", "").Replace(" ", "");
                            labels = labelString.Split(",").ToList();
                        }
                        else if (toLowerLine.StartsWith("["))
                        {
                            var stateDefString = toLowerLine.Replace("[", "").Replace("]", "").Replace(" ", "");
                            var nameStr = stateDefString.Split(":")[0].Replace("(", "").Replace(")", "").Split(",");
                            states.Add(new Set<string>(nameStr), new DFAState(
                                new Set<string>(nameStr),
                                new Dictionary<string, DFAState>(),
                                stateDefString.ToUpper().Contains("ISFINAL"),
                                stateDefString.ToUpper().Contains("ISINIT")));
                        }
                        else if (toLowerLine.StartsWith("("))
                        {
                            var splitStr = toLowerLine.Split(" ");
                            var left = splitStr[0].Replace("(", "").Replace(")", "").Split(",");
                            string middle = splitStr[1];
                            var right = splitStr[2].Replace("(", "").Replace(")", "").Split(",");

                            states[new Set<string>(left)].Transitions.Add(middle, states[new Set<string>(right)]);
                        }
                    }
                }
            }

            Labels = labels;
            States = states;

            Validate();
        }

        public void Write(string file) 
        {
            string outStr = "";
            // Label Declaration
            outStr += "{";
            for(int i = 0; i < Labels.Count; i++)
            {
                outStr += Labels[i];
                if (i != Labels.Count - 1)
                    outStr += ",";
            }
            outStr += $"}}{Environment.NewLine}";

            // State Declarations
            foreach(var state in States.Values)
            {
                outStr += $"[{state.Name}";
                if (state.IsFinalState)
                    outStr += ":IsFinal";
                if (state.IsInitialState)
                    outStr += ":IsInit";
                outStr += $"]{Environment.NewLine}";
            }

            // Transitions
            foreach(var state in States.Values)
                foreach(var label in Labels)
                    outStr += $"{state.Name} {label} {state.Transitions[label].Name}{Environment.NewLine}";

            // Output file
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(file, outStr);
        }

        public bool Validate()
        {
            // Label Transition Check
            foreach (var state in States.Values)
                foreach (var key in state.Transitions.Keys)
                    if (!Labels.Contains(key))
                        throw new Exception("Transitions contain labels that was not defined!");

            // DFA Transition Check
            foreach (var state in States.Values)
                if (Labels.Count != state.Transitions.Keys.Count)
                    throw new Exception("All states must have all labels as transitions!");

            // Transition Jump Check
            foreach (var state in States.Values)
                foreach (var key in state.Transitions.Keys)
                    if (!States.ContainsKey(state.Transitions[key].Name))
                        throw new Exception("A transition is missing a target state!");
            return true;
        }
    }
}
