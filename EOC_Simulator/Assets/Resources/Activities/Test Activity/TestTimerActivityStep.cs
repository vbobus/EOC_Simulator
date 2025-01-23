using System;
using Activity_System;
using UnityEngine;


namespace Resources.Activities.Test_Activity
{
    public class TestTimerActivityStep : ActivityStep
    {
        [SerializeField] private float timeToFinish;
        [SerializeField] private float time;

        // Button, finnished button -> Call method here to evaluate if its correct.
        // If the its correct _> FinishActivityStep();

        
        private void Update()
        {
            time += Time.deltaTime;
            Debug.Log($"Time of activity: {time}");
            
            
            if (time >= timeToFinish)
                FinishActivityStep();
        }
        
    }
}