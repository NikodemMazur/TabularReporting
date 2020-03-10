using TabularReporting.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    public class OneTimeRowQuery : IRowQuery
    {
        readonly IEnumerable<IColumnQuery> _colQueries;

        bool _consumed;

        public OneTimeRowQuery(IEnumerable<IColumnQuery> colQueries)
        {
            _colQueries = colQueries;
            _consumed = false;
        }
        public OneTimeRowQuery() : this(Array.Empty<IColumnQuery>()) { }
        public OneTimeRowQuery(params IColumnQuery[] colQueries) : this(colQueries.AsEnumerable()) { }

        IEnumerable<IColumnQuery> IRowQuery.ColumnQueries
        {
            get
            {
                _consumed = true;
                return _colQueries;
            }
        }

        bool IRowQuery.Predicate
        {
            get
            {
                if (_consumed)
                    return false;
                return true;
            }
        }
    }
}