using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    public class EveryRowQuery<T> : ISourcedRowQuery<T>
    {
        readonly IEnumerable<IColumnQuery> _colQueries;

        public EveryRowQuery(IEnumerable<IColumnQuery> colQueries)
        {
            _colQueries = colQueries;
        }
        public EveryRowQuery() : this(Enumerable.Empty<IColumnQuery>()) { }
        public EveryRowQuery(params IColumnQuery[] colQueries) : this(colQueries.AsEnumerable()) { }

        public bool Predicate => true;

        public IEnumerable<IColumnQuery> ColumnQueries => _colQueries;

        public T Source { get; set; }
    }
}