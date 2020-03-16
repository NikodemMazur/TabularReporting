using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabularReporting
{
    public class ColumnQueryWithRowsBranched<T> : ISourcedColumnQuery<T>
    {
        readonly Func<T, T> _branchSelector;
        readonly IEnumerable<IRowQuery> _rowQueries;
        T _source;

        public ColumnQueryWithRowsBranched(Func<T, T> branchSelector, IEnumerable<IRowQuery> rowQueries)
        {
            _branchSelector = branchSelector;
            _rowQueries = rowQueries;
        }
        public ColumnQueryWithRowsBranched(Func<T, T> branchSelector) 
            : this(branchSelector, Enumerable.Empty<IRowQuery>()) { }
        public ColumnQueryWithRowsBranched(Func<T, T> branchSelector, params IRowQuery[] rowQueries) 
            : this(branchSelector, rowQueries.AsEnumerable()) { }

        public T Source
        {
            get => _branchSelector.Invoke(_source);
            set { _source = value; }
        }

        public Union2<IEnumerable<IRowQuery>, object> Content => 
            new Union2<IEnumerable<IRowQuery>, object>.Case1(_rowQueries);
    }
}