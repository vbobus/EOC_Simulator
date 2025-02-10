using Interactable;
using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    public enum QuestStepStatus 
    {
        CANT_START,
        IS_ACTIVE,
        FINISHED,
    }
    
    
    public abstract class QuestStep : MonoBehaviour
    {
        public string questDescription = "Collect__";

        public UnityAction OnQuestStarted { get; set; }
        public UnityAction OnQuestComplete { get; set; }
        [HideInInspector] public QuestStepStatus status;
        protected InteractableObject Interactable;

        private void Awake()
        {
            Interactable = GetComponent<InteractableObject>();
        }

        
        public void ChangeStatus(QuestStepStatus newStatus)
        {
            Debug.Log($"Previous status {status}: Changed status to {newStatus}");
            status = newStatus;

            switch (status)
            {
                case QuestStepStatus.IS_ACTIVE:
                    OnQuestStarted?.Invoke();
                    if (Interactable) Interactable.ShowHideOutline(true);
                    break;
                case QuestStepStatus.CANT_START:
                case QuestStepStatus.FINISHED:
                    if (Interactable) Interactable.ShowHideOutline(false);
                    break;
            }
        }

        public virtual void QuestComplete()
        {
            OnQuestComplete?.Invoke();
            if (Interactable) Interactable.HideObject();
        }
    }
}