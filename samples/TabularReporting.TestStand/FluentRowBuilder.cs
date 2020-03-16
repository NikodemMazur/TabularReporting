using TabularReporting.Abstractions;
using System.Collections.Generic;
using NationalInstruments.TestStand.Interop.API;
using System;

namespace TabularReporting.TestStand
{
    /// <summary>
    /// Fluent wrapper for in-TestStand use.
    /// </summary>
    public class FluentRowBuilder
    {
        List<IColumnQuery> _colQueries;
        static readonly IDictionary<string, IColumnQuery> _countersDict = new Dictionary<string, IColumnQuery>();
        static readonly object _countersDictPadLock = new object();

        // Hide ctor from users.
        protected FluentRowBuilder() => _colQueries = new List<IColumnQuery>();

        /// <summary>
        /// Creates builder for building a row query.
        /// </summary>
        /// <returns></returns>
        public static FluentRowBuilder CreateRowBuilder()
        {
            return new FluentRowBuilder();
        }

        /// <summary>
        /// Adds column which gets the formatted value of property object.
        /// </summary>
        /// <param name="lookupString">TestStand lookup string to property object.</param>
        /// <param name="printfFormat">C's printf format string.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColWithFormattedValue(string lookupString, string printfFormat = "")
        {
            var col = new FormattedValueGetter(lookupString, printfFormat);
            _colQueries.Add(col);
            return this;
        }

        /// <summary>
        /// Adds column which gets the formatted value of property object.
        /// </summary>
        /// <param name="prefix">Constant prefix string prepended to property object formatted value.</param>
        /// <param name="lookupString">TestStand lookup string to property object.</param>
        /// <param name="postfix">Constant postfix string appended to property object formatted value.</param>
        /// <param name="printfFormat">C's printf format string.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColWithFormattedValue(string prefix, string lookupString, string postfix, string printfFormat = "")
        {
            var col = new FormattedValueGetter(prefix, lookupString, postfix, printfFormat);
            _colQueries.Add(col);
            return this;
        }

        /// <summary>
        /// Adds constant column which is a container for row queries.
        /// </summary>
        /// <param name="rowQueries">Product of <see cref="BuildOneTimeRow"/> or <see cref="BuildRowByStepType(string)"/>.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColWithRows(params IRowQuery[] rowQueries)
        {
            var col = new ColumnQueryWithRows(rowQueries);
            _colQueries.Add(col);
            return this;
        }

        /// <summary>
        /// Adds constant column which is a constant string.
        /// </summary>
        /// <param name="str">Actual column content.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColWithStr(string str)
        {
            var col = new ColumnQueryWithStr(str);
            _colQueries.Add(col);
            return this;
        }

        /// <summary>
        /// Adds column with memory which increments returned value at each use.
        /// </summary>
        /// <param name="counterName">ID of counter.</param>
        /// <param name="countFromOne">When true this column counts from one instead of zero.</param>
        /// <param name="format">CSharp standard numeric format string.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColCounter(string counterName = "", bool countFromOne = true, string format = "D")
        {
            lock (_countersDictPadLock)
            {
                if (_countersDict.TryGetValue(counterName, out IColumnQuery value))
                    _colQueries.Add(value);
                else
                {
                    var col = new CounterColumnQuery(countFromOne, format);
                    _colQueries.Add(col);
                    _countersDict[counterName] = col;
                }
            }
            return this;
        }

        /// <summary>
        /// Adds column which calculates difference between numeric property object, the <paramref name="lookupString"/> points to.
        /// </summary>
        /// <param name="initialValue">Initial register value.</param>
        /// <param name="lookupString">TestStand lookup string to numeric property object.</param>
        /// <param name="format">CSharp standard numeric format string.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColNumericDiff(double initialValue, string lookupString, string format = "F3")
        {
            var col = new NumericDiff(initialValue, lookupString, format);
            _colQueries.Add(col);
            return this;
        }

        /// <summary>
        /// Adds column which changes path of projection. Use this column to iterate an array subproperty.
        /// </summary>
        /// <param name="lookupString">TestStand lookup string to branch.</param>
        /// <param name="rowQueries">Product of <see cref="BuildOneTimeRow"/> or <see cref="BuildRowByStepType(string)"/></param>
        /// <returns></returns>
        public FluentRowBuilder AddColWithRowsFromPropertyObject(string lookupString, params IRowQuery[] rowQueries)
        {
            var col = new ColumnQueryWithRowsBranched<EnumerablePropertyObject>(epo =>
                new EnumerablePropertyObject(epo.GetPropertyObject(lookupString, 0x0)), rowQueries);
            _colQueries.Add(col);
            return this;
        }

        /// <summary>
        /// Builds row which processes TestStand array elements only if an element's StepType equals <paramref name="stepType"/>.
        /// </summary>
        /// <param name="stepType">TestStand step type. For example, 'PassFailTest'.</param>
        /// <returns></returns>
        public IRowQuery BuildRowByStepType(string stepType)
        {
            return new ByStepTypeFilter(stepType, _colQueries.ToArray());
        }

        /// <summary>
        /// Builds constant row which is used only once at the beginning of the projection.
        /// </summary>
        /// <returns></returns>
        public IRowQuery BuildOneTimeRow()
        {
            return new OneTimeRowQuery(_colQueries.ToArray());
        }

        /// <summary>
        /// Builds row which processes every element (its predicate is always true).
        /// </summary>
        /// <returns></returns>
        public IRowQuery BuildEveryRow()
        {
            return new EveryRowQuery<EnumerablePropertyObject>(_colQueries.ToArray());
        }
    }
}