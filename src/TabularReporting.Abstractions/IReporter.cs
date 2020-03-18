using System.Collections.Generic;

namespace TabularReporting.Abstractions
{
    public interface IReporter<T>
    {
        IColumn Report(IEnumerable<T> sources, IEnumerable<IRowQuery> reportQueries);
    }
}
