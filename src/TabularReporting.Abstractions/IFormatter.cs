namespace TabularReporting.Abstractions
{
    public interface IFormatter
    {
        string Format(IColumn report);
    }
}
