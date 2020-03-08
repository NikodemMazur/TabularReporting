namespace TabularReporting.Abstractions
{
    public interface IReporter<T>
    {
        IColumn Report(T source, IColumnQuery reportQuery);
    }
}
