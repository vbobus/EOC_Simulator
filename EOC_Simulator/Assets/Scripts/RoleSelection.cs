using UnityEngine;
using UnityEngine.SceneManagement;

public class RoleSelection : MonoBehaviour
{
    public void SelectRole(string roleName)
    {
        // Store the selected role
        PlayerPrefs.SetString("SelectedRole", roleName);
        PlayerPrefs.Save();

        // Load the main game scene
        SceneManager.LoadScene("LLMTest");
    }
}