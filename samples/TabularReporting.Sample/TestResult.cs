using System;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting.Sample
{
    public class TestResult
    {
        public string TestMethodName { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public bool Result { get; set; }
        public string AssertType { get; set; }
        public IEnumerable<TestResult> InnerTests { get; set; }

        public TestResult(string testMethodName, TimeSpan executionTime,
            bool result, string assertType, IEnumerable<TestResult> innerTests)
        {
            TestMethodName = testMethodName;
            ExecutionTime = executionTime;
            Result = result;
            AssertType = assertType;
            InnerTests = innerTests;
        }

        public TestResult(string testMethodName, TimeSpan executionTime,
            bool result, string assertType, params TestResult[] innerTests)
                : this(testMethodName, executionTime, result, assertType, innerTests.AsEnumerable()) { }
    }
}
