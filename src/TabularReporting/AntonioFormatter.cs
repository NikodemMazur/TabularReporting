using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    public class AntonioFormatter : IFormatter
    {
        char _leftColMargin = ' ';
        char _rightColMargin = ' ';
        char _colSeparator = '¦';
        char _rowSeparator = '-';
        char _linker = '+';
        char _fulfillment = ' ';
        int _maxColWidth = 128;
        int _maxColLinesCount = 2;
        readonly EndpointFormatter _endpointFormatter;

        /// <summary>
        /// Default is \s.
        /// </summary>
        public char LeftColMargin { get => _leftColMargin; set => _leftColMargin = value; }
        /// <summary>
        /// Default is \s.
        /// </summary>
        public char RightColMargin { get => _rightColMargin; set => _rightColMargin = value; }
        /// <summary>
        /// Default is ¦ (U+00A6).
        /// </summary>
        public char ColSeparator { get => _colSeparator; set => _colSeparator = value; }
        /// <summary>
        /// Default is -.
        /// </summary>
        public char RowSeparator { get => _rowSeparator; set => _rowSeparator = value; }
        /// <summary>
        /// Default is +.
        /// </summary>
        public char Linker { get => _linker; set => _linker = value; }
        /// <summary>
        /// Default is \s.
        /// </summary>
        public char Fulfillment { get => _fulfillment; set => _fulfillment = value; }
        /// <summary>
        /// Default is 128.
        /// </summary>
        public int MaxColWidth { get => _maxColWidth; set => _maxColWidth = value; }
        /// <summary>
        /// Default is 2.
        /// </summary>
        public int MaxColLinesCount { get => _maxColLinesCount; set => _maxColLinesCount = value; }

        public AntonioFormatter()
        {
            _endpointFormatter = new EndpointFormatter();
        }

        public string Format(IColumn report) => ColumnToString(report);

        /// <summary>
        /// Precalculates the width of all columns and returns a dictionary whose key is column nesting with siblings count
        /// and value is the maximum width found. This method is used to match the width of columns.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        internal IDictionary<Tuple<int[], int[]>, int> PrecalculateColumnWidths(IColumn column)
        {

            IEnumerable<IColumnLocation> colLocations = column.Process((cl, col) => cl).ToArray();

            var colLocSiblingsCountDict = new Dictionary<IColumnLocation, int[]>();
            column.ActOnRows(F);

            // Get all column nestings, siblings count and their width. If a column doesn't directly contain an endpoint, assume it has 0 width.
            // Note: only column nesting and siblings count are the key - therefore the column borders will be aligned throughout the report.
            IEnumerable<KeyValuePair<Tuple<int[], int[]>, int>> colNestSiblingsCountAndWidthPairs = 
                column.Process((cl, col) => 
                    new KeyValuePair<Tuple<int[], int[]>, int>(new Tuple<int[], int[]>(cl.ColumnNesting, colLocSiblingsCountDict[cl]),
                        col.Content.Extract(rows => 0, obj => _endpointFormatter.CalculateWidth(obj, _maxColWidth))));

            // Distinct by maximum width.
            var colNestSiblingsCountAndMaxWidthPairs =
                colNestSiblingsCountAndWidthPairs.GroupBy(kvp => kvp.Key, new TupleKeyEqualityComparer())
                                                 .Select(g => new KeyValuePair<Tuple<int[], int[]>, int>(new Tuple<int[], int[]>(g.Key.Item1, 
                                                    g.Key.Item2),
                                                    g.Max(kvp_ => kvp_.Value))).ToList();

            // Make dictionary.
            var colNestSiblingsCountAndMaxWidthDict = colNestSiblingsCountAndMaxWidthPairs.ToDictionary(new TupleKeyEqualityComparer());

            bool updated = default;
            do
            {
                updated = false; // Clear updated boolean at the beginning of every iteration.

                // Include contained columns in parent column width (including margins and separators).
                // Sort key value pairs by key (int[]) length descending.
                var colNestSiblingsCountAndMaxWidthPairsOrderedDesc = 
                    colNestSiblingsCountAndMaxWidthDict.OrderByDescending(kvp => kvp.Key.Item1.Length);
                foreach (var kvp in colNestSiblingsCountAndMaxWidthPairsOrderedDesc)
                {
                    var containedGroups = colNestSiblingsCountAndMaxWidthDict.Where(kvp_ => kvp_.Key.Item1.Length == kvp.Key.Item1.Length + 1 &&
                    StructuralEqualityComparer<int[]>.Default.Equals(kvp_.Key.Item1.Take(kvp.Key.Item1.Length).ToArray(), kvp.Key.Item1) &&
                    StructuralEqualityComparer<int[]>.Default.Equals(kvp_.Key.Item2.Take(kvp.Key.Item2.Length).ToArray(), kvp.Key.Item2)).ToArray()
                    .GroupBy(kvp_ => kvp_.Key.Item2, StructuralEqualityComparer<int[]>.Default);

                    if (containedGroups.Count() != 0)
                    {
                        foreach (var containedGroup in containedGroups) // For every columns group.
                        {
                            var contained = containedGroup.Select(cg => new KeyValuePair<Tuple<int[], int[]>, int>(cg.Key, 
                                                            colNestSiblingsCountAndMaxWidthDict[cg.Key]))
                                                          .ToArray(); // Always take up-to-date element.

                            var containedCount = contained.Count();

                            if (containedCount != 0) // Do nothing if there's no contained elements.
                            {
                                var acc = contained.Aggregate(0, (acc_, kvp_) => acc_ + kvp_.Value); // Sum up widths.

                                // 3 == column right margin + column separator (or linker) + column left margin.
                                var decorWidth = Math.Max(0, (containedCount - 1) * 3);

                                acc += decorWidth; // Add inter-column decoration width to accumulator.

                                if (colNestSiblingsCountAndMaxWidthDict[kvp.Key] < acc) // Update parent's width only if the greater one is found.
                                {
                                    colNestSiblingsCountAndMaxWidthDict[kvp.Key] = acc;

                                    updated = true;
                                }
                                else if (colNestSiblingsCountAndMaxWidthDict[kvp.Key] > acc) // Distribute extra width between containing columns.
                                {
                                    var extraWidth = colNestSiblingsCountAndMaxWidthDict[kvp.Key] - acc;
                                    int i = 0;
                                    while (extraWidth > 0)
                                    {
                                        colNestSiblingsCountAndMaxWidthDict[contained[i].Key]++;
                                        extraWidth--;
                                        i = (i + 1) % containedCount;
                                    }

                                    updated = true;
                                }
                            }
                        }
                    }
                }
            } while (updated);

            // Return the result as dictionary.
            return colNestSiblingsCountAndMaxWidthDict;

            void F(IRowLocation rl, IRow row)
            {
                if (rl.NestingLevel > 0)
                {
                    var parentLoc = rl.Trim();
                    var parentSiblingsCountArray = Array.Empty<int>();
                    if (parentLoc.NestingLevel != 0)
                        parentSiblingsCountArray = colLocSiblingsCountDict[parentLoc];
                    else
                        colLocSiblingsCountDict[ColumnLocation.Root] = Array.Empty<int>();

                    var colArray = row.Columns.ToArray();
                    var colCount = colArray.Count();
                    for (int i = 0; i < colCount; i++)
                        colLocSiblingsCountDict[rl.Close(i)] = parentSiblingsCountArray.Append(colCount).ToArray();
                }
            }
        }

        /// <summary>
        /// Precalculates the lines count of all rows and returns a dictionary whose key is Tuple of row nesting and row siblings count
        /// and value is the maximum lines count found. This method is used to match the lines count of rows.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        internal IDictionary<IRowLocation, int> PrecalculateRowHeights(IColumn column)
        {
            // Get all row locations.
            var locations = column.ProcessRows((l, row) => l);

            var locationLinesCountDict = MakeLocationLinesCountDict(column);

            var locationLinesCountDistributedDict = DistributeLines(locationLinesCountDict);

            return locationLinesCountDistributedDict;
        }

        /// <summary>
        /// <para>Makes dictionary whose key is row location and value is total lines count in the row.</para>
        /// The dictionary is made traversing <paramref name="column"/> from the most inner to the most outer column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        IDictionary<IRowLocation, int> MakeLocationLinesCountDict(IColumn column)
        {
            // Get all row locations and their lines count. If a row doesn't directly contain column with an endpoint, assume it has 0 height.
            var locationLinesCountSortedDict = 
                new SortedDictionary<IRowLocation, int>(column.ProcessRows(f).ToDictionary(), RowLocation.InvertedComparer);

            // Include contained rows in parent row lines count.
            var dictCount = locationLinesCountSortedDict.Count();
            for (int i = 0; i < dictCount; i++)
            {
                var currentKvp = locationLinesCountSortedDict.ElementAt(i);
                var nestingLevel = currentKvp.Key.NestingLevel;
                // Take all rows contained by the current kvp.
                var containedGroups = locationLinesCountSortedDict.Where(kvp_ => kvp_.Key.NestingLevel == nestingLevel + 1 &&
                                                                       StructuralEqualityComparer<int[]>.Default.Equals(
                                                                           kvp_.Key.RowNesting.Take(nestingLevel).ToArray(),
                                                                           currentKvp.Key.RowNesting))
                                                                  .GroupBy(kvp_ => kvp_.Key.ColumnNesting, StructuralEqualityComparer<int[]>.Default)
                                                                  .ToArray();

                // For each group sum up lines and pick the highest column.
                foreach (var g in containedGroups)
                {
                    var contained = g.ToArray();
                    var containedCount = contained.Count();

                    var acc = contained.Aggregate(0, (acc_, kvp_) => acc_ + kvp_.Value); // Accumulate lines count of each contained row.

                    acc += Math.Max(0, containedCount - 1); // Add lines for inter-row decoration.

                    locationLinesCountSortedDict[currentKvp.Key] =
                        Math.Max(locationLinesCountSortedDict[currentKvp.Key], acc); // Update only if the greater one is found.
                }
            }

            return locationLinesCountSortedDict;

            KeyValuePair<IRowLocation, int> f(IRowLocation rowLocation, IRow row)
            {
                var height = 0;

                foreach (var col in row.Columns)
                    height = Math.Max(height, col.Content.Extract(rows => 0, obj => _endpointFormatter.CalculateHeight(obj, _maxColLinesCount)));

                return new KeyValuePair<IRowLocation, int>(rowLocation, height);
            }
        }

        IDictionary<IRowLocation, int> DistributeLines(IDictionary<IRowLocation, int> dictToUpdate)
        {
            // Sort input by row nesting level ascending.
            var sortedDict = new SortedDictionary<IRowLocation, int>(dictToUpdate.ToDictionary());

            var dictCount = sortedDict.Count();
            for (int i = 0; i < dictCount; i++)
            {
                var currentKvp = sortedDict.ElementAt(i);
                var nestingLevel = currentKvp.Key.NestingLevel;
                var maxLines = currentKvp.Value; // Lines count of parent row.
                // Take all rows contained by the current kvp.
                var containedGroups = sortedDict.Where(kvp_ => kvp_.Key.NestingLevel == nestingLevel + 1 &&
                                                    StructuralEqualityComparer<int[]>.Default.Equals(
                                                        kvp_.Key.RowNesting.Take(nestingLevel).ToArray(),
                                                        currentKvp.Key.RowNesting))
                                                .GroupBy(kvp_ => kvp_.Key.ColumnNesting, StructuralEqualityComparer<int[]>.Default)
                                                .ToArray();

                // For each group, distribute extra lines.
                foreach (var g in containedGroups)
                {
                    var contained = g.ToArray();
                    var containedCount = contained.Count();

                    // Sum up the lines count of all rows in current (column) group. Include inter-row decorations.
                    var usedLines = contained.Aggregate(0, (acc, kvp) => acc + kvp.Value) + Math.Max(0, containedCount - 1);
                    var toDistribute = maxLines - usedLines; // Lines count to distribute.

                    for (int j = 0; j < toDistribute; j++)
                    {
                        var min = contained.Select(kvp => sortedDict[kvp.Key]).Min(); // Find minimum lines count within rows.
                        sortedDict[contained.Where(kvp => sortedDict[kvp.Key] == min).First().Key] += 1; // Distribute.
                    }
                }
            }

            return sortedDict;
        }

        string ColumnToString(IColumn column)
        {
            // Wrap into column with empty (dummy) rows/cols to apply the most outer decorations.
            var wrapper = WrapColumnToApplyOuterDecorations();

            // Prepare dictionaries with precalculated report widths/heights.
            var colNestWidthsDict = PrecalculateColumnWidths(wrapper);
            var rowHeightsDict = PrecalculateRowHeights(wrapper);

            // Call main method.
            var wrappedCol = ColumnToStringCore(wrapper, ColumnLocation.Root, Array.Empty<int>(), colNestWidthsDict, rowHeightsDict);

            // Remove byproducts of wrapper. 
            return TrimProtrudingDecorations();

            IColumn WrapColumnToApplyOuterDecorations()
            {
                var wrapper_ = 
                    new Column(
                        new Row(
                            new Column(string.Empty),
                            new Column(string.Empty),
                            new Column(string.Empty)),
                        new Row(
                            new Column(string.Empty),
                            column,
                            new Column(string.Empty)),
                        new Row(
                            new Column(string.Empty),
                            new Column(string.Empty),
                            new Column(string.Empty)));

                return wrapper_;
            }

            string TrimProtrudingDecorations()
            {
                var trimmedCol = wrappedCol.Trim('\r', // Trim new line delimiters and every report formatter special char but the linker.
                    '\n',
                    _leftColMargin,
                    _rightColMargin,
                    _colSeparator,
                    _rowSeparator,
                    _fulfillment);

                var sb = new StringBuilder();

                foreach (var line in trimmedCol.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                    sb.AppendLine(line.Trim('\r', // Trim new line delimiters and every special char but the linker and column separator.
                        '\n',
                        _leftColMargin,
                        _rightColMargin,
                        _rowSeparator,
                        _fulfillment));

                return sb.ToString().Trim(); // One more trim to remove new line at the end.
            }
        }

        string ColumnToStringCore(IColumn column, IColumnLocation columnLocation, int[] siblingsCount, 
            IDictionary<Tuple<int[], int[]>, int> colNestWidths, IDictionary<IRowLocation, int> rowHeights)
        {
            var sb = new StringBuilder();

            return column.Content.Extract(f, g);

            string f(IEnumerable<IRow> rows)
            {
                var rowsArray = rows.ToArray();
                var rowsCount = rowsArray.Count();

                for (int i = 0; i < rowsCount; i++)
                {
                    var colsArray = rowsArray[i].Columns.ToArray();
                    int colsCount = colsArray.Count();
                    var nestedCols = new List<string>();
                    var sCount = siblingsCount.Append(colsCount).ToArray();

                    for (int j = 0; j < colsCount; j++)
                    {
                        var cl = columnLocation.Nest(i, j); // Keep tracking the column nesting.
                        nestedCols.Add(ColumnToStringCore(colsArray[j], cl, sCount, colNestWidths, rowHeights));
                    }

                    var nextRowHasSameColArrangement = false;
                    var isLastRow = i == rowsCount - 1;
                    if (!isLastRow)
                        nextRowHasSameColArrangement = colsCount == rowsArray[i + 1].Columns.Count();
                    sb.AppendLine(ConcatenateColumns(nestedCols, nextRowHasSameColArrangement, !isLastRow));
                }
                return sb.ToString();
            }

            string g(object obj) => _endpointFormatter.Format(obj, colNestWidths[new Tuple<int[], int[]>(columnLocation.ColumnNesting, siblingsCount)],
                rowHeights[columnLocation.Open()], _fulfillment);
        }

        /// <summary>
        /// Concatenates block of strings (multiline strings) and adds inter-column decorations.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        string ConcatenateColumns(IEnumerable<string> columns, bool endWithDecorationWithLinker, bool decorate)
        {
            // Split lines into separate strings. Reject empty strings produced by splitting unless the original string was empty.
            var columnLinesArray = columns.Select(str => Regex.Split(str, Environment.NewLine)
                                                              .Where(str_ => !string.IsNullOrEmpty(str_) || Regex.Match(str, @"^\s*$").Success)
                                                              .ToArray())
                                          .ToArray();

            // Add line for decoration.
            if (decorate)
            {
                columnLinesArray = columnLinesArray.Select(linesArr => linesArr.ToList()
                                                                               .Append(new string(_rowSeparator, linesArr.First().Count()))
                                                                               .ToArray())
                                                   .ToArray();
            }

            // Check column strings consistency.
            var linesCounts = columnLinesArray.Select(lines => lines.Count());
            if (linesCounts.GroupBy(count => count).Count() > 1)
                throw new ArgumentException("Columns have different number of lines.", nameof(columns));
            var linesCount = linesCounts.First();

            // Initialize 1 StringBuilder per line.
            var lineStrBuilders = new StringBuilder[linesCount];
            for (int i = 0; i < linesCount; i++)
                lineStrBuilders[i] = new StringBuilder();

            var columnLinesArrayCount = columnLinesArray.Count();
            for (int i = 0; i < columnLinesArrayCount; i++) // For each column.
            {
                for (int j = 0; j < linesCount; j++) // For each line.
                {
                    // Append left side decorations.
                    if (i != 0) // If not first column.
                    {
                        // If inter-row decoration and previous column line is not empty.
                        if (LineIsInterRowDecorationOnLeft(columnLinesArray[i][j]))
                            lineStrBuilders[j].Append(_rowSeparator);
                        else // If concatenating column with different row arrangement.
                            lineStrBuilders[j].Append(_leftColMargin);
                    }

                    // Append actual column content.
                    lineStrBuilders[j].Append(columnLinesArray[i][j]);

                    // Append right side decorations (with inter-column decorations).
                    if (i != columnLinesArrayCount - 1) // If not last column.
                    {
                        if (LineIsInterRowDecorationOnRight(columnLinesArray[i][j])) // If inter-row decoration.
                        {
                            if (j == linesCount - 1) // If last line.
                            {
                                lineStrBuilders[j].Append(_rowSeparator);
                                if (endWithDecorationWithLinker)
                                    lineStrBuilders[j].Append(_linker);
                                else
                                    lineStrBuilders[j].Append(_rowSeparator);
                            }
                            else
                            {
                                lineStrBuilders[j].Append(_rowSeparator);
                                lineStrBuilders[j].Append(_colSeparator);
                            }
                        }
                        else // If concatenating column with different row arrangement.
                        {
                            lineStrBuilders[j].Append(_rightColMargin);
                            lineStrBuilders[j].Append(_colSeparator);
                        }
                    }
                }
            }

            var blockStrBuilder = new StringBuilder();
            for (int i = 0; i < linesCount; i++) // Merge all lines.
            {
                if (i != linesCount - 1)
                    blockStrBuilder.AppendLine(lineStrBuilders[i].ToString());
                else
                    blockStrBuilder.Append(lineStrBuilders[i].ToString());
            }

            return blockStrBuilder.ToString();

            bool LineIsInterRowDecorationOnLeft(string @string)
            {
                var pattern = "(?m)" + // Multiline mode.
                              "^" + // Match the start of a line.
                              $@"\{_rowSeparator}+?" + // Match all row separator chars...
                              $@"(?:\{_colSeparator}|\{_linker})" + // Until the end of a line.
                              $@"|^\{_rowSeparator}+?$"; // Or a line consists only row separators.
                var match = Regex.Match(@string, pattern);
                return match.Success;
            }

            bool LineIsInterRowDecorationOnRight(string @string)
            {
                var pattern = "(?m)" + // Multiline mode.
                              $@"(?:\{_colSeparator}|\{_linker})" + // Assert the start of inter-row decoration line.
                              $@"\{_rowSeparator}+?$" + // Match all row separator chars until the end of a line.
                              $@"|^\{_rowSeparator}+?$"; // Or a line consists only row separators.
                var match = Regex.Match(@string, pattern);
                return match.Success;
            }
        }

        #region Nested Classes
        /// <summary>
        /// Helper class that contains the endpoint formatting logic.
        /// </summary>
        public class EndpointFormatter
        {
            /// <summary>
            /// Calculates the endpoint height according to some rule.
            /// </summary>
            /// <param name="endpoint"></param>
            /// <returns></returns>
            public int CalculateHeight(object endpoint, int maxColLinesCount)
            {
                if (!(endpoint is string str))
                    throw new ArgumentException("Endpoint is not a string.", nameof(endpoint));
                if (string.IsNullOrEmpty(str))
                    return 1;
                var lines = str.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                return Math.Min(maxColLinesCount, lines.Count());
            }

            /// <summary>
            /// Calculates the endpoint width according to some rule.
            /// </summary>
            /// <param name="endpoint"></param>
            /// <returns></returns>
            public int CalculateWidth(object endpoint, int maxColWidth)
            {
                if (!(endpoint is string str))
                    throw new ArgumentException("Endpoint is not a string.", nameof(endpoint));
                if (string.IsNullOrEmpty(str))
                    return 1;
                var lines = str.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                var widestLineWidth = lines.Max(str_ => str_.Length);
                return Math.Min(maxColWidth, widestLineWidth);
            }

            /// <summary>
            /// Formats <paramref name="endpoint"/> into string according to some rule.
            /// </summary>
            /// <param name="endpoint"></param>
            /// <returns></returns>
            public string Format(object endpoint, int width, int linesCount, char fulfillment)
            {
                if (!(endpoint is string str))
                    throw new ArgumentException("Endpoint is not a string.", nameof(endpoint));
                if (string.IsNullOrEmpty(str))
                    str = fulfillment.ToString();
                var lines = str.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                               .ToArray();
                if (lines.Count() < linesCount)
                    lines = lines.Concat(Enumerable.Repeat(string.Empty, linesCount - lines.Count())).ToArray();
                else
                    lines = lines.Take(linesCount).ToArray();
                lines = lines.Select(l => l.Length > width ? 
                                string.Concat(string.Concat(l.Take(width - 3)), new string('.', 3)).PadRight(width) 
                                : l.PadRight(width))
                             .ToArray();
                return string.Join(Environment.NewLine, lines);
            }
        }

        /// <summary>
        /// Helper class for comparing <see cref="Tuple"/>&lt;int[], int&gt; instances.
        /// </summary>
        public class TupleKeyEqualityComparer : IEqualityComparer<Tuple<int[], int[]>>
        {
            public bool Equals(Tuple<int[], int[]> x, Tuple<int[], int[]> y)
            {
                if (x == null || y == null)
                    return false;
                return StructuralEqualityComparer<int[]>.Default.Equals(x.Item1, y.Item1) &&
                    StructuralEqualityComparer<int[]>.Default.Equals(x.Item2, y.Item2);
            }

            public int GetHashCode(Tuple<int[], int[]> obj)
            {
                // Josh Bloch's pattern.
                int hash = 17;
                hash = hash * 31 + StructuralEqualityComparer<int[]>.Default.GetHashCode(obj.Item1);
                hash = hash * 31 + StructuralEqualityComparer<int[]>.Default.GetHashCode(obj.Item2);
                return hash;
            }
        }
        #endregion
    }
}
