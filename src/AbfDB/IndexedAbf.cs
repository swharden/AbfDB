using System;

namespace AbfDB
{
    public record IndexedAbf
    {
        public string Path { get; init; } = string.Empty;
        public DateTime Modified { get; init; } = DateTime.MinValue;
        public int SizeBytes { get; init; } = -1;
        public bool HasRSV { get; init; } = false;
    }
}
