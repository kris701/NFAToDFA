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

            var emptyState = states[new Set("ø")];

            // Construct transitions
            foreach(var state in states)
            {
                if (state.Key != emptyState.Name)
                {
                    if (state.Key.Count > 1)
                    {
                        // Handle combined states
                        List<NFAState> nfaStates = new List<NFAState>();
                        foreach(var stateName in state.Key.Items)
                            if (stateName != "")
                                nfaStates.Add(process.States[new Set(stateName)]);

                        foreach (var label in process.Labels)
                        {
                            Set targetStates = new Set();
                            foreach(var nfaState in nfaStates)
                                if (nfaState.Transitions.ContainsKey(label))
                                    foreach (var toState in nfaState.Transitions[label])
                                        targetStates.Add(toState.Name);

                            states[state.Key].Transitions.Add(label, states[targetStates]);
                        }
                    }
                    else
                    {
                        // Handle single states
                        var nfaState = process.States[state.Key];
                        foreach (var label in process.Labels)
                        {
                            int transitionCount = 0;
                            if (nfaState.Transitions.ContainsKey(label))
                                transitionCount = nfaState.Transitions[label].Count;

                            if (transitionCount == 0)
                                states[state.Key].Transitions.Add(label, emptyState);
                            else if (transitionCount == 1)
                                states[state.Key].Transitions.Add(label, states[nfaState.Transitions[label][0].Name]);
                            else if (transitionCount > 1)
                            {
                                Set targetName = new Set();
                                foreach (var targetState in nfaState.Transitions[label])
                                    targetName.Add(targetState.Name);
                                states[state.Key].Transitions.Add(label, states[targetName]);
                            }
                        }
                    }
                }
            }

            states = RemoveUncreachableStates(states);

            var dfa = new DFAProcess(states, process.Labels);

            dfa.Validate();

            return dfa;
        }

        private Dictionary<Set, DFAState> InitializeDFA(NFAProcess process)
        {
            var states = new Dictionary<Set, DFAState>();

            int skip = 0;
            // Construct all possible state combinations
            foreach (var state in process.States.Values)
            {
                states.Add(
                    new Set(state.Name),
                    new DFAState(
                        new Set(state.Name),
                        new Dictionary<string, DFAState>(),
                        state.IsFinalState,
                        state.IsInitialState));

                foreach (var otherState in process.States.Values.Skip(skip))
                {
                    if (state.Name != otherState.Name)
                    {
                        states.Add(
                            new Set(state.Name, otherState.Name),
                            new DFAState(
                                new Set(state.Name, otherState.Name),
                                new Dictionary<string, DFAState>(),
                                state.IsFinalState || otherState.IsFinalState));
                    }
                }
                skip++;
            }

            // Construct Total State
            Set totalState = new Set();
            bool isFinal = false;
            foreach (var state in process.States.Values)
            {
                totalState.Add(state.Name);
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
            Set emptyStateName = new Set("ø");
            if (!states.ContainsKey(emptyStateName))
            {
                var emptyState = new DFAState(
                        emptyStateName,
                        new Dictionary<string, DFAState>());
                foreach (var label in process.Labels)
                    emptyState.Transitions.Add(label, emptyState);
                states.Add(
                    emptyStateName,
                    emptyState);
            }

            return states;
        }

        private Dictionary<Set, DFAState> RemoveUncreachableStates(Dictionary<Set, DFAState> states)
        {
            Dictionary<Set, DFAState> reachableStates = new Dictionary<Set, DFAState>();
            List<Set> initialStates = new List<Set>();
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
                        if (CanReach(states, new List<Set>(), initialState, state.Key))
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

        private bool CanReach(Dictionary<Set, DFAState> states, List<Set> visited, Set from, Set to)
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
