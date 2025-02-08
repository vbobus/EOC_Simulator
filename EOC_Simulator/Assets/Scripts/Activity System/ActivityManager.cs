using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Events;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Activity_System
{
    public class ActivityManager: MonoBehaviour
    {
        public Dictionary<string, Activity> ActivityMap {get; private set;}
        
        public static ActivityManager Instance;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(this.gameObject);
                return;
            }
            ActivityMap = CreateActivityMap();
        }

        private void OnEnable()
        {
            GameEventsManager.Instance.ActivityEvents.OnStartActivity += StartActivity;
            GameEventsManager.Instance.ActivityEvents.OnAdvanceActivity += AdvanceActivity;
            GameEventsManager.Instance.ActivityEvents.OnFinishActivity += FinishActivity;
            GameEventsManager.Instance.ActivityEvents.OnActivityStepStateChange += ActivityStepStateChange;
        }

        private void OnDisable()
        {
            GameEventsManager.Instance.ActivityEvents.OnStartActivity -= StartActivity;
            GameEventsManager.Instance.ActivityEvents.OnAdvanceActivity -= AdvanceActivity;
            GameEventsManager.Instance.ActivityEvents.OnFinishActivity -= FinishActivity;
            GameEventsManager.Instance.ActivityEvents.OnActivityStepStateChange -= ActivityStepStateChange;
        }

        private void Start()
        {
            if (ActivityMap == null) throw new UnityException($"No activity map made in {this.name}");
            
            // Tests how long it takes to spawn each step + destroy them (To get the descriptions on each object) 
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var activity in ActivityMap.Values)
            { 
                activity.SetStepDescriptionList(this.transform);
                
                GameEventsManager.Instance.ActivityEvents.QuestStateChange(activity);
            }
            stopwatch.Stop();
            Debug.Log($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        }
        
        private void ChangeActivityState(ActivityInfoSo activityInfoSo, ActivityState state)
        {
            Activity activity = GetActivityById(activityInfoSo.ID);
            activity.State = state;
            GameEventsManager.Instance.ActivityEvents.QuestStateChange(activity);
        }

        private bool CheckRequirementsMet(Activity activity)
        {
            foreach (var infoQuestPrerequisite in activity.Info.activityPrerequisites)
            {
                // Already proven that it doesn't meet the requirements
                if (GetActivityById(infoQuestPrerequisite.ID).State != ActivityState.FINISHED)
                {
                    return false;
                }
            }
            
            return true;
        }

        private void Update()
        {
            foreach (var activity in ActivityMap.Values)
            {
                if (activity.State == ActivityState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(activity))
                    ChangeActivityState(activity.Info, ActivityState.CAN_START);
            }
        }

        private void StartActivity(ActivityInfoSo activityInfoSo)
        {
            Activity activity = GetActivityById(activityInfoSo.ID);
            if (!activity.State.Equals(ActivityState.CAN_START))
            {
                Debug.LogWarning($"{activity.Info.name} is not ready to be started or is in progress: State {activity.State}");
                return;
            }

            // Debug.Log("Start Activity");
            
            activity.InstantiateCurrentActivityStep(this.transform);
            ChangeActivityState(activity.Info, ActivityState.IN_PROGRESS);
        }
        
        private void AdvanceActivity(ActivityInfoSo activityInfoSo)
        {
            Activity activity = GetActivityById(activityInfoSo.ID);
            if (!activity.State.Equals(ActivityState.IN_PROGRESS))
            {
                Debug.LogWarning($"{activity.Info.name} is not in progress: State {activity.State}");
                return;
            }
            
            activity.MoveToNextStep();
            // Debug.Log("Advance Activity");
            if (activity.CurrentStepExits())
                activity.InstantiateCurrentActivityStep(this.transform);
            else
            {
                // No more steps so we can finish the quest
                ChangeActivityState(activity.Info, ActivityState.CAN_FINISH);
            }
        }
        
        private void FinishActivity(ActivityInfoSo activityInfoSo)
        {
            Activity activity = GetActivityById(activityInfoSo.ID);
            // Debug.Log("Finish Activity");
            // Only change rewards if we can claim them. E.g. there aren't enough place for reward item
            if (ClaimRewards(activity)) 
                ChangeActivityState(activity.Info, ActivityState.FINISHED);
        }

        private bool ClaimRewards(Activity activity)
        {
            // What kind of reward the activity game
            return true;
        }
        
        private void ActivityStepStateChange(ActivityInfoSo activityInfoSo, int stepIndex, ActivityStepState activityStepState)
        {
            Activity activity = GetActivityById(activityInfoSo.ID);
            activity.StoreActivityStepState(activityStepState, stepIndex);
            ChangeActivityState(activityInfoSo, activity.State); // Reload quest state listeners
        }

        private Dictionary<string, Activity> CreateActivityMap()
        {
            // Get all info for quests
            ActivityInfoSo[] allActivities = UnityEngine.Resources.LoadAll<ActivityInfoSo>($"Activities");
            
            var idToQuestMap = new Dictionary<string, Activity>();
            foreach (var activityInfoSo in allActivities)
            {
                if (idToQuestMap.ContainsKey(activityInfoSo.ID))
                    Debug.LogWarning($"Duplicate Quest: {activityInfoSo.ID} when creating quest map");
                
                idToQuestMap.Add(activityInfoSo.ID, new Activity(activityInfoSo)); // Loads the quest
            }
            return idToQuestMap;
        }

        public Activity GetActivityById(string id)
        {
            Activity activity = ActivityMap[id];
            if (activity == null)
                Debug.LogError($"Quest {id} not found in quest map");
            return activity;
        }
    }
}