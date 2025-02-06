using UnityEngine.Serialization;

namespace Activity_System
{
    [System.Serializable]
    public class ActivityData
    {
        public ActivityState State;
        public int ActivityStepIndex;
        public ActivityStepState[] ActivityStepStates;

        public ActivityData(ActivityState state, int activityStepIndex, ActivityStepState[] activityStepStates)
        {
            this.State = state;
            this.ActivityStepIndex = activityStepIndex;
            this.ActivityStepStates = activityStepStates;
        }
    }
}