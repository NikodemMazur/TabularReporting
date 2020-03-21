using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    public class EveryRowQuery : IRowQuery
    {
        readonly IEnumerable<IColumnQuery> _colQueries;

        public EveryRowQuery(IEnumerable<IColumnQuery> colQueries)
        {
            _colQueries = colQueries;
        }
        public EveryRowQuery() : this(Enumerable.Empty<IColumnQuery>()) { }
        public EveryRowQuery(params IColumnQuery[] colQueries) : this(colQueries.AsEnumerable()) { }

        bool IRowQuery.Predicate => true;

        IEnumerable<IColumnQuery> IRowQuery.ColumnQueries => _colQueries;
    }
}