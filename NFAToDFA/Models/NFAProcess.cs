﻿using System;
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
        public Dictionary<Set<string>, NFAState> States { get; internal set; }

        public NFAProcess(Dictionary<Set<string>, NFAState> states, List<string> labels)
        {
            States = states;
            Labels = labels;
        }

        public NFAProcess(string file)
        {
            Labels = new List<string>();
            States = new Dictionary<Set<string>, NFAState>();
            Read(file);
        }

        public void Read(string file)
        {
            var states = new Dictionary<Set<string>, NFAState>();
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
                            states.Add(new Set<string>(nameStr), new NFAState(
                                new Set<string>(nameStr),
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

                            if (states[new Set<string>(left)].Transitions.ContainsKey(middle))
                                states[new Set<string>(left)].Transitions[middle].Add(states[new Set<string>(right)]);
                            else
                                states[new Set<string>(left)].Transitions.Add(middle, new List<NFAState>() { states[new Set<string>(right)] });
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
            // Output file
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(file, ToString());
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

            // Check if there is a final state
            bool finalStateFound = false;
            foreach (var state in States.Values)
            {
                if (state.IsFinalState)
                {
                    finalStateFound = true;
                    break;
                }
            }
            if (!finalStateFound)
                throw new Exception("A process must have at least one final state!");

            // Check if there is a initial state
            bool initialStateFound = false;
            foreach (var state in States.Values)
            {
                if (state.IsInitialState)
                {
                    initialStateFound = true;
                    break;
                }
            }
            if (!initialStateFound)
                throw new Exception("A process must have one initial state!");

            return true;
        }

        public override string? ToString()
        {
            string outStr = "";
            // Label Declaration
            outStr += $"// Label declaration{Environment.NewLine}";
            outStr += "{";
            for (int i = 0; i < Labels.Count; i++)
            {
                outStr += Labels[i];
                if (i != Labels.Count - 1)
                    outStr += ",";
            }
            outStr += $"}}{Environment.NewLine}";

            // State Declarations
            outStr += $"// State declaration, as well as if its a init state or a final state (or both){Environment.NewLine}";
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
            outStr += $"// Transitions{Environment.NewLine}";
            foreach (var state in States.Values)
                foreach (var label in state.Transitions.Keys)
                    foreach (var toState in state.Transitions[label])
                        outStr += $"{state.Name} {label} {toState.Name}{Environment.NewLine}";

            return outStr;
        }
    }
}
