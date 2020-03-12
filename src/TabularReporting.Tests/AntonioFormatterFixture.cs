using TabularReporting.Abstractions;
using Xunit;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TabularReporting.Tests
{
    public class AntonioFormatterFixture
    {
        readonly IColumn _reportAsColumn;

        public AntonioFormatterFixture()
        {
            _reportAsColumn = new Column(
                new Row(
                    new Column($"Report{Environment.NewLine}   header.")),
                new Row(
                    new Column("Date"),
                    new Column("00.00.0000")),
                new Row(
                    new Column("Result"),
                    new Column("FAIL")),
                new Row(
                    new Column(
                        new Row(
                            new Column("a3x"),
                            new Column("b3x"),
                            new Column("c2")),
                        new Row(
                            new Column("d4xx"),
                            new Column("e2"),
                            new Column("f2")),
                        new Row(
                            new Column(
                                new Row(
                                    new Column("g2"),
                                    new Column("h6xxxx"),
                                    new Column("i6xxxx")),
                                new Row(
                                    new Column("j2"),
                                    new Column("k7xxxxx"),
                                    new Column("l7xxxxx")),
                                new Row(
                                    new Column("m2"),
                                    new Column("n3x"),
                                    new Column($"o3x{Environment.NewLine}#4xx"))),
                            new Column(
                                new Row(
                                    new Column("p")),
                                new Row(
                                    new Column("q")),
                                new Row(
                                    new Column("r"))),
                            new Column(
                                new Row(
                                    new Column($"s3x{Environment.NewLine}@2")),
                                new Row(
                                    new Column("t2")))),
                        new Row(
                            new Column(
                                new Row(
                                    new Column("u6xxxx"),
                                    new Column("v4xx")),
                                new Row(
                                    new Column("w5xxx"),
                                    new Column("x4xx"))),
                            new Column("y"),
                            new Column("z")))),
                new Row(
                    new Column(
                        new Row(
                            new Column("€9xxxxxxx"),
                            new Column("!3x")))));

            //+------------------------------------+
            //¦ Report                             ¦
            //¦    header.                         ¦
            //¦------------------------------------¦
            //¦ Date           ¦ 00.00.0000        ¦
            //¦----------------+-------------------¦
            //¦ Result         ¦ FAIL              ¦
            //¦------------------------------------¦
            //¦ a3x                    ¦ b3x ¦ c2  ¦
            //¦------------------------+-----+-----¦
            //¦ d4xx                   ¦ e2  ¦ f2  ¦
            //¦------------------------+-----+-----¦
            //¦ g2 ¦ h6xxxx  ¦ i6xxxx  ¦ p   ¦ s3x ¦
            //¦----+---------+---------¦     ¦ @2  ¦
            //¦ j2 ¦ k7xxxxx ¦ l7xxxxx ¦-----¦     ¦
            //¦----+---------+---------¦ q   ¦-----¦
            //¦ m2 ¦ n3x     ¦ o3x     ¦-----¦ t2  ¦
            //¦    ¦         ¦ #4xx    ¦ r   ¦     ¦
            //¦------------------------+-----+-----¦
            //¦ u6xxxx      ¦ v4xx     ¦ y   ¦ z   ¦
            //¦-------------+----------¦     ¦     ¦
            //¦ w5xxx       ¦ x4xx     ¦     ¦     ¦
            //¦------------------------------------¦
            //¦ €9xxxxxxx           ¦ !3x          ¦
            //+------------------------------------+
        }

        [Fact]
        public void PrecalculateColumnWidthsWorksCorrectly()
        {

            var sut = new SimpleFormatter();

            var actual = sut.PrecalculateColumnWidths(_reportAsColumn);
            var expected = new Dictionary<Tuple<int[], int[]>, int>(new SimpleFormatter.TupleKeyEqualityComparer())
            {
                {new Tuple<int[], int[]>(Array.Empty<int>(), Array.Empty<int>()), 34},
                {new Tuple<int[], int[]>(new[] { 0 },       new[] { 1 }), 34},
                {new Tuple<int[], int[]>(new[] { 0 },       new[] { 2 }), 14},
                {new Tuple<int[], int[]>(new[] { 1 },       new[] { 2 }), 17},
                {new Tuple<int[], int[]>(new[] { 0, 0 },    new[] { 1, 3 }), 22},
                {new Tuple<int[], int[]>(new[] { 0, 1 },    new[] { 1, 3 }), 3},
                {new Tuple<int[], int[]>(new[] { 0, 2 },    new[] { 1, 3 }), 3},
                {new Tuple<int[], int[]>(new[] { 0, 0, 0 }, new[] { 1, 3, 3 }), 2},
                {new Tuple<int[], int[]>(new[] { 0, 0, 1 }, new[] { 1, 3, 3 }), 7},
                {new Tuple<int[], int[]>(new[] { 0, 0, 2 }, new[] { 1, 3, 3 }), 7},
                {new Tuple<int[], int[]>(new[] { 0, 1, 0 }, new[] { 1, 3, 1 }), 3},
                {new Tuple<int[], int[]>(new[] { 0, 2, 0 }, new[] { 1, 3, 1 }), 3},
                {new Tuple<int[], int[]>(new[] { 0, 0, 0 }, new[] { 1, 3, 2 }), 11},
                {new Tuple<int[], int[]>(new[] { 0, 0, 1 }, new[] { 1, 3, 2 }), 8},
                {new Tuple<int[], int[]>(new[] { 0, 0 },    new[] { 1, 2 }), 19},
                {new Tuple<int[], int[]>(new[] { 0, 1 },    new[] { 1, 2 }), 12}
            };

            Assert.Equal(expected.Count(), actual.Count());

            foreach (var kvp in expected)
                Assert.Equal(kvp.Value, actual[kvp.Key]);
        }

        [Fact]
        public void PrecalculateRowHeightsWorksCorrectly()
        {
            var sut = new SimpleFormatter();

            var actual = sut.PrecalculateRowHeights(_reportAsColumn);

            var expected = new Dictionary<IRowLocation, int>()
            {
                { ColumnLocation.Root.NestOpen(0),                          2 },
                { ColumnLocation.Root.NestOpen(1),                          1 },
                { ColumnLocation.Root.NestOpen(2),                          1 },
                { ColumnLocation.Root.NestOpen(3),                          14 },
                { ColumnLocation.Root.NestOpen(4),                          1 },
                { ColumnLocation.Root.NestOpen(3).Nest(0, 0),               1 },
                { ColumnLocation.Root.NestOpen(3).Nest(1, 0),               1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0),               6 },
                { ColumnLocation.Root.NestOpen(3).Nest(3, 0),               3 },
                { ColumnLocation.Root.NestOpen(4).Nest(0, 0),               1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(0, 0),    1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(1, 0),    1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(2, 0),    2 },
                { ColumnLocation.Root.NestOpen(3).Nest(3, 0).Nest(0, 0),    1 },
                { ColumnLocation.Root.NestOpen(3).Nest(3, 0).Nest(1, 0),    1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(0, 1),    2 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(1, 1),    1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(2, 1),    1 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(0, 2),    3 },
                { ColumnLocation.Root.NestOpen(3).Nest(2, 0).Nest(1, 2),    2 }
            };

            Assert.Equal(expected.Count, actual.Count);

            foreach (var kvp in expected)
                Assert.Equal(kvp.Value, actual[kvp.Key]);
        }

        [Theory]
        [InlineData("FirstLine\r\nSecondLineTooLong\r\nThirdLine.", 2, 10, 
            "FirstLine \r\nSecondL...", 2, 10)]
        [InlineData("FirstLine\r\nSecondLineTooLong\r\nThirdLine.", 1, 3, 
            "...", 1, 3)]
        [InlineData("FirstLine\r\nSecondLineTooLong\r\nThirdLine.", 2, 17, 
            "FirstLine        \r\nSecondLineTooLong", 2, 17)]
        [InlineData("FirstLine\r\nSecondLineTooLong\r\nThirdLine.", 3, 17, 
            "FirstLine        \r\nSecondLineTooLong\r\nThirdLine.       ", 3, 17)]
        [InlineData("FirstLine\r\nSecondLineTooLong\r\nThirdLine.", 2, 100, 
            "FirstLine        \r\nSecondLineTooLong", 2, 17)]
        [InlineData("FirstLine\r\nSecondLineTooLong\r\nThirdLine.", 10, 100, 
            "FirstLine        \r\nSecondLineTooLong\r\nThirdLine.       ", 3, 17)]
        [InlineData("OnlyLine", 2, 4,
            "O...", 1, 4)]
        [InlineData("OnlyLine", 1, 100,
            "OnlyLine", 1, 8)]
        [InlineData("", 1, 100, 
            " ", 1, 1)]
        public void EndpointFormatterMethodsReturnConsistentResults(string sample, int maxLinesCount, int maxColWidth, 
            string expectedStr, int expectedLinesCount, int expectedColWidth)
        {
            var sut = new SimpleFormatter.EndpointFormatter();
            
            var actualMaxLinesCount = sut.CalculateHeight(sample, maxLinesCount);
            var actualMaxColWidth = sut.CalculateWidth(sample, maxColWidth);
            var actualEndpoint = sut.Format(sample, actualMaxColWidth, actualMaxLinesCount, ' ');

            Assert.Equal(expectedLinesCount, actualMaxLinesCount);
            Assert.Equal(expectedColWidth, actualMaxColWidth);
            Assert.Equal(expectedStr, actualEndpoint);
        }

        [Fact]
        public void FormatWorksCorrectly()
        {
            var sut = new SimpleFormatter() { MaxColLinesCount = 2 };

            var actual = sut.Format(_reportAsColumn);

            Assert.Equal(ExpectedTestResults.ExpectedAntonioReport, actual);
        }
    }
}