namespace TabularReporting.Abstractions
{
    public interface IColumnLocation
    {
        int[] RowNesting { get; }
        int[] ColumnNesting { get; }
        int NestingLevel { get; }

        bool IsRoot { get; }

        IColumnLocation Nest(int rowNumber, int columnNumber);
        IRowLocation NestOpen(int rowNumber);
        IRowLocation Open();
        IColumnLocation Trim();
    }
}