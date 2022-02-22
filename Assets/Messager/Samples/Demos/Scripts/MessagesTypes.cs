public sealed class SIMPLE_MESSAGE
{
    public int Count;
}

public struct OTHER_MESSAGE
{
    public int Count { get; }

    public OTHER_MESSAGE(int count)
    {
        Count = count;
    }
}