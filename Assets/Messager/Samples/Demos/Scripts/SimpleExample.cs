using UnityEngine;

public class SimpleExample : MonoBehaviour
{
    readonly private Messager _messager = Messager.DefaultInstance;

    void Start()
    {
        _messager
            .Listen<MyMessage>(
                owner: this,
                (msg) => Debug.Log("Anonymous handler!")
            )
            .Listen<MyOtherMessage>(
                owner: this,
                (msg) => Debug.Log($"Count: {msg.count}")
            );
    }

    [ContextMenu("Fire Event")]
    void FireEvent()
    {
        _messager.Dispatch(new MyMessage { count = 100 });
        _messager.Dispatch(new MyOtherMessage(count: 100));
    }

    [ContextMenu("Cut")]
    void Cut()
    {
        _messager.Cut<MyMessage>(this);
    }
}

public sealed class MyMessage
{
    public int count;
}

readonly public struct MyOtherMessage
{
    readonly public int count;

    public MyOtherMessage(int count)
    {
        this.count = count;
    }
}
