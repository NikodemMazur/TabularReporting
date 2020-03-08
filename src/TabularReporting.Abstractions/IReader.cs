namespace TabularReporting.Abstractions
{
    public interface IReader
    {
        string ReadReport(string filePath);
    }
}
