using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbfDB.Database;

internal class TableBuilder
{
    readonly string Name;

    readonly List<string> ColumnQueryLines = new();

    public TableBuilder(string name = "Abfs")
    {
        Name = name;
    }

    public void AddColumn(string name, ColumnType type, string modifiers = "")
    {
        ColumnQueryLines.Add($"[{name}] {type} {modifiers}".Trim());
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"CREATE TABLE IF NOT EXISTS {Name}");
        sb.AppendLine("(");
        foreach (string line in ColumnQueryLines)
        {
            sb.Append($"  {line}");
            if (line != ColumnQueryLines.Last())
                sb.Append(',');
            sb.Append(Environment.NewLine);
        }
        sb.Append(");");
        return sb.ToString();
    }
}