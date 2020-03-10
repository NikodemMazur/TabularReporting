using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    public class ColumnQueryWithRows : IColumnQuery
    {
        readonly IEnumerable<IRowQuery> _rowQueries;

        public ColumnQueryWithRows(IEnumerable<IRowQuery> rowQueries)
        {
            _rowQueries = rowQueries;
        }
        public ColumnQueryWithRows() : this(Enumerable.Empty<IRowQuery>()) { }
        public ColumnQueryWithRows(params IRowQuery[] rowQueries) : this(rowQueries.AsEnumerable()) { }

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content =>
            new Union2<IEnumerable<IRowQuery>, object>.Case1(_rowQueries);
    }
}