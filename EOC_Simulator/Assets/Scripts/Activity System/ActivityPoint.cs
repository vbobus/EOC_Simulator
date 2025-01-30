using System;
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
        [FormerlySerializedAs("needToBeClose")] [SerializeField] private bool needToBeCloseToInteract = true;

        [Header("Config")]
        [SerializeField] private bool startPoint = true;
        [SerializeField] private bool finishPoint = true;
        
        [Header("Automatic")]
        [SerializeField] private bool automaticStartActivity;   // Will start the activity on its own
        [SerializeField] private bool automaticFinishActivity; // Will finish the activity if it can, and give appropriate rewards

        [Header("Optional Settings")]
        [SerializeField] private ActivityIcon activityIcon;
        
        private bool _playerIsNear;
        private ActivityState _currentActivityState;

        [SerializeField] private float radius = 2f;
        private const float TimeBetweenChecks = 0.5f;

        private void Awake()
        {
            InvokeRepeating(nameof(IsPlayerInsideCheck), 0, TimeBetweenChecks);
        }

        private void OnEnable()
        {
            GameEventsManager.Instance.ActivityEvents.OnActivityStateChange += ActivityStateChange;
            InputManager.Instance.OnConfirmActionPressed += SubmitPressed;
        }
        
        private void OnDisable()
        {
            GameEventsManager.Instance.ActivityEvents.OnActivityStateChange -= ActivityStateChange;
            InputManager.Instance.OnConfirmActionPressed -= SubmitPressed;
        }
        
        private void SubmitPressed()
        {
            if (needToBeCloseToInteract && !_playerIsNear) return;
            
            // Will either start or finish a activity if it's possible
            if (_currentActivityState.Equals(ActivityState.CAN_START) && startPoint)
                GameEventsManager.Instance.ActivityEvents.StartActivity(activityInfoForPoint);
            else if (_currentActivityState.Equals(ActivityState.CAN_FINISH) && finishPoint)
                GameEventsManager.Instance.ActivityEvents.FinishActivity(activityInfoForPoint);
        }
        
        private void ActivityStateChange(Activity activity)
        {
            if (!activity.Info.ID.Equals(activityInfoForPoint.ID)) return;

            _currentActivityState = activity.State;

            AutomaticCheckStartFinishActivity();
            
            if (activityIcon)
                activityIcon.SetState(_currentActivityState, startPoint, finishPoint);
        }


        private void AutomaticCheckStartFinishActivity()
        {
            // Automatic start or finish if it has been enabled
            
            
            /*
             * DOSENT WORK WITH BEING NEAR AND ONLY COMPLETE WHEN ITS FINISHED.
             * cHANGE THE IF STATESMENT
             */
            
            
            if (needToBeCloseToInteract && !_playerIsNear) return;
            
            if (automaticStartActivity && _currentActivityState.Equals(ActivityState.CAN_START) && startPoint)
                GameEventsManager.Instance.ActivityEvents.StartActivity(activityInfoForPoint);
            
            if (automaticFinishActivity && _currentActivityState.Equals(ActivityState.CAN_FINISH) && finishPoint)
                GameEventsManager.Instance.ActivityEvents.FinishActivity(activityInfoForPoint);
        }
        
        private void IsPlayerInsideCheck()
        {
            if (_currentActivityState == ActivityState.FINISHED)
            {
                CancelInvoke(nameof(IsPlayerInsideCheck));
                return;
            }
            if (!(_currentActivityState == ActivityState.CAN_START || _currentActivityState == ActivityState.CAN_FINISH)) return;

            // Need to have a check, since we use a Character Controller component, and not a normal collider / rigidbody 
            var sphereResults= Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Player"));
            _playerIsNear = sphereResults.Length > 0; // If player is near -> Can Start or finish the activity if they press the button.
            
            // 
            AutomaticCheckStartFinishActivity();
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draws the radius of the overlap check
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}