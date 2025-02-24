using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckInForm : MonoBehaviour
{
    public GameObject formPanel;  // Check-in UI
    public TMP_InputField nameInput;
    public TMP_InputField checkInInput;
    public TMP_Dropdown positionDropdown;
    public Button closeButton;
    public FPSController playerMovement; // 🔹 绑定玩家移动脚本

    private bool isFormOpen = false;

    void Start()
    {
        formPanel.SetActive(false); // 初始隐藏表单
        closeButton.onClick.AddListener(CloseForm); // 绑定关闭按钮
    }

    public void OpenForm()
    {
        if (isFormOpen) return;

        formPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true;
        isFormOpen = true;

        // **禁用玩家移动**
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 自动填充 Check-in 时间
        checkInInput.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    public void CloseForm()
    {
        formPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // 重新锁定鼠标
        Cursor.visible = false;
        isFormOpen = false;

        // **恢复玩家移动**
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}