### What is it?
Extensible framework for creating tabular reports from any type.

![What is it?](/images/what_is_it.png)

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

#### Type

```csharp
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
        // ...
    }

    // ...
}
```
#### Decorator

```csharp
public class EnumerableTestResult : TestResult, IEnumerable<EnumerableTestResult>
{
    readonly TestResult _testResult;

    // Ctor accepting object to decorate
    public EnumerableTestResult(TestResult testResult) 
        : base(testResult.TestName, testResult.ExecutionTime, testResult.Result, testResult.AssertType, testResult.InnerTests)
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
```

### Example

#### Report --> Format --> Write

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
```
#### Read --> Parse --> Interpret
```csharp
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

#### Output of **SimpleTextFormatter**

```none
+--------------------------------------------+
¦ Date                  ¦ 15.03.2020         ¦
¦-----------------------+--------------------¦
¦ Tested by             ¦ Me                 ¦
¦-----------------------+--------------------¦
¦ Final result          ¦ False              ¦
¦--------------------------------------------¦
¦ 0 ¦ TestMethod0 ¦ 2 ¦ True                 ¦
¦---+-------------+---+----------------------¦
¦ 1 ¦ TestMethod1 ¦ 2 ¦ Subtest0 ¦ 1 ¦ True  ¦
¦   ¦             ¦   ¦----------+---+-------¦
¦   ¦             ¦   ¦ Subtest1 ¦ 1 ¦ False ¦
¦---+-------------+---+----------------------¦
¦ 2 ¦ TestMethod3 ¦ 2 ¦ True                 ¦
+--------------------------------------------+
```

### More practical example (TestStand)

#### 1. Decorate **PropertyObject**

```csharp
public class EnumerablePropertyObject : PropertyObject, IEnumerable<EnumerablePropertyObject>
{
    readonly PropertyObject _propObj;

    public EnumerablePropertyObject(PropertyObject propObj)
    {
        _propObj = propObj ?? throw new ArgumentNullException(nameof(propObj));
    }

	// ...
	// PropertyObject implementation
	// ...

    public IEnumerator<EnumerablePropertyObject> GetEnumerator()
    {
        if (!_propObj.IsArray()) // If PropertyObject is not an array do not iterate.
            yield break;

        int numElements = _propObj.GetNumElements();
        for (int i = 0; i < numElements; i++)
        {
            yield return new EnumerablePropertyObject(_propObj.GetPropertyObjectByOffset(i,
				PropertyOptions.PropOption_NoOptions));
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
```

#### 2. Implement queries

```csharp
internal class FormattedValueGetter : ISourcedColumnQuery<EnumerablePropertyObject>
{
    readonly string _lookupString;
    readonly string _printfFormat;
    readonly string _prefix;
    readonly string _postfix;
    EnumerablePropertyObject _source;

    public FormattedValueGetter(string prefix, string lookupString, string postfix, string printfFormat = "")
    {
        if (string.IsNullOrEmpty(lookupString))
        {
            throw new ArgumentException("Lookup string cannot be null or an empty string.", nameof(lookupString));
        }

        _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        _lookupString = lookupString;
        _postfix = postfix ?? throw new ArgumentNullException(nameof(postfix));
        _printfFormat = printfFormat;
    }

    public FormattedValueGetter(string lookupString, string printfFormat = "")
        : this(string.Empty, lookupString, string.Empty, printfFormat) { }

    EnumerablePropertyObject ISourcedColumnQuery<EnumerablePropertyObject>.Source { get => _source;  set => _source = value; }

    Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content =>
        new Union2<IEnumerable<IRowQuery>, object>.Case2(_prefix +
                                                         _source.GetFormattedValue(_lookupString,
                                                                                   PropertyOptions.PropOption_NoOptions,
                                                                                   _printfFormat,
                                                                                   false,
                                                                                   string.Empty) +
                                                         _postfix);
}
```