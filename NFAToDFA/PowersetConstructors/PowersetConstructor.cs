using NFAToDFA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.PowersetConstructors
{
    public class PowersetConstructor : IPowersetConstructor
    {
        public DFAProcess ConstructDFA(NFAProcess process)
        {
            var states = InitializeDFA(process);

            var emptyState = states.Single(x => x.Key == "ø").Value;

            // Construct transitions
            foreach(var state in states)
            {
                if (state.Key != "ø")
                {
                    if (state.Key.Contains(","))
                    {

                    }
                    else
                    {
                        var nfaState = process.States.Single(x => x.Name == state.Key);
                        foreach (var label in process.Labels)
                        {
                            if (nfaState.Transitions.Count(x => x.Label == label) == 1)
                            {
                                states[state.Key].Transitions.Add(label, states[nfaState.Transitions.Single(x => x.Label == label).State.Name]);
                            }
                            else if (nfaState.Transitions.Count(x => x.Label == label) > 1)
                            {
                                string targetName = "";
                                foreach (var targetState in nfaState.Transitions.FindAll(x => x.Label == label))
                                    targetName += $"{targetState.State.Name},";
                                states[state.Key].Transitions.Add(label, states[targetName]);
                            }
                            else
                            {
                                states[state.Key].Transitions.Add(label, emptyState);
                            }
                        }
                    }
                }
            }

            states = RemoveUncreachableStates(states);

            var dfa = new DFAProcess(states.Values.ToList(), process.Labels);

            dfa.Validate();

            return dfa;
        }

        private Dictionary<string, DFAState> InitializeDFA(NFAProcess process)
        {
            var states = new Dictionary<string, DFAState>();

            int skip = 0;
            // Construct all possible state combinations
            foreach (var state in process.States)
            {
                states.Add(
                    state.Name,
                    new DFAState(
                        state.Name,
                        new Dictionary<string, DFAState>(),
                        state.IsFinalState,
                        state.IsInitialState));
                foreach (var otherState in process.States.Skip(skip))
                {
                    if (state.Name != otherState.Name)
                    {
                        states.Add(
                            $"{state.Name},{otherState.Name},",
                            new DFAState(
                                $"{state.Name},{otherState.Name},",
                                new Dictionary<string, DFAState>(),
                                state.IsFinalState || otherState.IsFinalState));
                    }
                }
                skip++;
            }

            // Construct Total State
            string totalState = "";
            bool isFinal = false;
            foreach (var state in process.States)
            {
                totalState += $"{state.Name},";
                if (state.IsFinalState)
                    isFinal = true;
            }
            states.Add(
                totalState,
                new DFAState(
                    totalState,
                    new Dictionary<string, DFAState>(),
                    isFinal));

            // Construct Empty State
            if (!states.ContainsKey("ø"))
            {
                var emptyState = new DFAState(
                        "ø",
                        new Dictionary<string, DFAState>());
                foreach (var label in process.Labels)
                    emptyState.Transitions.Add(label, emptyState);
                states.Add(
                    "ø",
                    emptyState);
            }

            return states;
        }

        private Dictionary<string, DFAState> RemoveUncreachableStates(Dictionary<string, DFAState> states)
        {
            Dictionary<string, DFAState> reachableStates = new Dictionary<string, DFAState>();
            List<string> initialStates = new List<string>();
            foreach (var state in states)
            {
                if (state.Value.IsInitialState)
                {
                    initialStates.Add(state.Key);
                    reachableStates.Add(state.Key, state.Value);
                }
            }

            foreach(var state in states)
            {
                bool isReachable = false;
                foreach (var initialState in initialStates)
                {
                    if (state.Key != initialState)
                    {
                        if (CanReach(states, new List<string>(), initialState, state.Key))
                        {
                            isReachable = true;
                            break;
                        }
                    }
                }
                if (isReachable)
                    reachableStates.Add(state.Key, state.Value);
            }

            return reachableStates;
        }

        private bool CanReach(Dictionary<string, DFAState> states, List<string> visited, string from, string to)
        {
            if (from == to)
                return true;
            if (visited.Contains(from))
                return false;
            visited.Add(from);
            foreach (var transition in states[from].Transitions)
                if (CanReach(states, visited, transition.Value.Name, to))
                    return true;

            return false;
        }
    }
}
