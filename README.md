# Event Aggregator

![version](https://img.shields.io/github/v/tag/Sov3rain/Unity-Event-Aggregator?label=latest) ![unity-version](https://img.shields.io/badge/unity-2019.4%2B-lightgrey)

This implementation of the Event Aggregator pattern tries to overcome the limitations of traditional event handling by providing a central place to publish and subscribe for events. It takes care of registering, unregistering and invoking events and thus decoupling publishers and subscribers.

## Installation

Add this dependency to the `dependencies` section of your `Packages/manifest.json`:

##### Latest:
```
"com.soverain.event-aggregator": "https://github.com/Sov3rain/Unity-Event-Aggregator.git#upm"
```

##### Specific version:
```
"com.soverain.event-aggregator": "https://github.com/Sov3rain/Unity-Event-Aggregator.git#v1.2.3"
```

You can also add it via the Package Manager window under `Add -> Add package from git URL...` and paste in the url part above.

## Usage
### Create an event

Events are just plain C# classes that implements the `IEvent` interface.

```csharp
using EventAggregation;

public class OnTestEvent : IEvent { } 

public class OnTestEventWithData : IEvent 
{ 
    public int number;
}
```

### Get a reference of the Event Aggregator

You can get a direct reference in any part of your code by accessing the default global static instance, or instantiate your own.

```csharp
// Direct reference.
var eventAggregator = EventAggregator.DefaultInstance;

// Custom instance.
public EventAggregator eventAggregator = new EventAggregator();
```

### Dispatch an event

You can dispatch an event by using the `Dispatch` method. You can dispatch an event with or without data.

```csharp
// Without data.
eventAggregator.Dispatch<OnTestEvent>();

// With data.
eventAggregator.Dispatch(new OnTestEventWithData { number = 42 });
```

### Add and remove listeners

You can add or remove listeners by using the given methods. Remember to always remove a listener upon object destruction to avoid memory leaks and exceptions.

If you want to use the data from an event, you need to cast the `IEvent` parameter to its type.

```csharp
// Listener.cs

private EventAggregator eventAggregator;

void Start()
{
    eventAggregator = EventAggregator.DefaultInstance;
    eventAggregator.AddListener<OnTestEvent>(OnTestEventHandler);
    eventAggregator.AddListener<OnTestEventWithData>(OnTestEventWithDataHandler);
}

void OnDestroy()
{
    eventAggregator.RemoveListener<OnTestEvent>(OnTestEventHandler);
    eventAggregator.RemoveListener<OnTestEventWithData>(OnTestEventWithDataHandler);
}

void OnTestEventHandler(IEvent eventData)
{
    Debug.Log("Hello, World!");
}

void OnTestEventWithDataHandler(IEvent eventData)
{
    var data = evenData as OnTestEventWithData;
    Debug.Log("Hello, World! " + data.number);
}
```
