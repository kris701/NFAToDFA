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
        public Dictionary<string, DFAState> States { get; internal set; }

        public DFAProcess(Dictionary<string, DFAState> states, List<string> labels)
        {
            States = states;
            Labels = labels;
        }

        public DFAProcess(string file)
        {
            Labels = new List<string>();
            States = new Dictionary<string, DFAState>();
            Read(file);
        }

        public void Read(string file)
        {
            var states = new Dictionary<string, DFAState>();
            var labels = new List<string>();

            var lines = File.ReadAllLines(file);
            foreach (var line in lines)
            {
                if (line != "")
                {
                    if (!line.Trim().StartsWith("//"))
                    {
                        var toLowerLine = line.ToLower().Trim();
                        if (toLowerLine.Contains("{") && toLowerLine.Contains("}"))
                        {
                            var labelString = toLowerLine.Replace("{", "").Replace("}", "").Replace(" ", "");
                            labels = labelString.Split(",").ToList();
                        }
                        else if (toLowerLine.Contains("[") && toLowerLine.Contains("]"))
                        {
                            var stateDefString = toLowerLine.Replace("[", "").Replace("]", "").Replace(" ", "");
                            var name = stateDefString.Split(":")[0];
                            states.Add(name, new DFAState(
                                name,
                                new List<string>(),
                                new Dictionary<string, DFAState>(),
                                stateDefString.ToUpper().Contains("ISFINAL"),
                                stateDefString.ToUpper().Contains("ISINIT")));
                        }
                        else if (toLowerLine.Contains("(") && toLowerLine.Contains(")"))
                        {
                            var transitionString = toLowerLine.Replace("(", "").Replace(")", "");
                            var transitionSteps = transitionString.Split(" ");
                            var fromState = transitionSteps[0];
                            var label = transitionSteps[1];
                            var toState = transitionSteps[2];

                            states[fromState].Transitions.Add(label, states[toState]);
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
                    outStr += $"{state.Name} ({label}) {state.Transitions[label].Name}{Environment.NewLine}";

            // Output file
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(file, outStr);
        }

        public void Validate()
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

        }
    }
}
