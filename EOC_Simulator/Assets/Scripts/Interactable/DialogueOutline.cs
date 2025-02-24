using System;
using Pathfinding;
using PixelCrushers.DialogueSystem;
using QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactable
{
    [RequireComponent(typeof(DialogueSystemTrigger))]
    public class DialogueOutline : MonoBehaviour
    {
        private Outline _outline;
        [SerializeField] private bool canDisableUsable = true;
        private Usable _usable;
        
        private DialogueSystemTrigger[] _dialogueTriggers;
        private bool _isOutlineEnabled = false;
        
        private void Awake()
        {
            _outline = GetComponent<Outline>();
            if (_outline == null) _outline = GetComponentInChildren<Outline>();
            if (_outline == null) throw new NullReferenceException($"_outline is null in {gameObject.name}");
            
            _dialogueTriggers = GetComponents<DialogueSystemTrigger>();
            if (_dialogueTriggers.Length == 0) throw new NullReferenceException($"There are missing a dialogue system trigger on {gameObject.name}, for the outline to work");
            _outline.enabled = false;
            
            _usable = GetComponent<Usable>();
            if (_usable && canDisableUsable) _usable.enabled = false;
        }
        
        private void Start()
        {
            DialogueManager.OnUpdateTracker += HideEnableGameObjectOutlineOnCondition;
            HideEnableGameObjectOutlineOnCondition(); // Check for the first time, if it should be enabled, and not wait for the Action
        }
        private void OnDestroy()
        {
            DialogueManager.OnUpdateTracker -= HideEnableGameObjectOutlineOnCondition;
        }
        
        // This *can* cause lag spikes since all DialogueOutline needs to call this at the same time.
        private void HideEnableGameObjectOutlineOnCondition()
        {
            bool shouldEnable = false;
            
            foreach (var dialogueSystemTrigger in _dialogueTriggers)
            {
                shouldEnable = dialogueSystemTrigger.condition != null &&
                               dialogueSystemTrigger.condition.IsTrue(Character.Player.PlayerController
                                   .playerTransform);
                if (shouldEnable) break;
            }
            
            // if (_debugThis) Debug.Log($"{gameObject.name}: ShouldEnable: {shouldEnable} = IsEnabled: {_isOutlineEnabled}");
            
            if (_isOutlineEnabled == shouldEnable) return;
            // Change the enabled based
            _isOutlineEnabled = shouldEnable;
            _outline.enabled = shouldEnable; 
            if (_usable && canDisableUsable) _usable.enabled = shouldEnable;
        }
    }
}