using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 1. create a channel from the menu with a name you wish
/// 2. Add the Listener as a new Component to the GameObject that need to listen
/// 2.1. Create a new Scriptable Object, channel from the create menu(right clicking)
/// 3. Add the channel to the event channel field on inspector of that gameobject
/// 4. Add the GameObject related to the UnityEvent and don't forget to select the function that invokes unityevent from this component
/// 5. Add the Channel into the publisher part(this can be an generic MonoBehavior and be added into any GameObject you want) or Add it directly into the class you want
/// 6. call invoke from the channel you added witht the value
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class EventChannel<T> : ScriptableObject
{
    readonly HashSet<EventListener<T>> observers = new();


    public void Invoke(T value)
    {
        foreach(var observer in observers)
        {
            observer.Raise(value);
        }
    }


    public void Register(EventListener<T> observer) => observers.Add(observer);
    public void UnRegister(EventListener<T> observer) => observers.Remove(observer);
}

public readonly struct Empty {}

[CreateAssetMenu(menuName = "Events/EventChannel")]
public class EventChannel : EventChannel<Empty> {}