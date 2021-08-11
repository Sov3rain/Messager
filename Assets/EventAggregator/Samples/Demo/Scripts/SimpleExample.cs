using UnityEngine;
using Messaging;

public class SimpleExample : MonoBehaviour
{
    private readonly Messenger messenger = Messenger.DefaultInstance;

    private void Start()
    {
        messenger.Listen<MyMessage>(
            owner: this,
            MyMessageHandler,
            MyOtherMessageHandler
        );
        messenger.Dispatch(new MyMessage());
    }

    [ContextMenu("Fire Event")]
    private void FireEvent()
    {
        messenger.Dispatch(new MyMessage
        {
            count = 100
        });
        messenger.Cut<MyMessage>(this);
    }

    private void MyMessageHandler(MyMessage data)
    {
        Debug.Log("Hey! 1");
    }

    private void MyOtherMessageHandler(MyMessage data)
    {
        Debug.Log("Hey! " + data.count);
    }
}

public sealed class MyMessage
{
    public int count;
}