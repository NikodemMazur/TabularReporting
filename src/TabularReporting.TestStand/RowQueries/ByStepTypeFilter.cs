using TabularReporting.Abstractions;
using NationalInstruments.TestStand.Interop.API;
using System;
using System.Collections.Generic;

namespace TabularReporting.TestStand
{
    internal class ByStepTypeFilter : ISourcedRowQuery<EnumerablePropertyObject>
    {
        readonly string _stepType;
        readonly IEnumerable<IColumnQuery> _colQueries;
        EnumerablePropertyObject _source;

        public ByStepTypeFilter(string stepType, IEnumerable<IColumnQuery> colQueries)
        {
            if (string.IsNullOrEmpty(stepType))
            {
                throw new ArgumentException("Cannot be null or an empty string.", nameof(stepType));
            }

            _stepType = stepType;
            _colQueries = colQueries;
        }
        public ByStepTypeFilter(string stepType) : this(stepType, Array.Empty<IColumnQuery>()) { }

        IEnumerable<IColumnQuery> IRowQuery.ColumnQueries => _colQueries;

        EnumerablePropertyObject ISourcedRowQuery<EnumerablePropertyObject>.Source { get => _source; set => _source = value; }

        bool IRowQuery.Predicate =>
            _source.GetValString("TS.StepType", PropertyOptions.PropOption_NoOptions) == _stepType;
    }
}