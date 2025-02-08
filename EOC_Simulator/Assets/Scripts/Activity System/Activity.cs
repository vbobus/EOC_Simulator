using System.Collections.Generic;
using UnityEngine;

namespace Activity_System
{
    public class Activity
    {
        public readonly ActivityInfoSo Info;
        private int _currentActivityStepIndex;
        public ActivityState State;
        private readonly ActivityStepState[] _questStepStates;
        
        public Activity(ActivityInfoSo info)
        {
            this.Info = info;
            this.State = ActivityState.REQUIREMENTS_NOT_MET;
            _currentActivityStepIndex = 0;
            this._questStepStates = new ActivityStepState[info.activityStepPrefabs.Length];
            for (int i = 0; i < _questStepStates.Length; i++)
            {
                _questStepStates[i] = new ActivityStepState();
            }
        }

        public Activity(ActivityInfoSo activityInfo, ActivityState activityState, int currentActivityStepIndex,
            ActivityStepState[] questStepStates)
        {
            this.Info = activityInfo;
            this.State = activityState;
            this._currentActivityStepIndex = currentActivityStepIndex;
            this._questStepStates = questStepStates;
            
            // If quest step states and prefab are different lengths,
            // the saved data is out of sync
            if (this._questStepStates.Length != this.Info.activityStepPrefabs.Length)
            {
                Debug.LogWarning($"Quest step states and prefab are different lengths, he saved data is out of sync\nReset data {this.Info.ID}");
            }
        }

        public void MoveToNextStep()
        {
            _currentActivityStepIndex++;
        }

        public bool CurrentStepExits()
        {
            return (_currentActivityStepIndex < Info.activityStepPrefabs.Length);
        }

        public void InstantiateCurrentActivityStep(Transform parentTransform)
        {
            GameObject questStepPrefab = GetCurrentQuestStepPrefab();
            if (questStepPrefab != null)
            {
                var questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform)
                    .GetComponent<ActivityStep>();

                questStep.InitializeActivityStep(Info, _currentActivityStepIndex, _questStepStates[_currentActivityStepIndex].State);
            }
        }


        public void SetStepDescriptionList(Transform parentTransform)
        {
            foreach (var prefab in Info.activityStepPrefabs)
            {
                var activityStep = Object.Instantiate(prefab, parentTransform).GetComponent<ActivityStep>();
                ActivityStepDescriptions.Add(activityStep.GetStepDescription());
                Object.Destroy(activityStep.gameObject);
            }
            
            // Debug.Log($"Descriptions count = {ActivityStepDescriptions.Count}");
        }
        
        public List<string> ActivityStepDescriptions = new List<string>(); 
        
        
        private GameObject GetCurrentQuestStepPrefab()
        {
            GameObject activityStepPrefab = null;
            if (CurrentStepExits())
                activityStepPrefab = Info.activityStepPrefabs[_currentActivityStepIndex];
            else 
                Debug.LogWarning($"Tried to get quest step prefab, but step index is out of range. QuestID={Info.ID}, stepIndex={_currentActivityStepIndex}");
            return activityStepPrefab;
        }

        public void StoreActivityStepState(ActivityStepState activityStep, int stepIndex)
        {
            if (stepIndex < _questStepStates.Length)
            {
                _questStepStates[stepIndex].State = activityStep.State;
            }
            else
            {
                Debug.LogWarning($"Tried to access quest step data, but step index was out of range. QuestID={Info.ID}, stepIndex={stepIndex}");
            }
        }
    }
}
