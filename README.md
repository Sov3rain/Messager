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

`Messages` are just plain C# classes. They serve the purpose of creating a unique signature by enforcing Type reference instead of string reference. They can also hold a state (data).

> Note: messages being classes, they are reference type. Modifying them in one place will potentially impact other systems in your application. You can mitigate this by making your class immutable.

```csharp
// Very simple message
public sealed class MyMessage { } 

// Another message, holding state (data).
// This class is immutable.
public sealed class MyOtherMessage
{ 
    public int Number { get; private set; }
  
  	public MyOtherMessage(int number)
    {
      Number = number;
    }
}
```

### Referencing the messenger

You can get a direct reference in any part of your code by accessing the default static instance, or instantiate your own.

```csharp
using Messenging;

// Default static instance
private readonly Messenger messenger = Messenger.DefaultInstance;

// Custom instance.
private readonly Messenger myMessenger = new Messenger();
```

> Note: instantiating your own Messenger class could help unit testing or mocking, but for regular use, it's recommended to use the default static instance.

### Dispatch a message

You can dispatch an event by using the `Dispatch()` method.

```csharp
// Dispatch.
messenger.Dispatch(new MyMessage());
```

The message type is inferred by the class instance you create and pass as an argument.

### Start listening for a message

Anywhere in any class, you can listen for a message by using the `Listen<T>()` method. The type you want to listen for is specified through the generic type parameter.

`````c#
// Listen (register).
messenger.Listen<MyMessage>(this, msg => {
   Debug.Log("Message incoming!"); 
});
`````

 You can add multiple listeners at the same time, the payload parameter is flagged with the keyword `params`:

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

> Note: You can register anonymous functions safely. Because they are bonded to the instance registering (hence the `owner` parameter), they are referenced and can be destroyed later.

### Stop listening for a message

When you want to stop being notified, or before destroying the instance of the class listening, you can call `Cut()` to unregister the object.

`````c#
// Cut (unregister).
messenger.Cut<MyMessage>(this);
`````

> Note: when using `Cut`, all listeners registered for that object are removed, as they are referenced by owner. 

## Performance

Of course the performance of the `Menssenger` is not the same as a plain C# action, but my goal was convenience over performance. Nonetheless, the `Messenger` is 40% to 50% slower than an `Action`.

I think for most people it's negligible, but if you think you can improve the performance of the `Messenger`, PRs are welcome! :).

Here is an example with a list of 100,000 listeners, each logging when the message is received:

// TODO: ADD SCREENSHOT  

## API Reference

### Messenger Class

#### Fields

| Name | Description |
| ---- | ----------- |
|      |             |

#### Properties

| Name | Description |
| ---- | ----------- |
|      |             |

#### Methods

| Name | Description |
| ---- | ----------- |
|      |             |

