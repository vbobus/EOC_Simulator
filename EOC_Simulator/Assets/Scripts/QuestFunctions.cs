using UnityEngine;
using Activity_System;
using Events;

public static class QuestFunctions
{
    /// <summary>
    /// Starts the quest if its current state allows it.
    /// </summary>
    public static string StartQuest(string questID)
    {
        Activity activity = ActivityManager.Instance.GetActivityById(questID);
        if (activity == null)
            return $"Quest {questID} not found.";

        if (activity.State == ActivityState.CAN_START)
        {
            // Trigger the start event for this quest.
            GameEventsManager.Instance.ActivityEvents.StartActivity(activity.Info);
            return $"Quest {questID} has been started.";
        }
        return $"Quest {questID} is not ready to be started (current state: {activity.State}).";
    }

    /// <summary>
    /// Advances the quest if it is in progress.
    /// </summary>
    public static string AdvanceQuest(string questID)
    {
        Activity activity = ActivityManager.Instance.GetActivityById(questID);
        if (activity == null)
            return $"Quest {questID} not found.";

        if (activity.State == ActivityState.IN_PROGRESS)
        {
            // Trigger the advance event for this quest.
            GameEventsManager.Instance.ActivityEvents.AdvanceQuest(activity.Info);
            return $"Quest {questID} has been advanced.";
        }
        return $"Quest {questID} cannot be advanced (current state: {activity.State}).";
    }

    /// <summary>
    /// Completes the quest if it is ready to be finished.
    /// </summary>
    public static string CompleteQuest(string questID)
    {
        Activity activity = ActivityManager.Instance.GetActivityById(questID);
        if (activity == null)
            return $"Quest {questID} not found.";

        if (activity.State == ActivityState.CAN_FINISH)
        {
            // Trigger the finish event for this quest.
            GameEventsManager.Instance.ActivityEvents.FinishActivity(activity.Info);
            return $"Quest {questID} has been completed.";
        }
        return $"Quest {questID} cannot be completed (current state: {activity.State}).";
    }
}
