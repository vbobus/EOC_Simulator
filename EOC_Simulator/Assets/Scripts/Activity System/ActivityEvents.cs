using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Activity_System
{
    public class ActivityEvents
    {
        public UnityAction<ActivityInfoSo> OnStartActivity {get; set;}
        public UnityAction<ActivityInfoSo> OnAdvanceActivity {get; set;}
        public UnityAction<ActivityInfoSo> OnFinishActivity {get; set;}
        
        public UnityAction<Activity> OnActivityStateChange {get; set;}
        public UnityAction<ActivityInfoSo, int, ActivityStepState> OnActivityStepStateChange {get; set;}
        public UnityAction<ActivityInfoSo, int> OnActivityStepUpdateInfo {get; set;}

        public void StartActivity(ActivityInfoSo activityInfoSo)
        {
            OnStartActivity?.Invoke(activityInfoSo);
        }

        public void AdvanceQuest(ActivityInfoSo activityInfoSo)
        { 
            OnAdvanceActivity?.Invoke(activityInfoSo);
        }
    
        public void FinishActivity(ActivityInfoSo activityInfoSo)
        {
            OnFinishActivity?.Invoke(activityInfoSo);
        }

        public void QuestStateChange(Activity activity)
        {
            OnActivityStateChange?.Invoke(activity);
            Debug.Log($"OnActivityStateChange: {activity.Info.ID}");
        }
    
        public void ActivityStateStepChange(ActivityInfoSo activityInfoSo, int stepIndex, ActivityStepState activityStepState)
        {
            OnActivityStepStateChange?.Invoke(activityInfoSo, stepIndex, activityStepState);
        }
        
        public void ActivityStepUpdateInfo(ActivityInfoSo activityInfoSo, int stepIndex)
        {
            OnActivityStepUpdateInfo?.Invoke(activityInfoSo, stepIndex);
        }
    }
}