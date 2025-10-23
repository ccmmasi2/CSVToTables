using System.Text;

namespace ReadCSVToTables
{
    public class ReadCSVToSelectTable
    {
        public static void Main(string[] args)
        {
            string csvPath = @"D:\\Repository\\Study\\columns.csv";

            if (!File.Exists(csvPath))
            {
                Console.WriteLine("No se encontró el archivo CSV: " + csvPath);
                return;
            }

            var lines = File.ReadAllLines(csvPath)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .ToList();

            var columns = new List<TableColumn>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length < 6)
                    continue;

                columns.Add(new TableColumn
                {
                    Schema = parts[0].Trim(),
                    Table = parts[1].Trim(),
                    Column = parts[2].Trim(),
                    DataType = parts[3].Trim(),
                    Length = parts[4].Trim(),
                    IsNullable = parts[5].Trim().ToUpper()
                });
            }

            var tables = columns
                .GroupBy(c => new { c.Schema, c.Table })
                .OrderBy(g => g.Key.Schema)
                .ThenBy(g => g.Key.Table);

            var sb = new StringBuilder();

            foreach (var table in tables)
            {
                var cols = table.ToList();
                string colNames = "";

                for (int i = 0; i < cols.Count; i++)
                {
                    var col = cols[i];
                    string comma = (i < cols.Count - 1) ? "," : "";

                    colNames += $"{col.Column}{comma} ";
                }

                sb.AppendLine($"SELECT {colNames} FROM [{table.Key.Schema}].[{table.Key.Table}];");
            }

            string outputPath = Path.Combine(Path.GetDirectoryName(csvPath)!, "CreateSelectToTables.sql");
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);

            Console.WriteLine("✅ Script generado correctamente:");
            Console.WriteLine(outputPath);
        }
    }
}
