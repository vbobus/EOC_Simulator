using System;
using Activity_System;
using UnityEngine;


namespace Resources.Activities.Test_Activity
{
    public class TestTimerActivityStep : ActivityStep
    {
        [SerializeField] private float timeToFinish;
        [SerializeField] private float time;

        private void Update()
        {
            time += Time.deltaTime;
            if (time >= timeToFinish)
                FinishActivityStep();
        }
        
    }
}