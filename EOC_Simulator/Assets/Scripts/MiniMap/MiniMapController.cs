using System;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    private Transform _player; // 绑定玩家

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        if (!_player) throw new UnityException($"Player not found in the scene");
    }

    void LateUpdate()
    {
        if (!_player) return;
        // 让 MiniMapCamera 始终跟随玩家位置，但固定高度
        transform.position = new Vector3(_player.position.x, transform.position.y, _player.position.z);
         
        Quaternion newRotation = Quaternion.Euler(90f, _player.transform.rotation.eulerAngles.y, 0f);
        transform.rotation = newRotation;
    }
}