using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.Models
{
    public class NFAProcess
    {
        public List<string> Labels { get; internal set; }
        public Dictionary<Set, NFAState> States { get; internal set; }

        public NFAProcess(Dictionary<Set, NFAState> states, List<string> labels)
        {
            States = states;
            Labels = labels;
        }

        public NFAProcess(string file)
        {
            Labels = new List<string>();
            States = new Dictionary<Set, NFAState>();
            Read(file);
        }

        public void Read(string file)
        {
            var states = new Dictionary<Set, NFAState>();
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
                            states.Add(new Set(nameStr), new NFAState(
                                new Set(nameStr),
                                new Dictionary<string, List<NFAState>>(),
                                stateDefString.ToUpper().Contains("ISFINAL"),
                                stateDefString.ToUpper().Contains("ISINIT")));
                        }
                        else
                        {
                            var splitStr = toLowerLine.Split(" ");
                            var left = splitStr[0].Replace("(", "").Replace(")", "").Split(",");
                            string middle = splitStr[1];
                            var right = splitStr[2].Replace("(", "").Replace(")", "").Split(",");

                            if (states[new Set(left)].Transitions.ContainsKey(middle))
                                states[new Set(left)].Transitions[middle].Add(states[new Set(right)]);
                            else
                                states[new Set(left)].Transitions.Add(middle, new List<NFAState>() { states[new Set(right)] });
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
            for (int i = 0; i < Labels.Count; i++)
            {
                outStr += Labels[i];
                if (i != Labels.Count - 1)
                    outStr += ",";
            }
            outStr += $"}}{Environment.NewLine}";

            // State Declarations
            foreach (var state in States.Values)
            {
                outStr += $"[{state.Name}";
                if (state.IsFinalState)
                    outStr += ":IsFinal";
                if (state.IsInitialState)
                    outStr += ":IsInit";
                outStr += $"]{Environment.NewLine}";
            }

            // Transitions
            foreach (var state in States.Values)
                foreach (var label in state.Transitions.Keys)
                    foreach(var toState in state.Transitions[label])
                        outStr += $"{state.Name} {label} {toState.Name}{Environment.NewLine}";

            // Output file
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(file, outStr);
        }

        public bool Validate()
        {
            // Label Transition Check
            foreach (var state in States.Values)
                foreach (var label in state.Transitions.Keys)
                    if (!Labels.Contains(label))
                        throw new Exception("Transitions contain labels that was not defined!");

            // Transition Jump Check
            foreach (var state in States.Values)
                foreach (var label in state.Transitions.Keys)
                    foreach(var toState in state.Transitions[label])
                        if (!States.ContainsKey(toState.Name))
                            throw new Exception("A transition is missing a target state!");

            return true;
        }
    }
}
