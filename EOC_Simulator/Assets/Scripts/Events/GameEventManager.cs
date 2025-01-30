using Activity_System;
using UnityEngine;

namespace Events
{
    public class GameEventsManager : MonoBehaviour
    { 
        public static GameEventsManager Instance {get; private set;}

        public ActivityEvents ActivityEvents;
    
        private void Awake()
        {
            if (Instance != null)
                Debug.LogError("More than one GameEventsManager in scene!");
            Instance = this;
        
            ActivityEvents = new ();
        }
    
    }
}