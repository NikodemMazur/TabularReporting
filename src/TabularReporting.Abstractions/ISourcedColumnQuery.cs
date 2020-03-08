namespace TabularReporting.Abstractions
{
    public interface ISourcedColumnQuery<T> : IColumnQuery
    {
        T Source { get; set; }
    }
}