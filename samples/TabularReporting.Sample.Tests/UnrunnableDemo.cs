using System;
using System.Collections.Generic;
using System.Linq;
using TabularReporting.Abstractions;
using System.IO;
using Xunit;
using TabularReporting;

namespace TabularReporting.Sample.Tests
{
    public class UnrunnableDemo
    {
        /// <summary>
        /// This code is for demo purpose
        /// </summary>
        // [Fact]
        public void Show()
        {
            // 1. Prepare result
            IEnumerable<TestResult> result = new[] {
                    new TestResult("TestMethod0", TimeSpan.FromSeconds(2), true, "Equal", null),
                    new TestResult("TestMethod1", TimeSpan.FromSeconds(2), false, "MultiTrue",
                        new TestResult("Subtest0", TimeSpan.FromSeconds(1), true, "True", null),
                        new TestResult("Subtest1", TimeSpan.FromSeconds(1), false, "True", null)),
                    new TestResult("TestMethod2", TimeSpan.FromSeconds(2), true, "Contains", null),
                    new TestResult("TestMethod3", TimeSpan.FromSeconds(2), true, "Equal", null),
                    new TestResult("TestMethod4", TimeSpan.FromSeconds(2), true, "Empty", null) };

            // 2. Define column (report) query
            IColumnQuery counterColQuery = new CounterColumnQuery(); // Mutable class - it's an ordinal column
            IEnumerable<IRowQuery> reportQueries = new IRowQuery[] {
                    new OneTimeRowQuery( // 2a. Define first header row
                        new ColumnWithStrQuery("Date"),
                        new ColumnWithStrQuery(DateTime.Now.ToShortDateString())),
                    new OneTimeRowQuery( // 2b. Define second header row
                        new ColumnWithStrQuery("Tested by"),
                        new ColumnWithStrQuery("Me")),
                    new OneTimeRowQuery( // 2c. Define third header row
                        new ColumnWithStrQuery("Final result"),
                        new ColumnWithStrQuery(result.All(tr => tr.Result) ? "Passed" : "Failed")),
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
                        new ColumnWithRowsBranchedQuery<TestResult>(tr => tr.InnerTests,
                            new EveryRowQuery(
                                new NameGetter(),
                                new ExecTimeInSecondsGetter(),
                                new ResultGetter()))
                    ) };

            // 3. Report
            IColumn reportedColumn =
                new Reporter<TestResult>().Report(result, reportQueries);

            // 3a. Preview column
            string columnStr = reportedColumn.ToXml().ToString();

            // 4. Format
            string formattedReport = new SimpleTextFormatter().Format(reportedColumn);

            // 5. Write
            string reportPath = new SimpleTextWriter().WriteReport(formattedReport, Path.GetTempPath(), "MyReport");

            // 6. Read
            string readReport = new SimpleTextReader().ReadReport(reportPath);

            // 7. Parse
            IColumn parsedColumn = new SimpleTextParser().Parse(readReport);

            // 8. Interpret
            string finalResult = reportedColumn[ColumnLocation.Root.Nest(2, 1)].ToString();
            // etc...
        }
    }
}