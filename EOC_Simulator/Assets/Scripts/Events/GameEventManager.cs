using UnityEngine;

namespace Events
{
    public class GameEventsManager : MonoBehaviour
    { 
        public static GameEventsManager Instance {get; private set;}

    
        private void Awake()
        {
            if (Instance != null)
                Debug.LogError("More than one GameEventsManager in scene!");
            Instance = this;
        
        }
    
    }
}