using TabularReporting.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TabularReporting
{
    using ColUnion = Union2<IEnumerable<IRow>, object>;

    public class Reporter<T> : IReporter<T> where T : IEnumerable<T>
    {
        public virtual IColumn Report(T source, IColumnQuery reportQuery) => ProjectToColumn(source, reportQuery);

        // ProjectToColumn along with ProjectToRows and PopulateColumn creates recursive chain.
        IColumn ProjectToColumn(T source, IColumnQuery colQuery) => new Column(PopulateColumn(source, colQuery));

        ColUnion PopulateColumn(T source, IColumnQuery colQuery)
        {
            if (colQuery is ISourcedColumnQuery<T> sourcedColQuery)
            {
                sourcedColQuery.Source = source; // If the type implements ISourcedXXXQuery, you must specify the source first.
                return colQuery.Content.Extract<ColUnion>(rq => new ColUnion.Case1(ProjectToRows(sourcedColQuery.Source, rq)),
                    obj => new ColUnion.Case2(obj));
            }
            else
                return colQuery.Content.Extract<ColUnion>(rq => new ColUnion.Case1(ProjectToRows(source, rq)),
                    obj => new ColUnion.Case2(obj));
        }

        IEnumerable<IRow> ProjectToRows(T source, IEnumerable<IRowQuery> rowQueries)
        {
            var rowList = new List<IRow>();

            // If the call doesn't have sourced queries or the source doesn't have elements, skip enumerating the source.
            if (rowQueries.OfType<ISourcedRowQuery<T>>().Count() == 0 || source.Count() == 0)
                rowList.AddRange(ProjectToRowsCore(source, rowQueries));
            else
                foreach (T element in source)
                    rowList.AddRange(ProjectToRowsCore(element, rowQueries));

            return rowList;

            IEnumerable<IRow> ProjectToRowsCore(T source_, IEnumerable<IRowQuery> rowQueries_)
            {
                var rowList_ = new List<IRow>();
                foreach (IRowQuery rowQuery in rowQueries_)
                {

                    IRow row;
                    
                    if (rowQuery is ISourcedRowQuery<T> sourcedRowQuery)
                    {
                        sourcedRowQuery.Source = source_; // If the type implements ISourcedXXXQuery, you must specify the source first.
                        // Compound new row recursively. Take source from sourced row query to allow for branching.
                        if (rowQuery.Predicate)
                            row = new Row(rowQuery.ColumnQueries.Select(cq => ProjectToColumn(sourcedRowQuery.Source, cq)).ToArray());
                    }
                    if (rowQuery.Predicate)
                    {
                        // Compound new row recursively.
                        row = new Row(rowQuery.ColumnQueries.Select(cq => ProjectToColumn(source_, cq)).ToArray());
                        // Append the row.
                        rowList_.Add(row);
                    }
                }
                return rowList_;
            }
        }
    }
}