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

Messages are just plain C# objects. They can be classes or structs. They serve the purpose of creating a unique signature by enforcing Type reference instead of string reference. They can also hold data.

```csharp
// Prefer using structs as message containers.
public struct MyMessage
{
    public int Number;
}
```

### Referencing the messager

You can get a direct reference in any part of your code by accessing the default static instance, or instantiate your own.

```csharp
using Messenging;

// Default static instance
private readonly Messenger _messager = Messager.DefaultInstance;

// Custom instance.
private readonly Messenger _myMessager = new Messager();
```

> Note: instantiating your own Messager class could help unit testing or mocking, but for regular use, it's recommended to use the default static instance.

### Dispatch a message

You can dispatch a new message by using the `Dispatch()` method.

```csharp
Messager.Dispatch(new MyMessage());
```

The message type is inferred by the class instance you create and pass as an argument.

### Start listening for a message

Anywhere in any class, you can listen for a message by using the `Listen<T>()` method. The type you want to listen for is specified through the generic type parameter.

`````c#
Messager.Listen<MyMessage>(this, msg => {
   Debug.Log("Message incoming!"); 
});
`````

> Note: You can register anonymous functions safely. Because they are bonded to the instance registering (hence the `owner` parameter), they can be destroyed later.

### Stop listening for a message

When you want to stop being notified when a type of message is dispatched, you can call `Cut()` to unregister the object.

`````c#
Messager.Cut<MyMessage>(this);
`````

> Note: when using `Cut`, all listeners registered for that object are removed, as they are referenced by owner.
> Note 2: remember to always call `Cut()` before destroying an object instance that is still listening to messages.

## Good Practices

**DO** use immutable objects as messages to avoid side effects:

```csharp
public sealed class ImmutableMessage
{
    public int Number { get; }

    public ImmutableMessage(int number)
    {
        Number = number;
    }
}

public readonly struct ImmutableStructMessage
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
MyMessage _msg = new MyMessage(42);

// Example code, don't do that.
void Update()
{
    _messager.Dispatch(this, _msg);
}
```

## API Reference

### Messenger Class

#### Properties

| Name                          | Description                              |
| ----------------------------- | ---------------------------------------- |
| `(Messsager) DefaultInstance` | Returns the `Messager` default instance. |

#### Methods

| Name                                                   | Description                                                                                           |
| ------------------------------------------------------ | ----------------------------------------------------------------------------------------------------- |
| `Listen<T>(object owner, params Action<T>[] handlers)` | Register handlers. The type you want to listen for must be passed in the generic parameter.           |
| `Cut<T>(object owner)`                                 | Remove all handlers owned by the `owner` object for the specified type.                               |
| `Dispatch<T>(T payload)`                               | Dispatch a new message. The type can be inferred from the instance passed as the `payload` parameter. |

### Message Class

#### Properties

| Name                      | Description                                                                              |
| ------------------------- | ---------------------------------------------------------------------------------------- |
| `(object) Owner`          | The reference of the object owning this message.                                         |
| `(Action<object>) Action` | The registered callback. Will be invoked when a new message type matching is dispatched. |

#### Constructors

| Name                                           | Description                                                |
| ---------------------------------------------- | ---------------------------------------------------------- |
| `Message(object owner, Action<object> action)` | Default constructor. Sets the owner and action properties. |
