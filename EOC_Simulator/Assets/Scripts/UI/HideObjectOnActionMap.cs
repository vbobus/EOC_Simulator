using System;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class HideObjectOnActionMap : MonoBehaviour
    {
        [SerializeField] private ActionMap showOnSelectedMap;
     
        /*
         * There were errors with the exicution of some scrolling list, so we want it to finish
         * the Awake/Start before we hide it. So we just hide it on the first update
         */
        private bool _reachedFirstUpdateLoop;  // Make sure the tmppro loads in correctly
        private bool _wantsToSwitchActionMap; 

        private void Awake()
        {
            InputManager.Instance.OnSwitchedActionMap += OnSwitchedActionMap;
        }

        private void Update()
        {
            if (_reachedFirstUpdateLoop) return;
            _reachedFirstUpdateLoop = true;
            if (_wantsToSwitchActionMap)
                OnSwitchedActionMap();
        }
        
        private void OnSwitchedActionMap(ActionMap actionMap = ActionMap.Player)
        {
            if (actionMap == showOnSelectedMap) return;
            _wantsToSwitchActionMap = true;
            if (!_reachedFirstUpdateLoop) return;
            gameObject.SetActive(false);
        }
    }
}
