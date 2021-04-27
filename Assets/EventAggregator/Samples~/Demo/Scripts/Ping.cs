using System.Collections;
using UnityEngine;

namespace EventAggregation.Examples
{
    public class Ping : MonoBehaviour
    {
        EventAggregator _eventAggregator;

        void Start()
        {
            _eventAggregator = EventAggregator.DefaultInstance;
            _eventAggregator.AddListener<OnPong>(OnPongHandler);
            _eventAggregator.Dispatch<OnPing>();
        }

        void OnDestroy()
        {
            _eventAggregator.RemoveListener<OnPong>(OnPongHandler);
        }

        private void OnPongHandler(IEvent obj)
        {
            StartCoroutine(DoResponse());
        }

        private IEnumerator DoResponse()
        {
            Debug.Log("Receive pong!", this);
            yield return new WaitForSeconds(1);
            _eventAggregator.Dispatch(new OnPing { message = "Hello World!" });
            Destroy(gameObject);
        }
    }

    class OnPing : IEvent
    {
        public string message;
    }
}