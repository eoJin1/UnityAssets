using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement & Speed")]
    public float moveSpeed = 6.0f;           // 기본 이동 속도

    [Header("Jump & Gravity")]
    public float jumpHeight = 2.0f;          // 점프 높이
    public float gravity = -9.81f * 2.5f;    // 중력 가속도

    [Header("Rotation Stabilization")]
    // Slerp 방식으로 회귀하여 회전 속도를 명확하게 제어합니다. (SmoothDamp의 휙휙 도는 문제 해결)
    public float rotationSpeed = 720f;       // 초당 회전 각도 (Degrees Per Second). 클수록 빠릅니다.
    private const float MIN_MOVE_THRESHOLD = 0.05f;

    private Vector3 moveDirection;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleGravityAndJump();
        Vector3 horizontalMovement = HandleMovementAndRotation();

        moveDirection = horizontalMovement;
        moveDirection.y = verticalVelocity;

        controller.Move(moveDirection * Time.deltaTime);
    }

    // --- Movement and Rotation ---

    private Vector3 HandleMovementAndRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 moveVelocity = Vector3.zero;

        if (inputDirection.magnitude >= MIN_MOVE_THRESHOLD)
        {
            // 1. 카메라 기준의 월드 이동 방향 계산
            Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;

            Vector3 finalMoveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

            // 2. 이동 벡터 설정
            moveVelocity = finalMoveDirection * moveSpeed;

            // 3. 회전 로직 (Slerp가 아닌 RotateTowards를 사용해 속도를 명확히 제어)
            // 후진 입력(verticalInput < 0)이 아닐 때만 회전 목표를 설정합니다.
            if (verticalInput >= -0.01f)
            {
                // 목표 회전값 계산: 이동 방향(finalMoveDirection)을 바라보도록
                Quaternion targetRotation = Quaternion.LookRotation(finalMoveDirection);

                // RotateTowards를 사용하여 지정된 각속도(rotationSpeed)만큼 회전
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            // else: 후진 중일 때는 회전 로직을 생략하고 현재 바라보는 방향을 유지합니다.
        }

        return moveVelocity;
    }

    // --- Gravity and Jump ---

    private void HandleGravityAndJump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity += gravity * Time.deltaTime;
    }
}