# Messager

![npm](https://img.shields.io/npm/v/com.sov3rain.messager) ![unity-version](https://img.shields.io/badge/unity-2019.4%2B-lightgrey)

This implementation of the event aggregator pattern tries to overcome the limitations of traditional event handling by providing a central place to publish and subscribe for events. It takes care of registering, unregistering and invoking events and thus decoupling publishers and subscribers.

## Installation

This package uses Unity's [scoped registry](https://docs.unity3d.com/Manual/upm-scoped.html) feature to import packages and is hosted [here](https://www.npmjs.com/package/com.sov3rain.messager). Add the following sections to the package manifest file located in `Packages/manifest.json`.

Add this object to the `scopedRegistries` section:

```
{
  "name": "sov3rain",
  "url": "https://registry.npmjs.com",
  "scopes": [ "com.sov3rain" ]
}
```

Add  this line to the `dependencies` section:

```
"com.sov3rain.messager": "1.0.0"
```

Your manifest file should look like this now:

```
{
  "scopedRegistries": [
    {
      "name": "sov3rain",
      "url": "https://registry.npmjs.com",
      "scopes": [ "com.sov3rain" ]
	}
  ],
  "dependencies": {
    "com.sov3rain.messager": "1.0.0",
    ...
  }
}
```

## Usage
### Create a message

Messages are just plain C# objects. They can be classes or structs. They serve the purpose of creating a unique signature by enforcing Type reference instead of string reference.

```csharp
public struct SIMPLE_MESSAGE
{
    public int Number;
}
```

### Referencing the messager

You can get a direct reference in any part of your code by accessing the default static instance:

```csharp
private readonly Messenger _messager = Messager.DefaultInstance;
```

> Note: instantiating your own Messager class could help unit testing or mocking, but for regular use, it's recommended to use the default static instance.
>
> `````csharp
> // Custom instance.
> private readonly Messenger _myMessager = new Messager();
> `````

### Dispatch a message

You can dispatch a new message by using the `Dispatch()` method.

```csharp
_messager.Dispatch(new SIMPLE_MESSAGE());
```

The message type is inferred by the class instance you pass as an argument.

### Start listening for a message

Anywhere in any class, you can listen for a message by using the `Listen<T>()` method. The type you want to listen for is specified through the generic type parameter.

```c#
_messager.Listen<SIMPLE_MESSAGE>(
    owner: this, 
    handler: msg => Debug.Log($"Message incoming: {msg.Number}"); 
);
```

> Note: You can register anonymous functions safely because they are bonded to the instance registering (hence the `owner` parameter) so they can be removed later.

### Stop listening for a message

When you want to stop being notified when a type of message is dispatched, you can call `Cut()` to unregister the object:

```c#
_messager.Cut<SIMPLE_MESSAGE>(this);
```

> Note: when using `Cut`, all listeners registered for that object are removed, as they are referenced by owner.
> Note 2: remember to always call `Cut()` before destroying an object instance that is still listening to messages.

## Good Practices

**DO** name your message objects in a `ALL_UPPER` style to differentiate them from your other classes and structs.

`````csharp
// Bad.
public class SimpleMessage { }

// Good.
public class SIMPLE_MESSAGE { }
`````

**DO** use immutable objects as messages to avoid side effects:

```csharp
public sealed class IMMUTABLE_MESSAGE
{
    public int Number { get; }

    public ImmutableMessage(int number)
    {
        Number = number;
    }
}

public readonly struct IMMUTABLE_STRUCT_MESSAGE
{
    public readonly int Number;

    public ImmutableStructMessage(int number)
    {
        Number = number;
    }
}
```

**DO** cache the message instance if you plan on reusing it multiple times to avoid unnecessary memory allocations:

```csharp
var _msg = new SIMPLE_MESSAGE { Number = 42 };

// Example code, don't do that.
void Update()
{
    _messager.Dispatch(this, _msg);
}
```

## Devtools

Messager comes with a set of devtools to let you better track down message subscriptions and have a timed history of published messages. Devtools are never shipped out in build, you can **only use them in the Editor**.

### Enable devtools

By default devtools are disabled. You can toggle them with the menu bar under `Messager/Enable Devtools`.

### Devtools Window

You can open the devtools window with the menu bar under `Messager/Open Devtools Window`. You can dock the window or use it in floating mode. The window itself has two tabs:

#### Message history

When you dispatch a message in you application code, a timed record will be created and displayed in the history.

![histo-tab](https://i.ibb.co/rkgv6yT/history-tab.jpg)

From here you can see the the type of message, the message timecode (from application start). Will be displayed under the foldout the type of the caller and the calling method, and if the message contains data the json serialiazed payload.

You can filter the history by message type. Names are case sensitive.

#### Subscribers

Available at runtime only, it will show each type of message used and all the subscribers listening to that particular type of message underneath. The number of subscribers will be shown between parentheses. If the subscriber is of type `UnityEngine.Object`, you will be able to click the object field to select the instance in the scene. If the subscriber is not of type `UnityEngine.Object`, you will just see the instance type name.

> Note: if a message has no subscribers at some point, it will not be shown.

![subs-tab](https://i.ibb.co/P5nFPnt/subscribers-tab.jpg)

 You can also filter the list by message type via the search field. Names are case sensitive.

## API Reference

### Messager Class

#### Properties

| Name                          | Description                              |
| ----------------------------- | ---------------------------------------- |
| `(Messsager) DefaultInstance` | Returns the `Messager` default instance. |

#### Methods

| Name                                                         | Description                                                  |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| `Use(Middleware onDispatch, Middleware onLister, Middleware onCut)` | Initialize middlewares. You can register a delegate matching the following signature: `(Type type, object owner, Action next)` where next is the next function in the delegate. Devtools use this middlewares to get notified about dispatch, listen and cut. |
| `Listen<T>(object owner, Action<T> handler)`                 | Registers a handler. The type you want to listen for must be passed in the generic parameter. |
| `Cut<T>(object owner)`                                       | Remove all handlers owned by the `owner` object for the specified type. |
| `Dispatch<T>(T payload)`                                     | Dispatch a new message. The type can be inferred from the instance passed as the `payload` parameter. |

### Messager.Subscription Class

#### Properties

| Name                      | Description                                                                              |
| ------------------------- | ---------------------------------------------------------------------------------------- |
| `(object) Owner`          | The reference of the object owning this message.                                         |
| `(Action<object>) Action` | The registered callback. Will be invoked when a new message type matching is dispatched. |

#### Constructors

| Name                                                | Description                                                |
| --------------------------------------------------- | ---------------------------------------------------------- |
| `Subscription(object owner, Action<object> action)` | Default constructor. Sets the owner and action properties. |

