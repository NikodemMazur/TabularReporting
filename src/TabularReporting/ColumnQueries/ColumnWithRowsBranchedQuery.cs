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

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content => 
            new Union2<IEnumerable<IRowQuery>, object>.Case1(_rowQueries);

        T ISourcedColumnQuery<T>.Source { get; set; }

        IEnumerable<T> IBranchingSourcedColumnQuery<T>.Branch => 
            _branchSelector.Invoke(((ISourcedColumnQuery<T>)this).Source);
    }
}