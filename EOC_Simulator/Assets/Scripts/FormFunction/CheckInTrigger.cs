using UnityEngine;

public class CheckInTrigger : MonoBehaviour
{
    public CheckInForm checkInForm; // 绑定 Check-in 表单
    public GameObject interactText; // "Press [E] to Check-in" 提示
    public TaskManager taskManager;
    private bool isPlayerNearby = false; // 玩家是否靠近

    void Start()
    {
        interactText.SetActive(false);
    }

    void Update()
    {
        // 只有靠近时才能交互
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            checkInForm.OpenForm();
            taskManager.CompleteCurrentTask();
            Debug.Log("Task 2 (Sign In Form) completed.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 确保是玩家
        {
            isPlayerNearby = true;
            interactText.SetActive(true); // 显示提示
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactText.SetActive(false); // 隐藏提示
        }
    }
}