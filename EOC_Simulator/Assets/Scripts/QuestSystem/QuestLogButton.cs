using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QuestSystem
{
    public class QuestLogButton : MonoBehaviour, ISelectHandler
    {
        public Button Button { get; private set; }
        [SerializeField] private RectTransform parentRowRectTransform;
        [SerializeField] private TMP_Text buttonText;
        private const string FinishedSymbol = "X";
        [SerializeField] private TMP_Text activityLogText;

        private QuestStep _questStep;
        
        public void InstantiateButton(QuestStep questStep)
        {
            Button = gameObject.GetComponent<Button>();
            this._questStep = questStep;
            SetBaseText();
        }
        
        private void SetBaseText()
        {
            activityLogText.text = _questStep.questDescription;
        }

        /// <summary>
        /// Ticks of the checkbox 
        /// </summary>
        public void FinishedQuest()
        {
            buttonText.text = FinishedSymbol;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            
        }
    }
}