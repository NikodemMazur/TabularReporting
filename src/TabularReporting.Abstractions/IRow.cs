using System.Collections.Generic;

namespace TabularReporting.Abstractions
{
    public interface IRow
    {
        IEnumerable<IColumn> Columns { get; }
    }
}