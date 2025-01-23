using Events;
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
        
        [Header("Optional Settings")]
        [SerializeField] private ActivityIcon activityIcon;
        /// <summary>
        /// Goes to finish and have no need to call the finnish 
        /// </summary>
        [SerializeField] private bool automaticFinnishActivity;
        // [SerializeField] private bool repeatableActivity; // Will start the
        
        /*
         * Needs player to be close, but first need a reference for the player.
         * When spawning in a player based on the selected role, we will add the reference
        */
        private bool _playerIsNear;

        // private string _activityId;
        private ActivityState _currentActivityState;
        
        private void Awake()
        {
            // _activityId = activityInfoForPoint.ID;
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
        
        // Need to make
        private void SubmitPressed(InputAction.CallbackContext obj)
        {
            // if (!_playerIsNear) return;
            
            if (_currentActivityState.Equals(ActivityState.CAN_START) && startPoint)
                GameEventsManager.Instance.ActivityEvents.StartActivity(activityInfoForPoint);
            else if (_currentActivityState.Equals(ActivityState.CAN_FINISH) && finishPoint)
                GameEventsManager.Instance.ActivityEvents.FinishActivity(activityInfoForPoint);
        }
        
        private void ActivityStateChange(Activity activity)
        {
            if (!activity.Info.ID.Equals(activityInfoForPoint.ID)) return;

            _currentActivityState = activity.State;

            if (automaticFinnishActivity && _currentActivityState.Equals(ActivityState.CAN_FINISH) && finishPoint)
                GameEventsManager.Instance.ActivityEvents.FinishActivity(activityInfoForPoint);

            if (activityIcon)
                activityIcon.SetState(_currentActivityState, startPoint, finishPoint);
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