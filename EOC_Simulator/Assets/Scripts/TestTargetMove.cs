using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestTargetMove : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform target;
    [SerializeField] private InputActionReference pointer;
    [SerializeField] private float smoothSpeed = 10f; // Adjust for smoother movement

    private Vector3 _targetPosition;

    private void Update()
    {
        if (!Application.isFocused) return;
        
        Ray ray = Camera.main.ScreenPointToRay(pointer.action.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            _targetPosition = hit.point; // Store the target position
        }

        // Smoothly move the target towards the new position
        target.position = Vector3.Lerp(target.position, _targetPosition, smoothSpeed * Time.deltaTime);
    }
}