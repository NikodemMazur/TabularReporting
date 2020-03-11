using System.Collections.Generic;
using TabularReporting.Abstractions;

namespace TabularReporting.Sample
{
    public class ResultGetter : ISourcedColumnQuery<EnumerableTestResult>
    {
        public EnumerableTestResult Source { get; set; }

        public Union2<IEnumerable<IRowQuery>, object> Content =>
            new Union2<IEnumerable<IRowQuery>, object>.Case2(Source.Result.ToString());
    }
}
