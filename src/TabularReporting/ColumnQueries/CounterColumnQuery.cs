using TabularReporting.Abstractions;
using System.Collections.Generic;

namespace TabularReporting
{
    public class CounterColumnQuery : IColumnQuery
    {
        readonly string _format;
        int _counter;

        public CounterColumnQuery(bool countFromOne = false, string format = "D")
        {
            _counter = countFromOne ? 1 : 0;
            _format = format;
        }

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content =>
            new Union2<IEnumerable<IRowQuery>, object>.Case2(string.Format($"{{0:{_format}}}", _counter++));
    }
}