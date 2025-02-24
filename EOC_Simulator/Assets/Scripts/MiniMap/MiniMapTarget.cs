using UnityEngine;
using UnityEngine.UI;

public class MiniMapTarget : MonoBehaviour
{
    public Transform target;          // 目标（Stress Zone）
    public Transform player;          // 玩家
  
    public RectTransform miniMapPanel; // MiniMap 的 UI Panel
    private RectTransform iconTransform;

    [SerializeField] private float mapScale = 30f; // MiniMap 缩放系数
    [SerializeField] private bool clampToEdge = true; // 是否固定在 MiniMap 边缘

    private bool hasInteracted = false;
    void Start()
    {
        iconTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        // 如果任务已完成，则隐藏图标和文字
        if (hasInteracted)
        {
            if (iconTransform.gameObject.activeSelf)
                iconTransform.gameObject.SetActive(false);
            
            return;
        }
        if (target == null || player == null || miniMapPanel == null) return;

        // **计算目标在世界坐标中的相对位置**
        Vector3 relativePos = player.InverseTransformPoint(target.position); // 转换到玩家局部坐标
        Vector2 miniMapPos = new Vector2(relativePos.x, relativePos.z) * mapScale; // 映射到 MiniMap

        // **确保 Target 在 MiniMap 上保持正确相对位置**
        float halfWidth = miniMapPanel.rect.width / 2;
        float halfHeight = miniMapPanel.rect.height / 2;

        if (clampToEdge)
        {
            // **如果目标超出 MiniMap 范围，固定在 MiniMap 边缘**
            if (Mathf.Abs(miniMapPos.x) > halfWidth || Mathf.Abs(miniMapPos.y) > halfHeight)
            {
                float angle = Mathf.Atan2(miniMapPos.y, miniMapPos.x);
                miniMapPos.x = Mathf.Cos(angle) * halfWidth;
                miniMapPos.y = Mathf.Sin(angle) * halfHeight;
            }
        }

        // **更新 Target Icon 的 UI 位置**
        iconTransform.anchoredPosition = miniMapPos;
    }
    // 外部调用，当任务完成后隐藏图标和文字
    public void MarkAsInteracted()
    {
        hasInteracted = true;
    }
}