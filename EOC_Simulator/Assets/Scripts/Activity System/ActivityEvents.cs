using UnityEngine.Events;

namespace Activity_System
{
    public class ActivityEvents
    {
        public UnityAction<string> OnStartActivity;
        public UnityAction<string> OnAdvanceActivity {get; set;}
        public UnityAction<string> OnFinishActivity {get; set;}
        
        public UnityAction<Activity> OnActivityStateChange;
        public UnityAction<string, int, ActivityStepState> OnActivityStepStateChange;

        public void StartActivity(string questName)
        {
            OnStartActivity?.Invoke(questName);
        }

        public void AdvanceQuest(string questName)
        { 
            OnAdvanceActivity?.Invoke(questName);
        }
    
        public void FinishActivity(string questName)
        {
            OnFinishActivity?.Invoke(questName);
        }

        public void QuestStateChange(Activity activity)
        {
            OnActivityStateChange?.Invoke(activity);
        }
    
        public void ActivityStateStepChange(string id, int stepIndex, ActivityStepState activityStepState)
        {
            OnActivityStepStateChange?.Invoke(id, stepIndex, activityStepState);
        }
    }
}