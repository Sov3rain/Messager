# Messenger

![version](https://img.shields.io/github/v/tag/Sov3rain/Unity-Event-Aggregator?label=latest) ![unity-version](https://img.shields.io/badge/unity-2019.4%2B-lightgrey)

This implementation of the event aggregator pattern tries to overcome the limitations of traditional event handling by providing a central place to publish and subscribe for events. It takes care of registering, unregistering and invoking events and thus decoupling publishers and subscribers.

## Installation

This package uses Unity's [scoped registry](https://docs.unity3d.com/Manual/upm-scoped.html) feature to import packages. Add the following sections to the package manifest file located in `Packages/manifest.json`.

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
"com.sov3rain.unity-messenger": "3.0.0"
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
    "com.sov3rain.unity-messenger": "3.0.0",
    ...
```

## Usage
### Create a message

`Messages` are just plain C# classes.

```csharp
// Very simple message
public sealed class MyMessage { } 

// Another message, holding state (data)
public sealed class MyOtherMessage
{ 
    public int number;
}
```

### Get a reference of the Messenger

You can get a direct reference in any part of your code by accessing the default static instance, or instantiate your own.

```csharp
using Messenging;

// Default static instance
private readonly Messenger messenger = Messenger.DefaultInstance;

// Custom instance.
private readonly Messenger myMessenger = new Messenger();
```

### Dispatch an event

You can dispatch an event by using the `Dispatch()` method.

```csharp
// Dispatch.
messenger.Dispatch(new MyMessage());
```

The message type is inferred by the class instance you create and pass as an argument.

### Start listening for message

Anywhere in any class, you can listen for a message by using the `Listen<T>()` method.

`````c#
// Listen (register).
messenger.Listen<MyMessage>(this, msg => {
   Debug.Log("Message incoming!"); 
});
`````

The type you want to listen for is specified through the generic type parameter. You can add multiple listeners at the same time, the payload parameter being flagged with the keyword `params`:

`````c#
// Add multiple listeners at the same time.
messenger.Listen<MyMessage>(
    owner: this,
    MyFirstHandler, // Just a standard method.
    MySecondHandler,
    (msg) => MyThirdHandler() // Adding a listener with an anonymous function.
);

// Example of a message handler.
private void MyFirstHandler(MyMessage msg)
{
    Debug.Log("Message incoming!");
}
`````

> Note: You can register anonymous functions safely, because they are bonded to the instance registering (hence the `owner` parameter).

### Stop listening for message

When you want to stop being notified, or before destroying the instance of the class listening, you can call `Cut()` to unregister the object.

`````c#
// Cut (unregister).
messenger.Cut<MyMessage>(this);
`````

> Note: when using `Cut`, all listeners are removed at once, as they are retrieved by owner. 

## Performance

## API Reference

### Messenger Class

#### Fields

#### Properties

#### Methods

