using TabularReporting.Abstractions;
using System.IO;

namespace TabularReporting
{
    public class SimpleTextReader : IReader
    {
        public string ReadReport(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            using (TextReader tr = new StreamReader(fs))
                return tr.ReadToEnd();
        }
    }
}