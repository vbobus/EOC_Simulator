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
            
            if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;
            
            _playerIcon = GetComponentInChildren<MiniMapPlayerIcon>();
            if (!_playerIcon) throw new UnityException($"Need to have a {nameof(MiniMapPlayerIcon)} on a child, to show the player");
            
            _miniMapTargetIcon = GetComponentInChildren<MiniMapTarget>();
            if (!_miniMapTargetIcon)
                throw new Exception(
                    $"Need to have a {nameof(MiniMapTarget)} on a child, since it will be used in the dialogue system");
            
            _rectTransform = GetComponent<RectTransform>();
            
            // Set start reference to player
            _playerIcon.player = player;
            _miniMapTargetIcon.player = player;
            _miniMapTargetIcon.miniMapPanel = _rectTransform;
        }

        private void Start()
        {
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
            
            /*
            if (newTarget != null)
                Debug.Log($"UpdateNewTarget: {newTarget.name}");
            else 
                Debug.Log($"UpdateNewTarget");
                */
            
            _miniMapTargetIcon.Show(!newTarget ? false: newTarget);
            _miniMapTargetIcon.target = newTarget;
        }
    }
}
