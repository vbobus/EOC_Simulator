using System;
using QuickOutline.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Interactable
{
    [RequireComponent(typeof(Collider), typeof(Outline))]
    public class InteractableObject: MonoBehaviour
    {
        private Outline _outline;

        private readonly Color _baseColor = new(5.3f, 5.3f, 0f, 1f); 
        private readonly Color _hoverColor = new(5.3f, 1.3f, 0f, 1f);
        
        public UnityAction OnInteracted {get; set;}
        [SerializeField] private bool hideOnQuestFinish = false;
        
        private void Awake()
        {
            _outline = GetComponent<Outline>();
            _outline.OutlineColor = _baseColor;
            
            ShowHideOutline(false);
        }
        
        public void OnHoverOver() => _outline.OutlineColor = _hoverColor;

        public void OnHoverOut() => _outline.OutlineColor = _baseColor;

        public void Interact() => OnInteracted?.Invoke();

        public void ShowHideOutline(bool show) => _outline.enabled = show;
        
        public void HideObject()
        {
            if (!hideOnQuestFinish) return;
            gameObject.SetActive(false);
        }
    }
}