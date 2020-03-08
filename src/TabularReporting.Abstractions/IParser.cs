namespace TabularReporting.Abstractions
{
    public interface IParser
    {
        IColumn Parse(string report);
    }
}
