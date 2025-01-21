using Ëvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Activity_System
{
    [RequireComponent(typeof(Collider))]
    public class ActivityPoint: MonoBehaviour
    {
        [Header("Activity")]
        [SerializeField] private ActivityInfoSo activityInfoForPoint;
        [SerializeField] private InputActionReference submitReference; // Maybe move this to player

        [Header("Config")]
        [SerializeField] private bool startPoint = true;
        [SerializeField] private bool finishPoint = true;
        private bool _playerIsNear;

        private string _activityId;
        private ActivityState _currentActivityState;
        private ActivityIcon _activityIcon;
        
        private void Awake()
        {
            _activityId = activityInfoForPoint.ID;
            _activityIcon = GetComponentInChildren<ActivityIcon>();
        }

        private void OnEnable()
        {
            GameEventsManager.Instance.ActivityEvents.OnActivityStateChange += ActivityStateChange;
            submitReference.action.performed += SubmitPressed;
            Debug.Log("Started Activity Point");
        }
        
        private void OnDisable()
        {
            GameEventsManager.Instance.ActivityEvents.OnActivityStateChange -= ActivityStateChange;
            submitReference.action.performed -= SubmitPressed;
        }
        
        private void SubmitPressed(InputAction.CallbackContext obj)
        {
            Debug.Log("Submit Pressed");
            // if (!_playerIsNear) return;
            
            if (_currentActivityState.Equals(ActivityState.CAN_START) && startPoint)
                GameEventsManager.Instance.ActivityEvents.StartActivity(_activityId);
            else if (_currentActivityState.Equals(ActivityState.CAN_FINISH) && finishPoint)
                GameEventsManager.Instance.ActivityEvents.FinishActivity(_activityId);
        }
        
        private void ActivityStateChange(Activity activity)
        {
            if (activity.Info.ID.Equals(_activityId))
            {
                _currentActivityState = activity.State;
                _activityIcon.SetState(_currentActivityState, startPoint, finishPoint);
            }
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.CompareTag("Player"))
        //         _playerIsNear = true;
        // }
        //
        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (other.CompareTag("Player"))
        //         _playerIsNear = false;
        // }
    }
}