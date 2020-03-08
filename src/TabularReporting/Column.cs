using System;
using System.Collections.Generic;
using System.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    public sealed class Column : IColumn
    {
        readonly Union2<IEnumerable<IRow>, object> _content;

        public Union2<IEnumerable<IRow>, object> Content => _content;

        public Column(Union2<IEnumerable<IRow>, object> content)
        {
            _content = content;
        }

        public Column(IEnumerable<IRow> rows) : this(new Union2<IEnumerable<IRow>, object>.Case1(rows)) { }

        public Column(object obj) : this(new Union2<IEnumerable<IRow>, object>.Case2(obj)) { }

        public Column(params IRow[] rows) : this(rows.AsEnumerable()) { }

        public Column(IColumn column) : this(column.Content) { }

        public int NestingLevel
        {
            get
            {
                return _content.Extract(f, g);

                int f(IEnumerable<IRow> rows)
                {
                    int acc = 0;

                    foreach (var row in rows)
                        foreach (var col in row.Columns.Select(c => new Column(c)).ToArray())
                            acc += col.NestingLevel;

                    return ++acc;
                }

                int g(object obj) => 0;
            }
        }

        IEnumerable<T> ProcessEndPointsCore<T>(Func<IColumnLocation, object, T> func, IColumnLocation currentLoc)
        {
            return _content.Extract(f, g);

            IEnumerable<T> f(IEnumerable<IRow> rows)
            {
                var results = new List<T>();
                var rowArray = rows.Select(row => new Row(row)).ToArray();
                var rowCount = rowArray.Length;

                for (int i = 0; i < rowCount; i++)
                {
                    var colArray = rowArray[i].Columns.Select(col => new Column(col)).ToArray();
                    var colCount = colArray.Length;

                    for (int j = 0; j < colCount; j++)
                        results.AddRange(colArray[j].ProcessEndPointsCore(func, currentLoc.Nest(i, j)));
                }

                return results;
            }

            IEnumerable<T> g(object obj) => new[] { func(currentLoc, obj) };
        }

        public IEnumerable<T> ProcessEndpoints<T>(Func<IColumnLocation, object, T> func) => ProcessEndPointsCore(func, ColumnLocation.Root);

        IEnumerable<T> ProcessCore<T>(Func<IColumnLocation, IColumn, T> func, IColumnLocation currentLoc)
        {
            var list = new List<T> { func(currentLoc, this) };
            list.AddRange(_content.Extract(f, g));
            return list;

            IEnumerable<T> f(IEnumerable<IRow> rows)
            {
                var results = new List<T>();
                var rowArray = rows.Select(row => new Row(row)).ToArray();
                var rowCount = rowArray.Length;

                for (int i = 0; i < rowCount; i++)
                {
                    var colArray = rowArray[i].Columns.Select(col => new Column(col)).ToArray();
                    var colCount = colArray.Length;

                    for (int j = 0; j < colCount; j++)
                        results.AddRange(colArray[j].ProcessCore(func, currentLoc.Nest(i, j)));
                }

                return results;
            }

            IEnumerable<T> g(object obj) => Array.Empty<T>();
        }

        public IEnumerable<T> Process<T>(Func<IColumnLocation, IColumn, T> func) =>
            ProcessCore(func, ColumnLocation.Root);

        IEnumerable<T> ProcessRowsCore<T>(Func<IRowLocation, IRow, T> func, IColumnLocation currentLoc)
        {
            return _content.Extract(f, g);

            IEnumerable<T> f(IEnumerable<IRow> rows)
            {
                var results = new List<T>();
                var rowArray = rows.Select(row => new Row(row)).ToArray();
                var rowCount = rowArray.Length;

                for (int i = 0; i < rowCount; i++)
                {
                    results.Add(func(currentLoc.NestOpen(i), rowArray[i]));

                    var colArray = rowArray[i].Columns.Select(col => new Column(col)).ToArray();
                    var colCount = colArray.Length;

                    for (int j = 0; j < colCount; j++)
                        results.AddRange(colArray[j].ProcessRowsCore(func, currentLoc.Nest(i, j)));
                }

                return results;
            }

            IEnumerable<T> g(object obj) => Array.Empty<T>();
        }

        public IEnumerable<T> ProcessRows<T>(Func<IRowLocation, IRow, T> func) =>
            ProcessRowsCore(func, ColumnLocation.Root);

        void ActCore(Action<IColumnLocation, IColumn> action, IColumnLocation currentLoc)
        {
            action(currentLoc, this);
            _content.Act(f, g);

            void f(IEnumerable<IRow> rows)
            {
                var rowArray = rows.Select(row => new Row(row)).ToArray();
                var rowCount = rowArray.Length;

                for (int i = 0; i < rowCount; i++)
                {
                    var colArray = rowArray[i].Columns.Select(col => new Column(col)).ToArray();
                    var colCount = colArray.Length;

                    for (int j = 0; j < colCount; j++)
                        colArray[j].ActCore(action, currentLoc.Nest(i, j));
                }
            }

            void g(object obj) { }
        }

        public void Act(Action<IColumnLocation, IColumn> action) =>
            ActCore(action, ColumnLocation.Root);

        void ActOnRowsCore(Action<IRowLocation, IRow> action, IColumnLocation currentLoc)
        {
            _content.Act(f, g);

            void f(IEnumerable<IRow> rows)
            {
                var rowArray = rows.Select(row => new Row(row)).ToArray();
                var rowCount = rowArray.Length;

                for (int i = 0; i < rowCount; i++)
                {
                    action(currentLoc.NestOpen(i), rowArray[i]);

                    var colArray = rowArray[i].Columns.Select(col => new Column(col)).ToArray();
                    var colCount = colArray.Length;

                    for (int j = 0; j < colCount; j++)
                        colArray[j].ActOnRowsCore(action, currentLoc.Nest(i, j));
                }
            }

            void g(object obj) { }
        }

        public void ActOnRows(Action<IRowLocation, IRow> action) =>
            ActOnRowsCore(action, ColumnLocation.Root);
    }
}