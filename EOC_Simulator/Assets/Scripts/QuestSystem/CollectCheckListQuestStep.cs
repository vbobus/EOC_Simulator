using UnityEngine;

namespace QuestSystem
{
    public class CollectCheckListQuestStep : CollectQuestStep
    {
        // Collider that stops the player from moving to the next room
        [SerializeField] private GameObject doorCollider;

        public override void QuestComplete()
        {
            base.QuestComplete();
            doorCollider.SetActive(false);
        }
    }
}