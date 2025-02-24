using UnityEngine;

public class PlayerStress : MonoBehaviour
{
    public float stressLevel = 0f; // 当前压力值
    public float stressIncreaseRate = 10f; // 每秒增加压力值
    public float stressDecreaseRate = 10f; // 每秒减少压力值
    private bool isInStressZone = false;
    private bool isInRelaxZone = false;
    public TaskManager taskManager;

    void Update()
    {
        if (isInStressZone)
        {
            // 在压力区时，逐渐增加压力值
            stressLevel += stressIncreaseRate * Time.deltaTime;
        }
        else if (isInRelaxZone)
        {
            // 在恢复区时，逐渐减少压力值
            stressLevel -= stressDecreaseRate * Time.deltaTime;
        }

        // 限制压力值范围在 0 - 100 之间
        stressLevel = Mathf.Clamp(stressLevel, 0f, 100f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StressZone"))
        {
            isInStressZone = true;
            taskManager.CompleteCurrentTask();
            Debug.Log("Task 1 (Stress Zone) completed.");
        }
        else if (other.CompareTag("RelaxZone"))
        {
            isInRelaxZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StressZone"))
        {
            isInStressZone = false;
        }
        else if (other.CompareTag("RelaxZone"))
        {
            isInRelaxZone = false;
        }
    }
}