using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    public class RowLocation : IRowLocation, IEquatable<RowLocation>, IComparable<RowLocation>, IComparable
    {
        readonly int[] _rowNesting;
        readonly int[] _columnNesting;

        /// <param name="rowNesting">An element index indicates the nesting level of row 
        /// and the value of the element is the row number in a column.</param>
        /// <param name="columnNesting">An element index indicates the nesting level of column
        /// and the value of the element is the column number in a row</param>
        public RowLocation(int[] rowNesting, int[] columnNesting)
        {
            if (rowNesting == null || columnNesting == null)
                throw new ArgumentNullException();

            if (rowNesting.Count() != columnNesting.Count() + 1)
                throw new ArgumentException($"{nameof(rowNesting)} must have one element more than {nameof(columnNesting)}.");

            _rowNesting = rowNesting.ToArray(); // Make a new copy of the array passed in.
            _columnNesting = columnNesting.ToArray();
        }

        /// <summary>
        /// Creates <see cref="RowLocation"/> from <see cref="ColumnLocation"/>.
        /// </summary>
        /// <param name="columnLocation"></param>
        /// <param name="rowNumber"></param>
        public RowLocation(ColumnLocation columnLocation, int rowNumber)
            : this(new List<int>(columnLocation.RowNesting) { rowNumber }.ToArray(),
                  columnLocation.ColumnNesting.ToArray()) { }

        RowLocation(RowLocation other, int rowNumber, int columnNumber)
            : this(new List<int>(other._rowNesting) { rowNumber }.ToArray(),
                  new List<int>(other._columnNesting) { columnNumber }.ToArray()) { }

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
        /// Returns the deepest row nesting level.
        /// </summary>
        public int NestingLevel => _rowNesting.Length;

        /// <summary>
        /// Creates <see cref="IRowLocation"/> with one nesting level more.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public IRowLocation Nest(int rowNumber, int columnNumber) => new RowLocation(this, rowNumber, columnNumber);

        /// <summary>
        /// Promote <see cref="IRowLocation"/> to <see cref="IColumnLocation"/>.
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public IColumnLocation Close(int columnNumber) =>
            new ColumnLocation(_rowNesting.ToArray(), new List<int>(_columnNesting) { columnNumber }.ToArray());

        /// <summary>
        /// Returns <see cref="IColumnLocation"/> made by trimming <see cref="IRowLocation.RowNesting"/> by 1 element.
        /// </summary>
        /// <returns></returns>
        public IColumnLocation Trim() => new ColumnLocation(_rowNesting.Take(NestingLevel - 1).ToArray(), _columnNesting);

        /// <summary>
        /// Helper comparer inverted with respect to the class <see cref="IComparable{T}"/> semantics.
        /// </summary>
        public static IComparer<IRowLocation> InvertedComparer => new PluggableInvertedComparer();

        #region Equality semantics override

        public bool Equals(RowLocation other)
        {
            if (other._rowNesting == null || other._columnNesting == null)
                return false;

            bool rowNumbersEqual = StructuralComparisons.StructuralEqualityComparer.Equals(_rowNesting, other.RowNesting);
            bool columnNumbersEqual = StructuralComparisons.StructuralEqualityComparer.Equals(_columnNesting, other.ColumnNesting);

            return rowNumbersEqual && columnNumbersEqual;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RowLocation location))
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

        public int CompareTo(RowLocation other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var nestingComp = _rowNesting.Count().CompareTo(other._rowNesting.Count());
            if (nestingComp != 0)
                return nestingComp;

            for (int i = 0; i < NestingLevel - 1; i++)
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

            return _rowNesting.Last().CompareTo(other._rowNesting.Last());
        }

        public int CompareTo(object obj) => CompareTo(obj as RowLocation);

        #endregion

        class PluggableInvertedComparer : IComparer<IRowLocation>
        {
            public int Compare(IRowLocation x, IRowLocation y)
            {
                var underComparisonX = new RowLocation(x.RowNesting, x.ColumnNesting);
                var underComparisonY = new RowLocation(y.RowNesting, y.ColumnNesting);

                return underComparisonY.CompareTo(underComparisonX);
            }
        }
    }
}