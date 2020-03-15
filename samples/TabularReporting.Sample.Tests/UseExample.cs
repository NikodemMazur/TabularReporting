using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TabularReporting.Abstractions;
using System.IO;

namespace TabularReporting.Sample.Tests
{
    public class UseExample
    {
        [Fact]
        public void Show()
        {
            // 1. Prepare result
            TestResult result =
                new TestResult("TestOfThings", TimeSpan.FromSeconds(10), false, null,
                    new TestResult("TestMethod0", TimeSpan.FromSeconds(2), true, "Equal", null),
                    new TestResult("TestMethod1", TimeSpan.FromSeconds(2), false, "MultiTrue",
                        new TestResult("Subtest0", TimeSpan.FromSeconds(1), true, "True", null),
                        new TestResult("Subtest1", TimeSpan.FromSeconds(1), false, "True", null)),
                    new TestResult("TestMethod2", TimeSpan.FromSeconds(2), true, "Contains", null),
                    new TestResult("TestMethod3", TimeSpan.FromSeconds(2), true, "Equal", null),
                    new TestResult("TestMethod4", TimeSpan.FromSeconds(2), true, "Empty", null));

            // 2. Define column (report) query
            IColumnQuery counterColQuery = new CounterColumnQuery(); // Mutable class - it's an ordinal column
            IColumnQuery reportQuery =
                new ColumnQueryWithRows(
                    new OneTimeRowQuery( // 2a. Define first header row
                        new ColumnQueryWithStr("Date"),
                        new ColumnQueryWithStr(DateTime.Now.ToShortDateString())),
                    new OneTimeRowQuery( // 2b. Define second header row
                        new ColumnQueryWithStr("Tested by"),
                        new ColumnQueryWithStr("Me")),
                    new OneTimeRowQuery( // 2c. Define third header row
                        new ColumnQueryWithStr("Final result"), 
                        new ColumnQueryWithStr(result.Result.ToString())),
                    // 2c. Define body
                    new ByAssertTypeFilter("Equal", 
                        counterColQuery, 
                        new NameGetter(), 
                        new ExecTimeInSecondsGetter(), 
                        new ResultGetter()),
                    new ByAssertTypeFilter("MultiTrue", 
                        counterColQuery, 
                        new NameGetter(), 
                        new ExecTimeInSecondsGetter(), 
                        new ColumnQueryWithRows(
                            new EveryRowQuery<EnumerableTestResult>(
                                new NameGetter(), 
                                new ExecTimeInSecondsGetter(), 
                                new ResultGetter()))));

            // 3. Report
            IColumn reportedColumn =
                new Reporter<EnumerableTestResult>().Report(new EnumerableTestResult(result), reportQuery);

            // 3a. Preview column
            string columnStr = reportedColumn.ToXml().ToString();

            // 4. Format
            string formattedReport = new SimpleTextFormatter().Format(reportedColumn);

            // 5. Write
            string reportPath = new SimpleTextWriter().WriteReport(formattedReport, Path.GetTempPath(), "MyReport");

            // 6. Read
            string readReport = new SimpleTextReader().ReadReport(reportPath);
            Assert.Equal(formattedReport, readReport);

            // 7. Parse
            IColumn parsedColumn = new SimpleTextParser().Parse(readReport);

            // 8. Interpret
            IEnumerable<IRow> rows = parsedColumn.Content.Extract(rows_ => rows_, obj => null);
            string date = rows.ToArray()[0].Columns.ToArray()[1].Content.Extract(rows_ => null, obj => obj.ToString());
            // etc...
        }
    }
}