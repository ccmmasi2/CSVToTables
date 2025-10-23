using System.Text;

namespace ReadCSVToTables
{
    public class ReadCSVToCreateTable
    {
        public static void Main(string[] args)
        {
            string csvPath = @"C:\scripts\tablas.csv";

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
                sb.AppendLine($"-- ========================================");
                sb.AppendLine($"-- SCHEMA: {table.Key.Schema} | TABLE: {table.Key.Table}");
                sb.AppendLine($"-- ========================================");
                sb.AppendLine($"CREATE SCHEMA IF NOT EXISTS [{table.Key.Schema}];");
                sb.AppendLine("GO");
                sb.AppendLine();

                sb.AppendLine($"CREATE TABLE [{table.Key.Schema}].[{table.Key.Table}] (");

                var cols = table.ToList();

                for (int i = 0; i < cols.Count; i++)
                {
                    var col = cols[i];
                    string lengthPart = "";

                    if (!string.Equals(col.Length, "NULL", StringComparison.OrdinalIgnoreCase) &&
                        (col.DataType.Contains("char") || col.DataType.Contains("binary")))
                    {
                        lengthPart = $"({col.Length})";
                    }

                    string nullPart = col.IsNullable == "NO" ? "NOT NULL" : "NULL";
                    string comma = (i < cols.Count - 1) ? "," : "";

                    sb.AppendLine($"    [{col.Column}] {col.DataType}{lengthPart} {nullPart}{comma}");
                }

                sb.AppendLine(");");
                sb.AppendLine("GO");
                sb.AppendLine();
            }

            string outputPath = Path.Combine(Path.GetDirectoryName(csvPath)!, "CreateTables.sql");
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);

            Console.WriteLine("✅ Script generado correctamente:");
            Console.WriteLine(outputPath);
        }
    }
}
