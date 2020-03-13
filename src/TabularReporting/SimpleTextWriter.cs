using TabularReporting.Abstractions;
using System.IO;

namespace TabularReporting
{
    public class SimpleTextWriter : IWriter
    {
        public string WriteReport(string report, string dir, string fileNameWithoutExtension)
        {
            var filePath = Path.Combine(dir, fileNameWithoutExtension + ".txt");
            using (FileStream fs = File.Create(filePath))
            using (TextWriter tw = new StreamWriter(fs))
                tw.Write(report);
            return filePath;
        }
    }
}