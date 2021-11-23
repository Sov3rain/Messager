using UnityEngine;

public class SimpleExample : MonoBehaviour
{
    readonly private Messager _messager = Messager.DefaultInstance;

    void Start()
    {
        _messager
            .Listen<SIMPLE_MESSAGE>(
                owner: this,
                (msg) => Debug.Log("Anonymous handler!")
            )
            .Listen<OTHER_MESSAGE>(
                owner: this,
                (msg) => Debug.Log($"Count: {msg.count}")
            );
    }

    [ContextMenu("Fire Event")]
    void FireEvent()
    {
        _messager.Dispatch(new SIMPLE_MESSAGE { count = 100 });
        _messager.Dispatch(new OTHER_MESSAGE(count: 100));
    }

    [ContextMenu("Cut")]
    void Cut()
    {
        _messager.Cut<SIMPLE_MESSAGE>(this);
    }
}

public sealed class SIMPLE_MESSAGE
{
    public int count;
}

readonly public struct OTHER_MESSAGE
{
    readonly public int count;

    public OTHER_MESSAGE(int count)
    {
        this.count = count;
    }
}
