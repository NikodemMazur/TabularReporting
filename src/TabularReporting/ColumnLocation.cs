using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    /// <summary>
    /// Navigates to an element in the column tree.
    /// </summary>
    public class ColumnLocation : IColumnLocation, IEquatable<ColumnLocation>, IComparable<ColumnLocation>
    {
        readonly int[] _rowNesting;
        readonly int[] _columnNesting;

        /// <summary>
        /// If you want to create a root location, use <see cref="ColumnLocation.Root"/> instead.
        /// </summary>
        /// <param name="rowNesting">An element index indicates the nesting level of row 
        /// and the value of the element is the row number in a column.</param>
        /// <param name="columnNesting">An element index indicates the nesting level of column
        /// and the value of the element is the column number in a row</param>
        public ColumnLocation(int[] rowNesting, int[] columnNesting)
        {
            if (rowNesting == null || columnNesting == null)
                throw new ArgumentNullException();

            if (rowNesting.Count() != columnNesting.Count())
                throw new ArgumentException($"{nameof(rowNesting)} and {nameof(columnNesting)} must have the same length.");

            _rowNesting = rowNesting;
            _columnNesting = columnNesting;
        }

        ColumnLocation(ColumnLocation other, int rowNumber, int columnNumber)
            : this(new List<int>(other._rowNesting) { rowNumber }.ToArray(),
                  new List<int>(other._columnNesting) { columnNumber }.ToArray()) { }

        ColumnLocation() : this(Array.Empty<int>(), Array.Empty<int>()) { }

        /// <summary>
        /// Returns an array of row numbers. An element index indicates the nesting level of row 
        /// and the value of the element is the row number in a column.
        /// </summary>
        public int[] RowNesting => _rowNesting;

        /// <summary>
        /// Returns an array of column numbers. An element index indicates the nesting level of column
        /// and the value of the element is the column number in a row.
        /// </summary>
        public int[] ColumnNesting => _columnNesting;

        /// <summary>
        /// Creates <see cref="ColumnLocation"/> with one nesting level more.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public IColumnLocation Nest(int rowNumber, int columnNumber) => new ColumnLocation(this, rowNumber, columnNumber);

        /// <summary>
        /// Creates <see cref="ColumnLocation"/> with one nesting level more with open column (the location is ended with row, not column).
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public IRowLocation NestOpen(int rowNumber) => new RowLocation(this, rowNumber);

        /// <summary>
        /// Checks if <see cref="ColumnLocation"/> points to the root - a column not contained by row.
        /// </summary>
        public bool IsRoot => _rowNesting.Length == 0 && _columnNesting.Length == 0;

        /// <summary>
        /// Returns an empty <see cref="ColumnLocation"/>.
        /// </summary>
        public static IColumnLocation Root => new ColumnLocation();

        /// <summary>
        /// Returns the deepest column nesting level.
        /// </summary>
        public int NestingLevel => _columnNesting.Length;

        /// <summary>
        /// Helper comparer inverted with respect to the class <see cref="IComparable{T}"/> semantics.
        /// </summary>
        public static IComparer<IColumnLocation> InvertedComparer => new PluggableInvertedComparer();

        /// <summary>
        /// Opens location. Returned location no longers points to specific column but rather ends with a row.
        /// </summary>
        /// <returns></returns>
        public IRowLocation Open() => new RowLocation(_rowNesting, _columnNesting.Take(_rowNesting.Count() - 1).ToArray());

        /// <summary>
        /// Returns <see cref="ColumnLocation"/> with one nesting level less.
        /// </summary>
        /// <returns></returns>
        public IColumnLocation Trim() => new ColumnLocation(_rowNesting.Take(NestingLevel - 1).ToArray(), _columnNesting.Take(NestingLevel - 1).ToArray());

        #region Equality semantics override

        public bool Equals(ColumnLocation other)
        {
            if (other.RowNesting == null || other.ColumnNesting == null)
                return false;

            bool rowNumbersEqual = StructuralComparisons.StructuralEqualityComparer.Equals(_rowNesting, other.RowNesting);
            bool columnNumbersEqual = StructuralComparisons.StructuralEqualityComparer.Equals(_columnNesting, other.ColumnNesting);

            return rowNumbersEqual && columnNumbersEqual;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ColumnLocation location))
                return false;
            return Equals(location);
        }

        public override int GetHashCode()
        {
            // Josh Bloch's pattern.
            int hash = 17;
            hash = hash * 31 + StructuralComparisons.StructuralEqualityComparer.GetHashCode(_rowNesting);
            hash = hash * 31 + StructuralComparisons.StructuralEqualityComparer.GetHashCode(_columnNesting);

            return hash;
        }

        #endregion

        #region Comparison semantics override

        public int CompareTo(ColumnLocation other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var nestingComp = _rowNesting.Count().CompareTo(other._rowNesting.Count());
            if (nestingComp != 0)
                return nestingComp;

            for (int i = 0; i < NestingLevel; i++)
            {
                var columnComp = StructuralComparisons.StructuralComparer.Compare(_columnNesting.Take(i + 1).ToArray(),
                    other._columnNesting.Take(i + 1).ToArray());
                if (columnComp != 0)
                    return columnComp;

                var rowComp = StructuralComparisons.StructuralComparer.Compare(_rowNesting.Take(i + 1).ToArray(),
                    other._rowNesting.Take(i + 1).ToArray());
                if (rowComp != 0)
                    return rowComp;
            }

            return 0; // Equal if loop finishes or has been skipped.
        }

        public int CompareTo(object obj) => CompareTo(obj as RowLocation);

        #endregion

        class PluggableInvertedComparer : IComparer<IColumnLocation>
        {
            public int Compare(IColumnLocation x, IColumnLocation y)
            {
                var underComparisonX = new ColumnLocation(x.RowNesting, x.ColumnNesting);
                var underComparisonY = new ColumnLocation(y.RowNesting, y.ColumnNesting);

                return underComparisonY.CompareTo(underComparisonX);
            }
        }
    }
}