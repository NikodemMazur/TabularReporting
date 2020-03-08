using System.Linq;
using System.Xml.Linq;
using TabularReporting.Abstractions;

namespace TabularReporting
{
    public static class ColumnExtensions
    {
        public static XNode ToXml(this IColumn source)
        {
            var content = source.Content.Extract(rows => rows.Select(r => r.ToXml()), obj => new[] { new XText(obj.ToString()) });
            return new XElement(nameof(Column), content);
        }
    }
}
