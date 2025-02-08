using System;
using Activity_System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ActivityLogButton : MonoBehaviour, ISelectHandler
    {
        public Button Button { get; private set; }
        [SerializeField] private RectTransform parentRowRectTransform;
        [SerializeField] private TMP_Text buttonText;
        private const string FinishedSymbol = "X";
        [SerializeField] private TMP_Text activityLogText;

        public Activity Activity {get; private set;}

        public void InstantiateButton(Activity activity)
        {
            Button = gameObject.GetComponent<Button>();
            Activity = activity;
            // activityLogText.alignment = TextAlignmentOptions.Midline;

            SetBaseText();
            UpdateSizeBasedOnText();
        }

        private void SetBaseText()
        {
            activityLogText.text = Activity.Info.displayName;
        }
        
        public void OnSelect(BaseEventData eventData)
        {
        }

        public void StartAndUpdateActivityBtn()
        {
            // Add to the button text
            string activityActionName = $"{Activity.Info.displayName}";
            
            /*
             * Check each stepdescription, add them all.
             * Remove the one where it's a part of the 
             */
            foreach (string stepDescription in Activity.ActivityStepDescriptions)
            {
                activityActionName += $"\n  - {stepDescription}";
            }
            
            activityLogText.text = activityActionName;
            UpdateSizeBasedOnText();
        }
        
        public void FinishActivityBtn()
        {
            buttonText.text = FinishedSymbol;
            Debug.Log($"Finished Activity: {Activity.Info.displayName}");
            // Grayed out
            activityLogText.text = $"{Activity.Info.displayName}";
            UpdateSizeBasedOnText();

        }

        // private void Update()
        // {
        //     UpdateSizeBasedOnText();
        // }

        private void UpdateSizeBasedOnText()
        {
            float heightSize = TmpTextHeight(activityLogText);
            // Debug.Log($"{Activity.Info.displayName}: Height: {heightSize}");
            if (parentRowRectTransform.sizeDelta.y < heightSize)
            {
                // activityLogText.alignment = TextAlignmentOptions.TopLeft;
                TmpTextHeight(activityLogText);
            }
            else
            {
                heightSize = parentRowRectTransform.sizeDelta.y;
            }
            
            parentRowRectTransform.sizeDelta = new Vector2(parentRowRectTransform.sizeDelta.x, heightSize);
            activityLogText.rectTransform.sizeDelta = new Vector2(activityLogText.rectTransform.sizeDelta.x, heightSize);
        }
        

        private float TmpTextHeight(TMP_Text text)
        {
            text.ForceMeshUpdate();
            
            // text.rect.sizedelta change to same as height
            // Add previous height to it. The base height:...
            int lineCount = text.textInfo.lineCount + 1; // Since the line for the padding on the sides also need to be there
            float lineHeight = text.fontSize * (1 + text.lineSpacing);
            return (lineCount * lineHeight);
        }
        

    }
}
