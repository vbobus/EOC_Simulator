using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MiniMapPlayerIcon : MonoBehaviour
{
    [HideInInspector] public Transform player; // 绑定玩家
    private RectTransform iconTransform;

    void Start()
    {
        iconTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (player != null)
        {
            // 让 PlayerIcon 随着玩家旋转
            iconTransform.rotation = Quaternion.Euler(0, 0, -player.eulerAngles.y);
        }
    }
}