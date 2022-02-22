using UnityEngine;
using UnityEngine.UI;

public class MessageReceiver : MonoBehaviour
{
    [SerializeField]
    private Text _label;

    void Start()
    {
        _label.text = string.Empty;
        _label.gameObject.SetActive(false);

        this.Listen<SIMPLE_MESSAGE>(msg =>
        {
            _label.text += $"Message received: {msg.Count}\n";
            _label.gameObject.SetActive(true);
        });
    }
}
