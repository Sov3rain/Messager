using UnityEngine;
using UnityEngine.UI;

public class SimpleExample : MonoBehaviour
{
    [SerializeField]
    private Button _dispatchButton, _cutButton;

    void Start()
    {
        this.Listen<SIMPLE_MESSAGE>(msg => print($"Message received: {msg.Count}"));

        _dispatchButton.onClick.AddListener(() => this.Dispatch(new SIMPLE_MESSAGE { Count = 42 }));
        _cutButton.onClick.AddListener(() => this.Cut<SIMPLE_MESSAGE>());
    }
}
