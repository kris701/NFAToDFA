using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.Models
{
    public class DFAState
    {
        public string Name { get; set; }
        public Dictionary<string, DFAState> Transitions { get; set; }
        public bool IsFinalState { get; set; }
        public bool IsInitialState { get; set; }

        public DFAState(string name, Dictionary<string, DFAState> transitions, bool isFinalState = false, bool isInitialState = false)
        {
            Name = name;
            Transitions = transitions;
            IsFinalState = isFinalState;
            IsInitialState = isInitialState;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public override bool Equals(object? obj)
        {
            if (obj is DFAState state)
                return state.Name == Name;
            return false;
        }

        public override string? ToString()
        {
            var output = $"{Name}";
            if (IsFinalState)
                output += " [IsFinal]";
            if (IsInitialState)
                output += " [IsInit]";
            return output;
        }
    }
}
