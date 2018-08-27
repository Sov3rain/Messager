namespace EventAggregation
{
    public class SimpleEvent : IEventBase { }

    public class IntEvent : IEventBase { public int Value { get; set; } } 
}