using UnityEngine;
using UnityEngine.Events;

namespace Activity_System
{
    public class ActivityEvents
    {
        public UnityAction<ActivityInfoSo> OnStartActivity {get; set;}
        public UnityAction<ActivityInfoSo> OnAdvanceActivity {get; set;}
        public UnityAction<ActivityInfoSo> OnFinishActivity {get; set;}
        
        public UnityAction<Activity> OnActivityStateChange;
        public UnityAction<ActivityInfoSo, int, ActivityStepState> OnActivityStepStateChange;

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
        }
    
        public void ActivityStateStepChange(ActivityInfoSo activityInfoSo, int stepIndex, ActivityStepState activityStepState)
        {
            OnActivityStepStateChange?.Invoke(activityInfoSo, stepIndex, activityStepState);
        }
    }
}