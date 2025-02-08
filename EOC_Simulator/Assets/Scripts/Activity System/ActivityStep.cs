using Events;
using UnityEngine;
using UnityEngine.Serialization;

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

        protected void UpdateActivityStepInformation()
        {
            GameEventsManager.Instance.ActivityEvents.ActivityStepUpdateInfo(_activityInfoSo, _stepIndex);
        }
        
        protected void ChangeState(string newState)
        {
            GameEventsManager.Instance.ActivityEvents.ActivityStateStepChange(_activityInfoSo, _stepIndex, new ActivityStepState(newState));
        }

        public abstract string GetStepDescription();
    }
}
