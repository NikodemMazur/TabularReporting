using System.Collections.Generic;
using System.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting.Sample
{
    public class ByAssertTypeFilter : ISourcedRowQuery<TestResult>
    {
        readonly string _assertType;
        readonly IEnumerable<IColumnQuery> _colQueries;

        public ByAssertTypeFilter(string assertType, IEnumerable<IColumnQuery> colQueries)
        {
            _assertType = assertType ?? throw new System.ArgumentNullException(nameof(assertType));
            _colQueries = colQueries ?? throw new System.ArgumentNullException(nameof(colQueries));
        }
        public ByAssertTypeFilter(string assertType, params IColumnQuery[] colQueries)
            : this(assertType, colQueries.AsEnumerable()) { }

        public TestResult Source { get; set; }

        public bool Predicate => Source.AssertType == _assertType;

        public IEnumerable<IColumnQuery> ColumnQueries => _colQueries;
    }
}
