using Events;
using UnityEngine;

namespace Activity_System
{
    public abstract class ActivityStep : MonoBehaviour
    {
        private bool _isFinished = false;
        private ActivityInfoSo _activityInfoSo;
        private int _stepIndex;

        public void InitializeActivityStep(ActivityInfoSo id, int stepIndex, string activityStepState)
        {
            _activityInfoSo = id;
            this._stepIndex = stepIndex;
        }
        
        protected void FinishActivityStep()
        {
            if (_isFinished) return;
            _isFinished = true;
            GameEventsManager.Instance.ActivityEvents.AdvanceQuest(_activityInfoSo);
            Destroy(gameObject);
        }

        protected void ChangeState(string newState)
        {
            GameEventsManager.Instance.ActivityEvents.ActivityStateStepChange(_activityInfoSo, _stepIndex, new ActivityStepState(newState));
        }
    }
}
