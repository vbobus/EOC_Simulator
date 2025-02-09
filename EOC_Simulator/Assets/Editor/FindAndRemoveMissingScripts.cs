// Create a new script named FindAndRemoveMissingScripts.cs and place it in an Editor folder
using UnityEditor;
using UnityEngine;

public class FindAndRemoveMissingScripts : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    public static void FindMissingScriptsInScene()
    {
        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();
        int goCount = 0, componentsCount = 0, missingCount = 0;
        foreach (GameObject g in gos)
        {
            goCount++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                componentsCount++;
                if (components[i] == null)
                {
                    missingCount++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.LogError(s + " has an empty script attached in position: " + i, g);
                }
            }
        }

        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", goCount, componentsCount, missingCount));
    }
}