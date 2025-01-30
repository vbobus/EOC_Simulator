using UnityEngine;

namespace Activity_System
{
    public class ActivityIcon : MonoBehaviour
    {
        [Header("Icons")]
        [SerializeField] private GameObject requirementsNotMetStartIcon;
        [SerializeField] private GameObject canStartIcon;
        [SerializeField] private GameObject requirementsNotMetFinishIcon;
        [SerializeField] private GameObject canFinishIcon;

        public void SetState(ActivityState newState, bool startPoint, bool finishPoint)
        {
            requirementsNotMetStartIcon.SetActive(false);
            canStartIcon.SetActive(false);
            requirementsNotMetFinishIcon.SetActive(false);
            canFinishIcon.SetActive(false);

            switch (newState)
            {
                case ActivityState.REQUIREMENTS_NOT_MET:
                    if (startPoint) requirementsNotMetStartIcon.SetActive(true);
                    break;
                case ActivityState.CAN_START:
                    if (startPoint) canStartIcon.SetActive(true);
                    break;
                case ActivityState.IN_PROGRESS:
                    if (finishPoint) requirementsNotMetFinishIcon.SetActive(true);
                    break;
                case ActivityState.CAN_FINISH:
                    if (startPoint) canFinishIcon.SetActive(true);
                    break;
                case ActivityState.FINISHED:
                    break;
            }
        }
    }
}
