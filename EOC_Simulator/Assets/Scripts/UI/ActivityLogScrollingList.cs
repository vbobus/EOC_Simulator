using System;
using System.Collections.Generic;
using Activity_System;
using Events;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class ActivityLogScrollingList : MonoBehaviour
    {
        [SerializeField] private GameObject contentParent;
        [SerializeField] private GameObject activityLogButtonPrefab;
        
        private readonly Dictionary<string, ActivityLogButton> _activityLogMap = new Dictionary<string, ActivityLogButton>();


        private void OnEnable()
        {
            GameEventsManager.Instance.ActivityEvents.OnActivityStateChange += StateChange;
        }
        private void OnDisable()
        {
            GameEventsManager.Instance.ActivityEvents.OnActivityStateChange -= StateChange;
        }
        private void StateChange(Activity activity)
        {
            Debug.Log($"Changed activity quest. {activity.Info.displayName}: New State {activity.State}");
            switch (activity.State)
            {
                case ActivityState.CAN_START:
                    CreateButtonIfNotExits(activity);
                    SetActivityLogInfo(activity);
                    break;
                case ActivityState.IN_PROGRESS:
                case ActivityState.FINISHED:
                    SetActivityLogInfo(activity);
                    break;
            }
        }

        private void SetActivityLogInfo(Activity activity)
        {
            _activityLogMap.TryGetValue(activity.Info.ID, out ActivityLogButton activityLogButton);
            if (activityLogButton == null) throw new UnityException($"Activity Log Button with ID {activity.Info.ID} not found in the activity map for the scrolling list");

            if (activity.State == ActivityState.CAN_START)
            {
                activityLogButton.StartAndUpdateActivityBtn();
            }
            else if (activity.State == ActivityState.FINISHED)
            {
                activityLogButton.FinishActivityBtn();
            }else if (activity.State == ActivityState.IN_PROGRESS)
            {
                
            }
        }
        
        public ActivityLogButton CreateButtonIfNotExits(Activity activity)
        {
            ActivityLogButton logButton = null;
            
            if (!_activityLogMap.TryGetValue(activity.Info.ID, out ActivityLogButton value))
            { 
                logButton = InstantiateLogButton(activity);   
            }
            else
            {
                logButton = value;
            }
            
            return logButton;
        }
        
        private ActivityLogButton InstantiateLogButton(Activity activity)
        {
            ActivityLogButton logButton = Instantiate(activityLogButtonPrefab, contentParent.transform).GetComponentInChildren<ActivityLogButton>();
            logButton.gameObject.name = activity.Info.name + "_Button";
            logButton.InstantiateButton(activity);
            
            _activityLogMap[activity.Info.ID] = logButton;
            return logButton;
        }

    }
}