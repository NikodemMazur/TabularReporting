using System;
using System.Collections;
using System.Collections.Generic;

namespace TabularReporting.Sample
{
    public class EnumerableTestResult : TestResult, IEnumerable<EnumerableTestResult>
    {
        readonly TestResult _testResult;

        // Ctor accepting object to decorate
        public EnumerableTestResult(TestResult testResult) 
            : base(testResult.TestMethodName, testResult.ExecutionTime, testResult.Result, testResult.AssertType, testResult.InnerTests)
        {
            _testResult = testResult;
        }

        // The only job you have to do
        public IEnumerator<EnumerableTestResult> GetEnumerator()
        {
            foreach (TestResult it in _testResult.InnerTests)
                yield return new EnumerableTestResult(it);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
