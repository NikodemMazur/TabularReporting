using TabularReporting.Abstractions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace TabularReporting.Tests
{
    public class ColumnExtensionsFixture
    {
        [Fact]
        public void ColumnToXmlContainsRowsToXmlResult()
        {
            XNode[] expected = new XNode[3];
            Mock<IRow>[] mockRows = new Mock<IRow>[3];
            Mock<IColumn>[] mockCols = new Mock<IColumn>[3];
            for (int i = 0; i < 3; i++)
            {
                var mockedContent = $"mockedContent[{i}]";
                expected[i] = new XElement($"Row", new XElement("Column", mockedContent));
                mockCols[i] = new Mock<IColumn>();
                mockCols[i].Setup(c => c.Content).Returns(new Union2<IEnumerable<IRow>, object>.Case2(mockedContent));
                mockRows[i] = new Mock<IRow>();
                mockRows[i].Setup(r => r.Columns).Returns(new[] { mockCols[i].Object });
            }
            Column sut = new Column(new Union2<IEnumerable<IRow>, object>.Case1(mockRows.Select(mr => mr.Object).ToArray()));

            XElement actualXElement = (XElement)sut.ToXml();
            IEnumerable<XElement> actual = actualXElement.Elements();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.True(expected.Zip(actual, (e, a) => e.ToString() == a.ToString()).All(b => b));
        }
    }
}