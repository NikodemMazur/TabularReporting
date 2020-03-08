using System.Collections.Generic;
using System.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    public sealed class Row : IRow
    {
        readonly IEnumerable<IColumn> _columns;

        public IEnumerable<IColumn> Columns => _columns;

        public Row(IEnumerable<IColumn> columns)
        {
            _columns = columns;
        }

        public Row(params IColumn[] columns) : this(columns.AsEnumerable()) { }

        public Row(IRow row) : this(row.Columns) { }
    }
}
