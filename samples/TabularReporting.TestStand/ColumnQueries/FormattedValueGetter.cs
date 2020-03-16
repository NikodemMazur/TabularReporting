using TabularReporting.Abstractions;
using NationalInstruments.TestStand.Interop.API;
using System;
using System.Collections.Generic;

namespace TabularReporting.TestStand
{
    internal class FormattedValueGetter : ISourcedColumnQuery<EnumerablePropertyObject>
    {
        readonly string _lookupString;
        readonly string _printfFormat;
        readonly string _prefix;
        readonly string _postfix;
        EnumerablePropertyObject _source;

        public FormattedValueGetter(string prefix, string lookupString, string postfix, string printfFormat = "")
        {
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            _lookupString = lookupString ?? throw new ArgumentNullException(nameof(lookupString));
            _postfix = postfix ?? throw new ArgumentNullException(nameof(postfix));
            _printfFormat = printfFormat;
        }

        public FormattedValueGetter(string lookupString, string printfFormat = "")
            : this(string.Empty, lookupString, string.Empty, printfFormat) { }

        EnumerablePropertyObject ISourcedColumnQuery<EnumerablePropertyObject>.Source { get => _source;  set => _source = value; }

        Union2<IEnumerable<IRowQuery>, object> IColumnQuery.Content =>
            new Union2<IEnumerable<IRowQuery>, object>.Case2(_prefix +
                                                             _source.GetFormattedValue(_lookupString,
                                                                                       PropertyOptions.PropOption_NoOptions,
                                                                                       _printfFormat,
                                                                                       false,
                                                                                       string.Empty) +
                                                             _postfix);
    }
}