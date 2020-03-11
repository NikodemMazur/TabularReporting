using TabularReporting.Abstractions;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.Text.RegularExpressions;

namespace TabularReporting.Tests
{
    public class ColumnFixture
    {
        readonly object[] _expectedEndPoints;
        readonly Column _column;

        public ColumnFixture()
        {
            _expectedEndPoints = new object[] { "Endpoint 0", "Endpoint 1", "Endpoint 2", "Endpoint 3", "Endpoint 4" };
            _column = new Column(
                new Row(
                    new Column(_expectedEndPoints[0]),
                    new Column(_expectedEndPoints[1]),
                    new Column(_expectedEndPoints[2]),
                    new Column(new Row(
                        new Column(_expectedEndPoints[3]),
                        new Column(_expectedEndPoints[4])))));
        }

        [Fact]
        public void NestingWorksCorrectly()
        {
            int actual = _column.NestingLevel;

            Assert.Equal(2, actual);
        }

        [Fact]
        public void ProcessEndPointsReturnsColumnNestingAndWidthPairs()
        {
            var actual = _column.ProcessEndpoints((l, ep) => new KeyValuePair<int[], int>(l.ColumnNesting, ep.ToString().Length)).ToArray();
            var expected = new Dictionary<int[], int>
            {
                { new[] { 0 }, 10 },
                { new[] { 1 }, 10 },
                { new[] { 2 }, 10 },
                { new[] { 3, 0 }, 10 },
                { new[] { 3, 1 }, 10 }
            };

            Assert.Equal(expected.Keys,
                actual.Select(kvp => kvp.Key).ToArray(),
                StructuralEqualityComparer<int[]>.Default); // arrays equal
            Assert.Equal(expected.Values,
                actual.Select(kvp => kvp.Value).ToArray()); // widths equal
        }

        [Fact]
        public void ProcessReturnsLocationOfEveryColumn()
        {
            var actual = _column.Process((l, content) => l);
            var expected = new[]
            {
                ColumnLocation.Root,
                ColumnLocation.Root.Nest(0, 0),
                ColumnLocation.Root.Nest(0, 1),
                ColumnLocation.Root.Nest(0, 2),
                ColumnLocation.Root.Nest(0, 3),
                ColumnLocation.Root.Nest(0, 3).Nest(0, 0),
                ColumnLocation.Root.Nest(0, 3).Nest(0, 1)
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ProcessRowsReturnsLocationOfEveryRow()
        {
            var actual = _column.ProcessRows((l, row) => l);
            var expected = new[]
            {
                ColumnLocation.Root.NestOpen(0),
                ColumnLocation.Root.Nest(0, 3).NestOpen(0)
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ActIsAssertAtTheSameTime()
        {
            int i = 0;
            _column.Act((cl, col) =>
                Assert.True(col.Content.Extract(rows => true, obj => Regex.Match(obj as string, $"Endpoint {i++}").Success)));
        }
    }
}