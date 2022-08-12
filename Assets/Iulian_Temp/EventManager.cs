using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Allows components to communicate with each other without a hard reference. Currently used for
/// reading/writing to/from the SerialManager to communicate with the Arduino
/// </summary>
public class EventManager : MonoBehaviour
{

    // Stores a dict of events, each with its own set of key/value pairs
    // that can be called and updated at any time.
    // May be overkill, but useful if we want to update multiple values
    // with a single event on trigger.
    private Dictionary<string, Action<Dictionary<string, object>>> eventDictionary;

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
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();

                    //  Sets this to not be destroyed when reloading scene
                    DontDestroyOnLoad(eventManager);
                }
            }
            return eventManager;
        }
    }

    // Creates an event dictionary
    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action<Dictionary<string, object>>>();
        }
    }

    public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> thisEvent;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        if (eventManager == null) return;
        Action<Dictionary<string, object>> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    /// <summary>
    /// Usages (No param, 1 param, 2 or more param)
    /// EventManager.TriggerEvent("gameOver", null);
    /// EventManager.TriggerEvent("gamePause", new Dictionary<string, object> { { "pause", true } });
    /// EventManager.TriggerEvent("addReward", 
    ///  new Dictionary<string, object> {
    ///    { "name", "candy" },
    ///    { "amount", 5 } 
    ///  });
    /// </summary>
    public static void TriggerEvent(string eventName, Dictionary<string, object> message)
    {
        Action<Dictionary<string, object>> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(message);
        }
    }
}