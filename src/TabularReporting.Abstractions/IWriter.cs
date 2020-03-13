namespace TabularReporting.Abstractions
{
    public interface IWriter
    {
        string WriteReport(string report, string dir, string fileNameWithoutExtension);
    }
}
