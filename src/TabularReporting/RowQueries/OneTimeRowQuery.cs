using TabularReporting.Abstractions;
using System;
using System.Collections.Generic;

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