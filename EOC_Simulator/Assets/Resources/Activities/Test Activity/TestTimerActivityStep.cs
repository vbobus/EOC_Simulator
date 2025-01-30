using System;
using System.Collections.Generic;
using Activity_System;
using UnityEngine;


namespace Resources.Activities.Test_Activity
{
    public class TestTimerActivityStep : ActivityStep
    {
        [SerializeField] private float timeToFinish;
        [SerializeField] private float time;

        // Button, finished button -> Call method here to evaluate if its correct.
        // If the its correct _> FinishActivityStep();

        private void Start()
        {
            // Debug.Log($"Activity Step Start with timer {timeToFinish}");            
        }

        private void Update()
        {
            time += Time.deltaTime;
            // Debug.Log($"Time of activity: {time}");
            
            
            if (time >= timeToFinish)
                FinishActivityStep();
        }
        
    }
}