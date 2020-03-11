using TabularReporting.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace TabularReporting.Tests
{
    public class RowExtensionsFixture
    {
        [Fact]
        public void RowToXmlContainsColumnsToXmlResult()
        {
            var expected = new List<XElement>();
            Column[] cols = new Column[3];
            for (int i = 0; i < 3; i++)
            {
                cols[i] = new Column(new Union2<IEnumerable<IRow>, object>.Case2($"Content of Column[{i}]"));
                expected.Add((XElement)cols[i].ToXml());
            }

            Row sut = new Row(cols);

            XElement actualXElement = (XElement)sut.ToXml();
            IEnumerable<XElement> actual = actualXElement.Elements();

            Assert.Equal(expected.Count(), actual.Count());
            Assert.True(expected.Zip(actual, (e, a) => e.ToString() == a.ToString()).All(b => b));
        }
    }
}