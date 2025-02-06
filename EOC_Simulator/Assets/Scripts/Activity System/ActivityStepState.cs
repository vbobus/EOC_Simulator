using UnityEngine.Serialization;

namespace Activity_System
{
    [System.Serializable]
    public class ActivityStepState
    {
        public string State;

        public ActivityStepState()
        {
            this.State = "";
        }
        
        public ActivityStepState(string state)
        {
            this.State = state;
        }
    }
}