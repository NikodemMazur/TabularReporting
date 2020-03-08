using System.Linq;
using System.Xml.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    public static class RowExtensions
    {
        public static XNode ToXml(this IRow source)
        {
            return new XElement(nameof(Row), source.Columns.Select(c => c.ToXml()));
        }
    }
}
