using Interactable;
using UnityEngine;

namespace QuestSystem
{
    public class CollectQuestStep : QuestStep
    {
        private void Start()
        {
            Interactable.OnInteracted += InteractedWithObject;
        }

        private void InteractedWithObject()
        {
            if (status != QuestStepStatus.IS_ACTIVE) return;
            QuestComplete();
        }
    }
}