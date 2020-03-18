using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    public class ColumnWithRowsQuery : IColumnQuery
    {
        readonly IEnumerable<IRowQuery> _rowQueries;

        public ColumnWithRowsQuery(IEnumerable<IRowQuery> rowQueries)
        {
            _rowQueries = rowQueries;
        }
        public ColumnWithRowsQuery() : this(Enumerable.Empty<IRowQuery>()) { }
        public ColumnWithRowsQuery(params IRowQuery[] rowQueries) : this(rowQueries.AsEnumerable()) { }

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content =>
            new Union2<IEnumerable<IRowQuery>, object>.Case1(_rowQueries);
    }
}