using System.Collections.Generic;

namespace TabularReporting.Abstractions
{
    public interface IRowQuery
    {
        bool Predicate { get; }
        IEnumerable<IColumnQuery> ColumnQueries { get; }
    }
}