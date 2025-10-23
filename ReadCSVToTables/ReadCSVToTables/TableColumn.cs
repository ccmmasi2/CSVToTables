namespace ReadCSVToTables
{
    public class TableColumn
    {
        public string Schema { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        public string IsNullable { get; set; }
    }
}
