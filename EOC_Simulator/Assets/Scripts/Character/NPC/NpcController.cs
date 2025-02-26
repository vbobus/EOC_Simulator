using System;
using UnityEngine;

namespace Character.NPC
{
    public class NpcController : Character
    {
        [SerializeField] private string testAnimLayerLockName;


        protected override void Awake()
        {
            base.Awake();
            
            if (!String.IsNullOrEmpty(testAnimLayerLockName))
            {
                int layerIndex = Animator.GetLayerIndex(testAnimLayerLockName);
                Animator.SetLayerWeight(layerIndex, 1f);
            }
        }
    }
}