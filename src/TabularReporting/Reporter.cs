using TabularReporting.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    using ColUnion = Union2<IEnumerable<IRow>, object>;

    public class Reporter<T> : IReporter<T>
    {
        public IColumn Report(IEnumerable<T> sources, IEnumerable<IRowQuery> reportQueries) =>
            new Column(ProjectToRows(sources, reportQueries));

        ColUnion PopulateColumn(T source, IColumnQuery colQuery)
        {
            if (colQuery is ISourcedColumnQuery<T> sourcedColQuery)
                sourcedColQuery.Source = source; // If the type implements ISourcedXXXQuery, you must specify the source first.

            if (colQuery is IBranchingSourcedColumnQuery<T> branchingSourcedColQuery)
                return colQuery.Content.Extract<ColUnion>(rq => new ColUnion.Case1(ProjectToRows(branchingSourcedColQuery.Branch, rq)),
                obj => new ColUnion.Case2(obj));

            return colQuery.Content.Extract<ColUnion>(rq => new ColUnion.Case1(ProjectToRows(Enumerable.Empty<T>(), rq)),
            obj => new ColUnion.Case2(obj));
        }

        IEnumerable<IRow> ProjectToRows(IEnumerable<T> sources, IEnumerable<IRowQuery> rowQueries)
        {
            var rowList = new List<IRow>();

            // If the call doesn't have sourced queries or sources are empty, skip interating forcing projection.
            if (rowQueries.OfType<ISourcedRowQuery<T>>().Count() == 0 || sources.Count() == 0)
                rowList.AddRange(ProjectToRowsCore(default, rowQueries, false));
            else
                foreach (T source in sources)
                    rowList.AddRange(ProjectToRowsCore(source, rowQueries, true));

            return rowList;

            IEnumerable<IRow> ProjectToRowsCore(T source_, IEnumerable<IRowQuery> rowQueries_, bool cast)
            {
                var rowList_ = new List<IRow>();
                foreach (IRowQuery rowQuery in rowQueries_)
                {

                    IRow row;

                    if (rowQuery is ISourcedRowQuery<T> sourcedRowQuery)
                        sourcedRowQuery.Source = source_; // If the type implements ISourcedXXXQuery, you must specify the source first.

                    if (rowQuery.Predicate)
                    {
                        // Compound new row recursively.
                        row = new Row(rowQuery.ColumnQueries.Select(cq => new Column(PopulateColumn(source_, cq))).ToArray());
                        // Append the row.
                        rowList_.Add(row);
                    }
                }
                return rowList_;
            }
        }
    }
}