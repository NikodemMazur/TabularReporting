namespace TabularReporting.Abstractions
{
    public interface ISourcedRowQuery<T> : IRowQuery
    {
        T Source { get; set; }
    }
}