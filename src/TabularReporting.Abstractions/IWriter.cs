namespace TabularReporting.Abstractions
{
    public interface IWriter
    {
        void WriteReport(string report, string dir, string fileNameWithoutExtension);
    }
}
