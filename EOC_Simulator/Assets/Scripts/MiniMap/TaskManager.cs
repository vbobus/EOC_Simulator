using UnityEngine;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
    // 按任务顺序存放各个任务点的小地图图标（GameObject）
    public List<MiniMapTarget> taskDots;

    private int currentTaskIndex = 0;

    void Start()
    {
        // 初始时，仅激活第一个任务点，其他任务点隐藏
        for (int i = 0; i < taskDots.Count; i++)
        {
            if (i == currentTaskIndex)
                taskDots[i].gameObject.SetActive(true);
            else
                taskDots[i].gameObject.SetActive(false);
        }
    }

    // 当当前任务完成后调用此方法
    public void CompleteCurrentTask()
    {
        if (currentTaskIndex < taskDots.Count)
        {
            // 标记当前任务已完成（让当前任务点 dot 隐藏）
            // taskDots[currentTaskIndex].MarkAsInteracted();
            taskDots[currentTaskIndex].gameObject.SetActive(false);

            // 切换到下一个任务
            currentTaskIndex++;
            if (currentTaskIndex < taskDots.Count)
            {
                // 激活下一个任务点的 dot
                taskDots[currentTaskIndex].gameObject.SetActive(true);
            }
        }
    }
}