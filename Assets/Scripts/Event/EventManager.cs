using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEventBase> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("EventManger script not attached to a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    class EventUnityEvent<T> : UnityEvent<T> where T : GameEvent {        
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEventBase>();
        }
    }

    public static void StartListening<T>(UnityAction<T> listener) where T : GameEvent
    {
        UnityEventBase thisEvent = null;
        string name = listener.GetType().GetGenericArguments()[0].Name;
        if (instance.eventDictionary.TryGetValue(name, out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).AddListener(listener);
        }
        else
        {
            UnityEvent<T> newEvent = new EventUnityEvent<T>();
            newEvent.AddListener(listener);
            instance.eventDictionary.Add(name, newEvent);
        }
    }

    public static void StopListening<T>(UnityAction<T> listener) where T : GameEvent
    {
        if (eventManager == null) return;
        UnityEventBase thisEvent = null;
        string name = listener.GetType().GetGenericArguments()[0].Name;
        if (instance.eventDictionary.TryGetValue(name, out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).RemoveListener(listener);
        }
    }

    public static void TriggerEvent<T>(T theEvent) where T : GameEvent
    {
        UnityEventBase thisEvent = null;
        string name = theEvent.GetType().Name;
        if (instance.eventDictionary.TryGetValue(name, out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).Invoke(theEvent);
        }
    }
}