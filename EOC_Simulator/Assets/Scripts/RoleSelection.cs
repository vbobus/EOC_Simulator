using UnityEngine;
using UnityEngine.SceneManagement;

public class RoleSelection : MonoBehaviour
{
    public void SelectRole(string roleName)
    {
        PlayerPrefs.SetString("SelectedRole", roleName);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("LLM-Quest");
    }
}