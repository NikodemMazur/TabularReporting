using TabularReporting.Abstractions;
using NationalInstruments.TestStand.Interop.API;

namespace TabularReporting.TestStand
{
    public class Reporter : Reporter<EnumerablePropertyObject>
    {
        public Reporter(PropertyObject source, params IRowQuery[] reportRowQueries) : base()
        {
            Source = new EnumerablePropertyObject(source);
            ReportQuery = new ColumnQueryWithRows(reportRowQueries);
        }
    }
}