using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TabularReporting
{
    public class ColumnWithRowsBranchedQuery<T> : IBranchingSourcedColumnQuery<T>
    {
        readonly Func<T, IEnumerable<T>> _branchSelector;
        readonly IEnumerable<IRowQuery> _rowQueries;

        public ColumnWithRowsBranchedQuery(Func<T, IEnumerable<T>> branchSelector, IEnumerable<IRowQuery> rowQueries)
        {
            _branchSelector = branchSelector;
            _rowQueries = rowQueries;
        }
        public ColumnWithRowsBranchedQuery(Func<T, IEnumerable<T>> branchSelector) 
            : this(branchSelector, Enumerable.Empty<IRowQuery>()) { }
        public ColumnWithRowsBranchedQuery(Func<T, IEnumerable<T>> branchSelector, params IRowQuery[] rowQueries) 
            : this(branchSelector, rowQueries.AsEnumerable()) { }

        public Union2<IEnumerable<IRowQuery>, object> Content => 
            new Union2<IEnumerable<IRowQuery>, object>.Case1(_rowQueries);

        public T Source { get; set; }

        public IEnumerable<T> Branch => _branchSelector.Invoke(Source);
    }
}