using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using TabularReporting.Abstractions;

namespace TabularReporting.Sample.Tests
{
    public class UseExample
    {
        [Fact]
        public void Show()
        {
            // 1. Prepare result
            TestResult result =
                new TestResult("TestOfThings", TimeSpan.FromSeconds(10), true, null,
                    new TestResult("TestMethod0", TimeSpan.FromSeconds(2), true, "Equal", null),
                    new TestResult("TestMethod1", TimeSpan.FromSeconds(2), true, "MultipleTrue",
                        new TestResult("Subtest0", TimeSpan.FromSeconds(1), true, "True", null,
                        new TestResult("Subtest1", TimeSpan.FromSeconds(1), true, "True", null))),
                    new TestResult("TestMethod2", TimeSpan.FromSeconds(2), true, "Contains", null),
                    new TestResult("TestMethod3", TimeSpan.FromSeconds(2), true, "Equal", null),
                    new TestResult("TestMethod4", TimeSpan.FromSeconds(2), true, "Empty", null));

            // 2. Define column (report) query
            
            IColumnQuery reportQuery =
                new ColumnQueryWithRows(
                    new OneTimeRowQuery( // 2a. Define first header row
                        new ColumnQueryWithStr("Date"),
                        new ColumnQueryWithStr(DateTime.Now.ToShortDateString())),
                    new OneTimeRowQuery( // 2b. Define second header row
                        new ColumnQueryWithStr("Tested by"),
                        new ColumnQueryWithStr("Me")),
                    new ByAssertTypeFilter("Equal"), new Coun)

        }
    }
}