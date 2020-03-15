using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TabularReporting.Sample
{
    public class TestResult
    {
        public string TestName { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public bool Result { get; set; }
        public string AssertType { get; set; }
        public IEnumerable<TestResult> InnerTests { get; set; }

        public TestResult(string testName, TimeSpan executionTime,
            bool result, string assertType, IEnumerable<TestResult> innerTests)
        {
            TestName = testName;
            ExecutionTime = executionTime;
            Result = result;
            AssertType = assertType;
            InnerTests = innerTests;
        }

        public TestResult(string testName, TimeSpan executionTime,
            bool result, string assertType, params TestResult[] innerTests)
                : this(testName, executionTime, result, assertType, innerTests.AsEnumerable()) { }

        string ToStringCore(int indent)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[TestName: {0}; ExecutionTime: {1}; Result: {2}; AssertType: {3}; InnerTests: ",
                TestName, ExecutionTime, Result, AssertType);
            if (InnerTests != null)
            {
                foreach (var it in InnerTests)
                {
                    sb.Append($"{Environment.NewLine}");
                    sb.Append(new string('\t', indent));
                    sb.Append(it.ToStringCore(indent + 1));
                }
            }
            sb.Append("]");
            return sb.ToString();
        }

        public override string ToString() => ToStringCore(1);
    }
}
