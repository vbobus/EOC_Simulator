using UnityEngine;
using UnityEngine.Serialization;

namespace Activity_System
{
    [CreateAssetMenu(fileName = "ActivityInfoSO", menuName = "ScriptableObjects/ActivityInfoSO", order = 1)]
    public class ActivityInfoSo : ScriptableObject
    {
        [field: SerializeField] public string ID { get;  set; }

        [Header("General")] 
        public string displayName;

        [Header("Requirements")] 
        public ActivityInfoSo[] activityPrerequisites;

        [Header("Steps")]
        public GameObject[] activityStepPrefabs;
        
        
        // Sets id based on script objects name
        private void OnValidate()
        {
            #if UNITY_EDITOR
            ID = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}
