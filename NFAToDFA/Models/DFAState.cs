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

        public bool IsComposite { get; set; }
        public List<string> CompositeName { get; set; }

        public DFAState(string name, List<string> compositeName, Dictionary<string, DFAState> transitions, bool isFinalState = false, bool isInitialState = false, bool isComposite = false)
        {
            Name = name;
            CompositeName = compositeName;
            Transitions = transitions;
            IsFinalState = isFinalState;
            IsInitialState = isInitialState;
            IsComposite = isComposite;
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
