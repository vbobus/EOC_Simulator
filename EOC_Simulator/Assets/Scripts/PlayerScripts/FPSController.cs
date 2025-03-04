using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // 移动速度
    public float sprintSpeed = 8f; // 奔跑速度
    public float jumpForce = 5f; // 跳跃力度
    public float gravity = 9.8f; // 重力加速度
    public float mouseSensitivity = 2f; // 鼠标灵敏度

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    public Camera playerCamera;
    private float xRotation = 0f;

    private bool isCursorVisible = false; // 记录鼠标状态

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // 初始化时隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MovePlayer();
        RotateView();
        ToggleCursor();
    }

    void MovePlayer()
    {
        // 检测是否在地面
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 获取玩家输入
        float moveX = Input.GetAxis("Horizontal"); // A/D 或 左右箭头
        float moveZ = Input.GetAxis("Vertical"); // W/S 或 上下箭头

        // 计算移动方向
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed; // 按住 Shift 加速
        controller.Move(moveDirection * speed * Time.deltaTime);

        // 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * 2f * gravity);
        }

        // 应用重力
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void RotateView()
    {
        if (!isCursorVisible) // 当鼠标隐藏时，才能进行视角旋转
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // 控制 X 轴旋转（上下）
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // 控制 Y 轴旋转（左右）
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    void ToggleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // 按 Q 切换鼠标状态
        {
            isCursorVisible = !isCursorVisible; // 切换状态
            Cursor.visible = isCursorVisible;
            Cursor.lockState = isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}