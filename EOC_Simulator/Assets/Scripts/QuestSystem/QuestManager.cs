using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    public class QuestManager: MonoBehaviour
    {
        public List<QuestStep> quests = new();
        private int _currentActiveQuestIndex = 0; 
        public UnityAction<QuestStep> OnQuestStepCompleted;
        
        private void Awake()
        {
            // Start the first quest
            StartQuest();
        }

        private void StartQuest()
        {
            if (_currentActiveQuestIndex >= quests.Count)
            {
                Debug.Log($"No more quests");
                return;
            }
            
            quests[_currentActiveQuestIndex].ChangeStatus(QuestStepStatus.IS_ACTIVE);
            quests[_currentActiveQuestIndex].OnQuestComplete += OnQuestComplete;
        }

        private void OnQuestComplete()
        {
            quests[_currentActiveQuestIndex].ChangeStatus(QuestStepStatus.FINISHED);
            quests[_currentActiveQuestIndex].OnQuestComplete -= OnQuestComplete;
            
            OnQuestStepCompleted?.Invoke(quests[_currentActiveQuestIndex]);
            
            _currentActiveQuestIndex++;
            StartQuest(); // Starts the next quest
        }
    }
}