using TabularReporting.Abstractions;
using NationalInstruments.TestStand.Interop.API;
using System;
using System.Collections.Generic;

namespace TabularReporting.TestStand
{
    internal class NumericDiff : ISourcedColumnQuery<EnumerablePropertyObject>
    {
        readonly string _lookupString;
        readonly string _format;
        double _register;
        EnumerablePropertyObject _source;

        /// <summary>
        /// Calculates difference between successive numeric-value property objects.
        /// </summary>
        /// <param name="initialValue"></param>
        /// <param name="lookupString">Lookup string to property object containing value you want a difference of.</param>
        /// <param name="format">C# numeric format string.</param>
        public NumericDiff(double initialValue, string lookupString, string format = "F3")
        {
            if (string.IsNullOrEmpty(lookupString))
            {
                throw new ArgumentException("Cannot be null or an empty string.", nameof(lookupString));
            }

            _register = initialValue;
            _lookupString = lookupString;
            _format = format;
        }

        EnumerablePropertyObject ISourcedColumnQuery<EnumerablePropertyObject>.Source { get => _source; set => _source = value; }

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content
        {
            get
            {
                double actual = _source.GetValNumber(_lookupString, PropertyOptions.PropOption_NoOptions);
                double diff = actual - _register;
                _register = actual;
                return new Union2<IEnumerable<IRowQuery>, object>.Case2(string.Format($"{{0:{_format}}}", diff));
            }
        }

        public NumericDiff Reset(double newInitialValue)
        {
            _register = newInitialValue;
            return this;
        }
    }
}