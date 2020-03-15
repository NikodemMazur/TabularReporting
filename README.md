### What is it?
Extensible framework for creating tabular reports from any type.
To add: graphic preseting operation of main interfaces.
### Terminology
- `source` - A type **T** implementing **IEnumerable\<T\>** from which you want to create a `report`.
- `report` - Composition of `row`s and `column`s.
- `row` - An element of a `report`, contained by `column`. It must contain at least one `column`. It is never an `endpoint` of a `report`.
- `column` - An element of a `report` (report is a `column`, not a `row`). It can contain either `row`s or an `endpoint`.
- `endpoint` - An instance of **object**. It carries a useful information you want to put in a `report`.
- `reporting` - The act of creating a `report` as **IColumn** from `source` using `query`ies.
- `query` - The information about how to process a `source` to get `row`s or a `column`.
- `interpreting` - The act of translating a report as **IColumn** to real-world data fields you extracted from `source` during `reporting`.
### Decorator needed.
The type **T** is expected to implement **IEnumerable\<T\>** which means that it may contain child elements of its type. In this way, the input source reflects the data hierarchy defined by columns and rows composition - a column can contain rows, which in turn contain columns.

To add: decorator code of sample type
### Topology overview
To add: uml logical/development view

In test-oriented companies it is common to develop and maintain large variety of report formats. This mini-framework is an attempt of standarization which allows for composing the tabular reporting out of interchangeable modules. Formatting often occurs along with the parsing back to the raw data. This task can be accomplished by creating a dual nature topology - that is, by defining two modules of opposite roles at once. That's how it's done here: the reporting principle of operation is opposite to the interpreting one, **IFormatter** is opposite to **IParser** and **IWriter** is opposite to **IReader**.

## Example

```csharp
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
```

To add: code snippets for sample type.

## More practical example (TestStand)

To add: sequence view screenshots and code snippets for EnumerablePropertyObject