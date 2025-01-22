using Events;
using UnityEngine;

namespace Activity_System
{
    public abstract class ActivityStep : MonoBehaviour
    {
        private bool _isFinished = false;
        private string _activityId;
        private int _stepIndex;
        public void InitializeActivityStep(string id, int stepIndex, string activityStepState)
        {
            this._activityId = id;
            this._stepIndex = stepIndex;
        }
        
        protected void FinishActivityStep()
        {
            if (!_isFinished)
            {
                _isFinished = true;
                GameEventsManager.Instance.ActivityEvents.AdvanceQuest(_activityId);
                Destroy(gameObject); 
            }
        }

        protected void ChangeState(string newState)
        {
            GameEventsManager.Instance.ActivityEvents.ActivityStateStepChange(_activityId, _stepIndex, new ActivityStepState(newState));
        }
    }
}
