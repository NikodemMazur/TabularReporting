using System.Collections.Generic;

namespace TabularReporting.Abstractions
{
    public interface IColumnQuery
    {
        Union2<IEnumerable<IRowQuery>, object> Content { get; }
    }
}