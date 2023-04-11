using NFAToDFA.Models;
using NFAToDFA.PowersetConstructors;

namespace NFAToDFATests
{
    [TestClass]
    public class NaiveConstructorTests
    {
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] {
                "Examples/example1.nfa",
                "// Label declaration{a,b}// State declaration, as well as if its a init state or a final state (or both)[(s0):IsInit][(s0,s2):IsFinal][(s1)][(s1,s2):IsFinal][(s2):IsFinal][(ø)]// Transitions(s0) a (s1)(s0) b (ø)(s0,s2) a (s1,s2)(s0,s2) b (s2)(s1) a (s0,s2)(s1) b (s0)(s1,s2) a (s0,s2)(s1,s2) b (s0,s2)(s2) a (s2)(s2) b (s2)(ø) a (ø)(ø) b (ø)"
            };
            yield return new object[] {
                "Examples/example2.nfa",
                "// Label declaration{a,b}// State declaration, as well as if its a init state or a final state (or both)[(t0):IsInit][(t0,t2):IsFinal][(t1)][(t1,t2):IsFinal][(t2):IsFinal][(ø)]// Transitions(t0) a (t1)(t0) b (ø)(t0,t2) a (t1,t2)(t0,t2) b (t2)(t1) a (t1,t2)(t1) b (t0)(t1,t2) a (t1,t2)(t1,t2) b (t0,t2)(t2) a (t2)(t2) b (t2)(ø) a (ø)(ø) b (ø)"
            };
            yield return new object[] {
                "Examples/example3.nfa",
                "// Label declaration{a,b}// State declaration, as well as if its a init state or a final state (or both)[(s0):IsInit][(s0,s2):IsFinal][(s1):IsFinal][(s1,s2):IsFinal][(s2):IsFinal]// Transitions(s0) a (s0,s2)(s0) b (s1)(s0,s2) a (s0,s2)(s0,s2) b (s1,s2)(s1) a (s2)(s1) b (s1)(s1,s2) a (s2)(s1,s2) b (s1,s2)(s2) a (s2)(s2) b (s2)"
            };
            yield return new object[] {
                "Examples/example4.nfa",
                "// Label declaration{a,b}// State declaration, as well as if its a init state or a final state (or both)[(t0):IsInit][(t1):IsFinal]// Transitions(t0) a (t1)(t0) b (t1)(t1) a (t1)(t1) b (t1)"
            };
        }

        [TestMethod]
        [DynamicData(nameof(GetTestData), DynamicDataSourceType.Method)]
        public void Can_ConstructCorrectly(string file, string expectedString)
        {
            // ARRANGE
            IPowersetConstructor constructor = new NaiveConstructor();
            var nfa = new NFAProcess(file);

            // ACT
            var dfa = constructor.ConstructDFA(nfa);
            var dfaString = dfa.ToString().Replace(Environment.NewLine, "");

            // ASSERT
            Assert.AreEqual(expectedString, dfaString);
        }
    }
}