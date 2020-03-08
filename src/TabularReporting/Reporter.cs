using TabularReporting.Abstractions;
using System;
using System.Collections.Generic;

namespace TabularReporting
{
    public class Reporter<T> : IReporter where T : IEnumerable<T>
    {
        T _source;
        IColumnQuery _reportQuery;

        public T Source
        {
            get => _source;
            set
            {
                if (value == default)
                    throw new ArgumentException("Invalid parameter.", nameof(Source));
                _source = value;
            }
        }
        public IColumnQuery ReportQuery
        {
            get => _reportQuery;
            set
            {
                _reportQuery = value ?? throw new ArgumentNullException(nameof(ReportQuery));
            }
        }

        public IColumn Report()
        {
            if (_source == null || _reportQuery == null)
                throw new InvalidOperationException($"Object is not initialized yet. Define report and provide source.");
            var report = new Projector<T>().ProjectToColumn(_source, _reportQuery);
            _source = default;
            _reportQuery = default;
            return report;
        }
    }
}