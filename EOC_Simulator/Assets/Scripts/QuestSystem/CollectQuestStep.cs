using Interactable;
using UnityEngine;

namespace QuestSystem
{
    public class CollectQuestStep : QuestStep
    {
        private void Start()
        {
            if (!Interactable) throw new UnityException($"Interactable component not set {gameObject.name}");
            Interactable.OnInteracted += InteractedWithObject;
        }

        private void InteractedWithObject()
        {
            if (status != QuestStepStatus.IS_ACTIVE) return;
            QuestComplete();
        }
    }
}