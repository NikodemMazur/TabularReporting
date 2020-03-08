using System;
using System.Collections.Generic;

namespace TabularReporting.Abstractions
{
    public interface IColumn
    {
        Union2<IEnumerable<IRow>, object> Content { get; }
        int NestingLevel { get; }

        IEnumerable<T> ProcessEndpoints<T>(Func<IColumnLocation, object, T> func);
        IEnumerable<T> Process<T>(Func<IColumnLocation, IColumn, T> func);
        void Act(Action<IColumnLocation, IColumn> action);
        IEnumerable<T> ProcessRows<T>(Func<IRowLocation, IRow, T> func);
        void ActOnRows(Action<IRowLocation, IRow> action);
    }
}