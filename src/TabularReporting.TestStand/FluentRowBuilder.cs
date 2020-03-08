using TabularReporting.Abstractions;
using System.Collections.Generic;

namespace TabularReporting.TestStand
{
    /// <summary>
    /// Fluent wrapper for in-TestStand use.
    /// </summary>
    public class FluentRowBuilder
    {

        List<IColumnQuery> _colQueries;

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
        public FluentRowBuilder AddColWithRows(IRowQuery[] rowQueries)
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
        /// <param name="countFromOne">When true this column counts from one instead of zero.</param>
        /// <param name="format">CSharp standard numeric format string.</param>
        /// <returns></returns>
        public FluentRowBuilder AddColCounter(bool countFromOne = true, string format = "D")
        {
            var col = new CounterColumnQuery(countFromOne, format);
            _colQueries.Add(col);
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
        /// Builds row which process TestStand array elements only if an element's StepType equals <paramref name="stepType"/>.
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

    }
}