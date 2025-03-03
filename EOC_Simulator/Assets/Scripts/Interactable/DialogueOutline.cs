using System;
using System.Collections.Generic;
using Pathfinding;
using PixelCrushers.DialogueSystem;
using QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactable
{
    public class DialogueOutline : MonoBehaviour
    {
        private readonly List<Outline> _outlines = new();
        [SerializeField] private bool canDisableUsable = true;
        private Usable _usable;
        
        private DialogueSystemTrigger[] _dialogueTriggers;
        private bool _isOutlineEnabled = false;
        
        private void Awake()
        {
            // Add this outline ref is it exits on current gameobject
            Outline outline = GetComponent<Outline>();
            if (outline) _outlines.Add(outline);
            
            // Checks it's children, and adds them if there is any
            Outline[] outlinesInChildren = GetComponentsInChildren<Outline>();
            if (outlinesInChildren != null && outlinesInChildren.Length > 0) _outlines.AddRange(outlinesInChildren);
            if (_outlines.Count == 0) throw new NullReferenceException($"There is no Outline component on {gameObject.name} object or it's children");
            EnableDisableOutlines(false);
            
            _usable = GetComponent<Usable>();
            if (_usable && canDisableUsable) _usable.enabled = false;
            
            _dialogueTriggers = GetComponents<DialogueSystemTrigger>();
            if (_dialogueTriggers.Length == 0) throw new NullReferenceException($"There are missing a dialogue system trigger on {gameObject.name}, for the outline to work");
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
        // Should properly be optimized, if a analysis of the game says so 
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
            EnableDisableOutlines(shouldEnable);

            if (_usable && canDisableUsable) _usable.enabled = shouldEnable;
        }
        
        private void EnableDisableOutlines(bool enabled)
        {
            foreach (var outline in _outlines)
            {
                outline.enabled = enabled;
            }
        }
    }
}