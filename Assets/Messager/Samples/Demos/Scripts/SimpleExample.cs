using UnityEngine;
using Messaging;

public class SimpleExample : MonoBehaviour
{
    private readonly Messager Messager = Messager.DefaultInstance;

    private void Start()
    {
        Messager.Listen<MyMessage>(
            owner: this,
            MyMessageHandler,
            MyOtherMessageHandler,
            (msg) =>
            {
                Debug.Log("Anonymous handler!");
            }
        );
    }

    [ContextMenu("Fire Event")]
    private void FireEvent()
    {
        Messager.Dispatch(new MyMessage
        {
            count = 100
        });
    }

    [ContextMenu("Cut")]
    private void Cut()
    {
        Messager.Cut<MyMessage>(this);
    }

    private void MyMessageHandler(MyMessage data)
    {
        Debug.Log(name + ": Handler1 ", this);
    }

    private void MyOtherMessageHandler(MyMessage data)
    {
        Debug.Log(name + ": Handler2 " + data.count, this);
    }
}

public sealed class MyMessage
{
    public int count;
}
