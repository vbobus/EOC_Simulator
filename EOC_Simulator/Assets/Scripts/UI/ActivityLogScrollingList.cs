using System;
using System.Collections.Generic;
using Activity_System;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ActivityLogScrollingList : MonoBehaviour
    {
        [SerializeField] private GameObject contentParent;
        [SerializeField] private GameObject activityLogButtonPrefab;
        
        private readonly Dictionary<string, ActivityLogButton> _activityLogMap = new Dictionary<string, ActivityLogButton>();

        private void Start()
        {
            for (int i = 0; i < 30; i++)
            {
                ActivityInfoSo infoSo = ScriptableObject.CreateInstance<ActivityInfoSo>();
                infoSo.ID = $"test{i}";
                infoSo.displayName = $"Activity {i}";
                infoSo.activityStepPrefabs = Array.Empty<GameObject>();
                Activity activity = new Activity(infoSo);
                var button = CreateButtonIfNotExits(activity, () => { Debug.Log($"Selected {infoSo.ID}"); });
                
                if (i == 0)
                    button.Button.Select();
            }
        }

        public ActivityLogButton CreateButtonIfNotExits(Activity activity, UnityAction selectAction)
        {
            ActivityLogButton logButton = null;
            
            if (!_activityLogMap.TryGetValue(activity.Info.ID, out ActivityLogButton value))
            { 
                logButton = InstantiateLogButton(activity, selectAction);   
            }
            else
            {
                logButton = value;
            }
            
            return logButton;
        }
        
        private ActivityLogButton InstantiateLogButton(Activity activity, UnityAction selectAction)
        {
            ActivityLogButton logButton = Instantiate(activityLogButtonPrefab, contentParent.transform).GetComponentInChildren<ActivityLogButton>();
            logButton.gameObject.name = activity.Info.name + "_Button";
            logButton.InstantiateButton(activity);
            
            _activityLogMap[activity.Info.ID] = logButton;
            return logButton;
        }
    }
}