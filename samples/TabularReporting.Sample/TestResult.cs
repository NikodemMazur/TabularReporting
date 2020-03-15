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

        public override string ToString()
        {
            var sb = new StringBuilder();
            string str = string.Format("TestMetho")
        }
    }
}
