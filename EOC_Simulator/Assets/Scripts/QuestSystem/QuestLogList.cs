using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    public class QuestLogList : MonoBehaviour
    {
        [SerializeField] private GameObject contentParent;
        [SerializeField] private GameObject questLogButtonPrefab;
        private readonly Dictionary<string, QuestLogButton> _questLogMap = new Dictionary<string, QuestLogButton>();
        
        [SerializeField] private QuestManager questManager;

        private void Start()
        {
            foreach (var questPointQuest in questManager.quests)
            {
                QuestLogButton logButton = Instantiate(questLogButtonPrefab, contentParent.transform).GetComponentInChildren<QuestLogButton>();
                logButton.gameObject.name = questPointQuest.questDescription + "_Button";
                logButton.InstantiateButton(questPointQuest);
            
                _questLogMap[questPointQuest.name] = logButton;
            }

            questManager.OnQuestStepCompleted += OnQuestStepComplete;
        }

        private void OnQuestStepComplete(QuestStep questStep)
        {
            // Finished the quest that was active
            _questLogMap[questStep.name].FinishedQuest();
        }
        
    }
}