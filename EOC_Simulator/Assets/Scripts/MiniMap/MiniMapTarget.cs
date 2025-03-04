using UnityEngine;
using UnityEngine.UI;

public class MiniMapTarget : MonoBehaviour
{
    [HideInInspector] public Transform player;          // 玩家
    public Transform target;          // 目标（Stress Zone）
  
    [HideInInspector] public RectTransform miniMapPanel; // MiniMap 的 UI Panel
    private RectTransform iconTransform;

    
    [Tooltip("This variable has to be changed, when we change the size of the MiniMap Camera (how far it can look)")] 
    [SerializeField] private float mapScale = 30f; // MiniMap 缩放系数    Need to make this happen in code, since its dependent on another variable in the Camera
    [SerializeField] private bool clampToEdge = true; // 是否固定在 MiniMap 边缘

    private bool hasInteracted = false;
    private Image _icon;
    void Awake()
    {
        iconTransform = GetComponent<RectTransform>();
        _icon = GetComponent<Image>();
    }

    public void Show(bool show)
    {
        _icon.enabled = show;
        Debug.Log($"Show icon {show}");
    }
    
    void Update()
    {
        if (target == null || player == null || miniMapPanel == null)
        {
            return;
        }
        
        // Calculate the target's relative position in world coordinates
        Vector3 relativePos = player.InverseTransformPoint(target.position); // Convert to player's local coordinates
        Vector2 miniMapPos = new Vector2(relativePos.x, relativePos.z) * mapScale; // Map to MiniMap coordinates

        // Get the minimap radius (assuming it's a square panel with a circular shape)
        float miniMapRadius = miniMapPanel.rect.width / 2;

        if (clampToEdge)
        {
            // Check if the position is outside the minimap circle
            float distanceFromCenter = miniMapPos.magnitude;
    
            if (distanceFromCenter > miniMapRadius)
            {
                // Clamp to the edge of the minimap circle
                float angle = Mathf.Atan2(miniMapPos.y, miniMapPos.x);
                miniMapPos.x = Mathf.Cos(angle) * miniMapRadius;
                miniMapPos.y = Mathf.Sin(angle) * miniMapRadius;
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