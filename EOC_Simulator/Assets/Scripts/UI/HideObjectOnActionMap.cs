using System;
using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class HideObjectOnActionMap : MonoBehaviour
    {
        [SerializeField] private ActionMap showOnSelectedMap;
        
        private void Awake()
        {
            InputManager.Instance.OnSwitchedActionMap += OnSwitchedActionMap;
        }

        private void OnSwitchedActionMap(ActionMap actionMap)
        {
            if (actionMap != showOnSelectedMap)
                gameObject.SetActive(false);
        }
    }
}
