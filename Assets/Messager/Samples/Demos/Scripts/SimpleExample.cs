using UnityEngine;

public class SimpleExample : MonoBehaviour
{
    readonly Messager _messager = Messager.DefaultInstance;

    void Start()
    {
        _messager
            .Listen<SIMPLE_MESSAGE>(
                owner: this,
                handler: msg => Debug.Log("Anonymous handler!")
            )
            .Listen<OTHER_MESSAGE>(
                owner: this,
                handler: msg => Debug.Log($"Count: {msg.count}")
            );
    }

    [ContextMenu("Fire Event")]
    public void DispatchMessage()
    {
        _messager.Dispatch(new SIMPLE_MESSAGE { Count = 100 });
        _messager.Dispatch(new OTHER_MESSAGE(count: 100));
    }

    [ContextMenu("Cut")]
    void Cut()
    {
        _messager.Cut<SIMPLE_MESSAGE>(this);
    }
}
