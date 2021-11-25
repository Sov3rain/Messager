public sealed class SIMPLE_MESSAGE
{
    public int Count;
}

public readonly struct OTHER_MESSAGE
{
    public readonly int count;

    public OTHER_MESSAGE(int count)
    {
        this.count = count;
    }
}