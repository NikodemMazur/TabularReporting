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

        ColUnion PopulateColumn(IEnumerable<T> sources, T source, IColumnQuery colQuery)
        {
            if (colQuery is ISourcedColumnQuery<T> sourcedColQuery)
                sourcedColQuery.Source = source; // If the type implements ISourcedXXXQuery, you must specify the source first.

            if (colQuery is IBranchingSourcedColumnQuery<T> branchingSourcedColQuery) // Branch the projection.
                return colQuery.Content.Extract<ColUnion>(rq => new ColUnion.Case1(ProjectToRows(branchingSourcedColQuery.Branch, rq)),
                obj => new ColUnion.Case2(obj));

            // If not branching, pass sources on.
            return colQuery.Content.Extract<ColUnion>(rq => new ColUnion.Case1(ProjectToRows(sources, rq)),
            obj => new ColUnion.Case2(obj));
        }

        IEnumerable<IRow> ProjectToRows(IEnumerable<T> sources, IEnumerable<IRowQuery> rowQueries)
        {
            var rowList = new List<IRow>();

            // If sources are empty, skip interating. It's a case where non-sourced queries are fed.
            if (sources.Count() == 0)
                rowList.AddRange(ProjectToRowsCore(sources, default, rowQueries)); // Note that no source is passed in.
                                                                                   // Row queries shouldn't carry sourced row/col queries
                                                                                   // in this case.
            else
                foreach (T source in sources)
                    rowList.AddRange(ProjectToRowsCore(sources, source, rowQueries));

            return rowList;

            IEnumerable<IRow> ProjectToRowsCore(IEnumerable<T> sources_, T source_, IEnumerable<IRowQuery> rowQueries_)
            {
                var rowList_ = new List<IRow>();
                foreach (IRowQuery rowQuery in rowQueries_)
                {
                    if (rowQuery is ISourcedRowQuery<T> sourcedRowQuery)
                        sourcedRowQuery.Source = source_; // If the type implements ISourcedXXXQuery, you must specify the source first.

                    if (rowQuery.Predicate)
                    {
                        // Compound new row recursively.
                        var row = new Row(rowQuery.ColumnQueries.Select(cq => new Column(PopulateColumn(sources_, source_, cq))).ToArray());
                        // Append the row.
                        rowList_.Add(row);
                    }
                }
                return rowList_;
            }
        }
    }
}