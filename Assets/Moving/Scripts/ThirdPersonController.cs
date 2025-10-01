using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement & Speed")]
    public float moveSpeed = 6.0f;           // �⺻ �̵� �ӵ�

    [Header("Jump & Gravity")]
    public float jumpHeight = 2.0f;          // ���� ����
    public float gravity = -9.81f * 2.5f;    // �߷� ���ӵ�

    [Header("Rotation Stabilization")]
    // Slerp ������� ȸ���Ͽ� ȸ�� �ӵ��� ��Ȯ�ϰ� �����մϴ�. (SmoothDamp�� ���� ���� ���� �ذ�)
    public float rotationSpeed = 720f;       // �ʴ� ȸ�� ���� (Degrees Per Second). Ŭ���� �����ϴ�.
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
            // 1. ī�޶� ������ ���� �̵� ���� ���
            Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;

            Vector3 finalMoveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

            // 2. �̵� ���� ����
            moveVelocity = finalMoveDirection * moveSpeed;

            // 3. ȸ�� ���� (Slerp�� �ƴ� RotateTowards�� ����� �ӵ��� ��Ȯ�� ����)
            // ���� �Է�(verticalInput < 0)�� �ƴ� ���� ȸ�� ��ǥ�� �����մϴ�.
            if (verticalInput >= -0.01f)
            {
                // ��ǥ ȸ���� ���: �̵� ����(finalMoveDirection)�� �ٶ󺸵���
                Quaternion targetRotation = Quaternion.LookRotation(finalMoveDirection);

                // RotateTowards�� ����Ͽ� ������ ���ӵ�(rotationSpeed)��ŭ ȸ��
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
            // else: ���� ���� ���� ȸ�� ������ �����ϰ� ���� �ٶ󺸴� ������ �����մϴ�.
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