using System;
using UnityEngine;

namespace MiniMap
{
    public class MiniMap : MonoBehaviour
    {
        public Transform player;

        private MiniMapPlayerIcon _playerIcon;
        private MiniMapTarget _target;
        
        private void Awake()
        {
            if (player == null) throw new UnityException($"For the minimap to work, we need to set the player transform reference");
            
            _playerIcon = GetComponentInChildren<MiniMapPlayerIcon>();
            if (!_playerIcon) throw new UnityException($"Need to have a {nameof(MiniMapPlayerIcon)} on a child, to show the player");
            
            _target = GetComponentInChildren<MiniMapTarget>();
            if (!_target)
                throw new Exception(
                    $"Need to have a {nameof(MiniMapTarget)} on a child, since it will be used in the dialogue system");
        }

        public void UpdateNewTarget(Transform newTarget)
        {
            if (!_target) return; 
            
        }
        
    }
}
