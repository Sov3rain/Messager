# Unity-Event-Aggregator

## About

Simple .net event aggregator for Unity engine. 

## Where to use it

In every project that is event heavy or when you need to decouple your publishers from your subscibers.

## How to use it

import the namespace:  
`using EventAggregation;`

You can get access to the EventAggregator at anytime by just typing `EventAggregator`.

### 1. Creating a new event

When you need to publish an event:  
```C#
EventAggregator.Publish(MyEventType())
```

### 2. Subscribe to an event

In any part of your script or in Unity's callbacks (Start, Awake, et...):  
```C#
EventAggregator.Subscribe<MyEventType>(MyMethodName)
```

### 3. Custom event arguments

Events are defined by a custom class, which can hold additional data. The class needs to implement the `IEventBase` interface. You can extend, inherit and derive from this class as you want as long as it implements the interface.

#### a. Create your own class

```C#
public class MyEventType : MyEventTypeBase, IEventBase
{
    public int Index { get; set; }
    public Coordinates Coordinates { get; set; }
}

public abstract class MyEventTypeBase
{
    public List<int> Numbers {get; set; }
}
```

#### b. Passing arguments

```C#
var myType = new MyEventType() 
{
    Numbers = new List<int>() { 0, 1, 2 },
    Index = 42,
    Coordinates = new Coordinates()
}
EventAggregator.Publish(myType);
```

#### c. Get the data
```C#
private void MyMethodName(IEventData eventData)
{
    var arg = eventData as MyEventType;
    if (arg != null)
    {
        // Do whatever you want with your data
    }
}
```