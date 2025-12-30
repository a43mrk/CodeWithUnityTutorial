using UnityEngine;
using UnityEngine.Events;

public abstract class EventListener<T> : MonoBehaviour
{
    [SerializeField] EventChannel<T> eventChannel;
    [SerializeField] UnityEvent<T> unityEvent;

    void Awake()
    {
        eventChannel.Register(this);
    }

    void OnDestroy()
    {
        eventChannel.UnRegister(this);
    }

    public void Raise(T value)
    {
        unityEvent?.Invoke(value);
    }

}
