using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFAToDFA.Models
{
    public class Transition
    {
        public string Label { get; set; }
        public NFAState State { get; set; }

        public Transition(string label, NFAState state)
        {
            Label = label;
            State = state;
        }
    }
}
