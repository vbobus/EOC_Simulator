using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Transform player; // 绑定玩家

    void LateUpdate()
    {
        if (player != null)
        {
            // 让 MiniMapCamera 始终跟随玩家位置，但固定高度
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        }
    }
}