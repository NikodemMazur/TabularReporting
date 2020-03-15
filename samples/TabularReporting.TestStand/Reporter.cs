using TabularReporting.Abstractions;
using NationalInstruments.TestStand.Interop.API;
using System.Collections.Generic;

namespace TabularReporting.TestStand
{
    public class Reporter
    {
        Reporter<EnumerablePropertyObject> _reporter;

        public Reporter()
        {
            _reporter = new Reporter<EnumerablePropertyObject>();
        }

        public IColumn Report(PropertyObject source, IEnumerable<IRowQuery> reportRowQueries) =>
            _reporter.Report(new EnumerablePropertyObject(source), new ColumnQueryWithRows(reportRowQueries));
    }
}