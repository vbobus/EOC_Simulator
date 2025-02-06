using Activity_System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ActivityLogButton : MonoBehaviour, ISelectHandler
    {
        public Button Button { get; private set; }
        [SerializeField] private TMP_Text activityLogText;

        private Activity _activity;
        public void InstantiateButton(Activity activity)
        {
            Button = gameObject.GetComponent<Button>();
            _activity = activity;
            activityLogText.text = _activity.Info.displayName;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            // Open with more text 
            
        }
    }
}
