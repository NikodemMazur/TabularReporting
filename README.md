# TabularReporting
Extensible mini-framework for generating reports.
## Introduction
### Terminology
- `source` - A type **T** implementing **IEnumerable\<T\>** from which you want to create a `report`.
- `report` - Composition of `row`s and `column`s.
- `row` - An element of a `report`, contained by `column`. It must contain at least one `column`. It is never an `endpoint element` of a `report`.
- `column` - An element of a `report` (report is a `column`, not a `row`). It can contain either `row`s or an `endpoint element`.
- `endpoint element` - An instance of **object**. It carries a useful information you want to put in a `report`.
- `reporting` - The act of creating a `report` as **IColumn** from `source` using `query`ies.
- `query` - The information about how to process a `source` to get `row`s or a `column`.
- `interpreting` - The act of translating a report as **IColumn** to real-world data fields you extracted from `source` during `reporting`.
### Typed source
Reports are often made from different object types but the procedure for defining the actual content of the report itself remains the same - a desired type is processed and converted into rows or/and columns.
### Constraint on source
The type **T** is expected to implement **IEnumerable\<T\>** which means that it may contain child elements of its type. In this way, the input source reflects the data hierarchy defined by columns and rows composition - a column can contain rows, which in turn contain columns.
### Report
report composition..........
### Modularity
In test-oriented companies it is common to develop and maintain large variety of report formats. This mini-framework is an attempt of standarization which allows for composing the tabular reporting out of interchangeable modules. Formatting often occurs along with the parsing back to the raw data. This task can be accomplished by creating a dual nature topology - that is, by defining two modules of opposite roles at once. That's how it's done here: the reporting principle of operation is opposite to the interpreting one, **IFormatter** is opposite to **IParser** and **IWriter** is opposite to **IReader**.
## UML
## Example
## More practical example (TestStand)