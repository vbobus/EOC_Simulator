using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace MiniMap
{
    public class MiniMap : MonoBehaviour
    {
        public Transform player;

        private MiniMapPlayerIcon _playerIcon;
        private MiniMapTarget _miniMapTargetIcon;
        private RectTransform _rectTransform;

        public static MiniMap Instance;
        
        private void Awake()
        {
            if (!Instance) Instance = this;
            else
            {
                Destroy(Instance);
                return;
            }
            
            if (player == null) throw new UnityException($"For the minimap to work, we need to set the player transform reference");
            
            _playerIcon = GetComponentInChildren<MiniMapPlayerIcon>();
            if (!_playerIcon) throw new UnityException($"Need to have a {nameof(MiniMapPlayerIcon)} on a child, to show the player");
            
            _miniMapTargetIcon = GetComponentInChildren<MiniMapTarget>();
            if (!_miniMapTargetIcon)
                throw new Exception(
                    $"Need to have a {nameof(MiniMapTarget)} on a child, since it will be used in the dialogue system");
            
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            // Set start reference to player
            _playerIcon.player = player;
            _miniMapTargetIcon.player = player;
            _miniMapTargetIcon.miniMapPanel = _rectTransform;
            
            // Hide the icon at the start
            UpdateNewTarget(null);
        }

        
        /// <summary>
        /// Used when wanting to change the minimap target
        /// </summary>
        /// <param name="newTarget">Shows the position, and can hide if param is null</param>
        public void UpdateNewTarget(Transform newTarget)
        {
            if (!_miniMapTargetIcon) return;
            
            _miniMapTargetIcon.Show(newTarget);
            _miniMapTargetIcon.target = newTarget;
        }
        
    }
}
