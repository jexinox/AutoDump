namespace Guts.Analyzer;

public record struct Size(ulong Value, Unit Unit)
{
    public static implicit operator Size(ulong value)
    {
        return new Size(value, Unit.Byte);
    }
}

public enum Unit
{
    Byte,
    Kilobyte,
    Megabyte,
    Gigabyte,
}