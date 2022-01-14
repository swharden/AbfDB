using System;

namespace AbfDB.IndexedSearch;

/// <summary>
/// Represents ABF file data obtained from an indexed search
/// </summary>
public record IndexedAbf
{
    public string Path { get; init; } = string.Empty;
    public DateTime Modified { get; init; } = DateTime.MinValue;
    public int SizeBytes { get; init; } = -1;
}