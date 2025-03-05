using System.Collections;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace UI.Quest
{
    public class SingleUIQuestTracker: StandardUIQuestTracker
    {
        /*protected override IEnumerator RefreshAtEndOfFrame()
        {
            // yield return new WaitForSecondsRealtime(3f);

            bool containsQuest = false;
            QuestState flags = (showActiveQuests ? (QuestState.Active | QuestState.ReturnToNPC) : 0) |
                               (showCompletedQuests ? (QuestState.Success | QuestState.Failure) : 0);
            foreach (string quest in QuestLog.GetAllQuests(flags))
            {
                if (QuestLog.IsQuestTrackingEnabled(quest))
                {
                    AddQuestTrack(quest);
                    containsQuest = true;
                }
            }

            var quests = QuestLog.GetAllQuests(flags);
            
            Debug.Log($"Quests counts: {quests.Length}: Show container: showifempty {showContainerIfEmpty}: containsQuest: {containsQuest}");
            container?.gameObject.SetActive(showContainerIfEmpty || containsQuest);
            refreshCoroutine = null;

            yield return null;
        }

        protected override void AddQuestTrack(string quest)
        {
            if (container == null || questTrackTemplate == null) return;

            var heading = GetQuestHeading(quest);
            SetupQuestTrackInstance(questTrackTemplate, quest, heading);

            /*GameObject go;
            
            
            if (unusedInstances.Count > 0)
            {
                // Try to use an unused instance:
                go = unusedInstances[0].gameObject;
                unusedInstances.RemoveAt(0);
            }
            else
            {
                // Otherwise instantiate one:
                go = Instantiate(questTrackTemplate.gameObject) as GameObject;
                Debug.Log($"Adding quest track {quest}");
                if (go == null)
                {
                    Debug.LogError(string.Format("{0}: {1} couldn't instantiate quest track template", new object[] { DialogueDebug.Prefix, name }));
                    return;
                }
            }
            
            go.name = heading;
            go.transform.SetParent(container.transform, false);
            go.SetActive(true);
            var questTrack = go.GetComponent<StandardUIQuestTrackTemplate>();
            instantiatedItems.Add(questTrack);
            if (questTrack != null)
            {
                SetupQuestTrackInstance(questTrack, quest, heading);
                questTrack.transform.SetSiblingIndex(siblingIndexCounter++);
            }#1#
        }

        protected override void SetupQuestTrackInstance(StandardUIQuestTrackTemplate questTrack, string quest, string heading)
        {
            if (questTrack == null) return;
            questTrack.gameObject.SetActive(true);
            
            questTrack.Initialize();
            var questState = QuestLog.GetQuestState(quest);
            questTrack.SetDescription(heading, questState);
            int entryCount = QuestLog.GetQuestEntryCount(quest);
            for (int i = 1; i <= entryCount; i++)
            {
                var entryState = QuestLog.GetQuestEntryState(quest, i);
                var entryText = FormattedText.Parse(GetQuestEntryText(quest, i, entryState), DialogueManager.masterDatabase.emphasisSettings).text;
                if (!string.IsNullOrEmpty(entryText))
                {
                    questTrack.AddEntryDescription(entryText, entryState);
                }
            }
        }*/
    }
}