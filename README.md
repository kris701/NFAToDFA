# NFA To DFA
A simple program to convert NFA's to DFA's. Its a simple CLI interface that accepts a NFA file and outputs an equivalent DFA for it.

![image](https://user-images.githubusercontent.com/22596587/230786103-a24ccb02-0ff3-42fd-99d5-8603c5cae683.png)

## CLI Arguments
There are the following command line arguments:
* `--nfa`: The path to the NFA file
* `--dfa`: The path to where you want the output DFA to go

## File Format
The input file format for this program is very simple, and consists of 3 parts:
* Label declaration
* State declaration
* Transitions

The label declarations is simply a set of what labels are available in the process:
* `{a, b, c, ...}`

The state declarations consists of max three parts and minimum one:
* `[(StateName):IsInit:IsFinal]`
The `IsInit` and `IsFinal` is optional.

Lastly, there is the transitions. These describe how to jump from state to state through a label:
* `(StateName) LabelName (StateName)`

![image](https://user-images.githubusercontent.com/22596587/231118394-e9988019-cb56-462b-aa55-fc922ba4386e.png)
