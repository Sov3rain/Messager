using UnityEngine;

public class MessageReceiver : MonoBehaviour
{
    [SerializeField]
    private GameObject _label;

    readonly Messager _messager = Messager.DefaultInstance;

    void Start()
    {
        _label.SetActive(false);
        _messager.Listen<SIMPLE_MESSAGE>(this, _ => _label.SetActive(true));
    }
}
