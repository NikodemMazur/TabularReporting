namespace TabularReporting.Abstractions
{
    public interface IRowLocation
    {
        int[] RowNesting { get; }
        int[] ColumnNesting { get; }
        int NestingLevel { get; }

        IRowLocation Nest(int rowNumber, int columnNumber);
        IColumnLocation Close(int columnNumber);
        IColumnLocation Trim();
    }
}