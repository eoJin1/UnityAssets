using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed = 10.0f;

    public float rotateSpeed = 10.0f;

    public float jumpForce = 10.0f;          // 점프하는 힘

    private bool isGround = true;           // 캐릭터가 땅에 있는지 확인할 변수

    Rigidbody body;                         // 컴포넌트에서 RigidBody를 받아올 변수

    float h, v;

    // 유니티 실행과 동시에 한번 실행되는 함수
    void Start()
    {
        body = GetComponent<Rigidbody>();   // GetComponent를 활용하여 body에 해당 오브젝트의 Rigidbody를 넣어준다.
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 이동 관련 함수를 짤 때는 Update보다 FixedUpdate가 더 효율이 좋다고 한다. 그래서 사용했다.
    void FixedUpdate()
    {
        Move();
        Jump();
    }

    // 이전 FixedUpdate에 있던 것을 보기 좋게 묶기 위해 Move로 옮김
    void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);

        if (!(h == 0 && v == 0))
        {
            transform.position += dir * Speed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
        }
    }

    void Jump()
    {
        // 스페이스바를 누르면(또는 누르고 있으면), 그리고 캐릭터가 땅에 있다면
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            // body에 힘을 가한다(AddForce)
            // AddForce(방향, 힘을 어떻게 가할 것인가)
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // 땅에서 떨어졌으므로 isGround를 false로 바꿈
            isGround = false;
        }
    }

    // 충돌 함수
    void OnCollisionEnter(Collision collision)
    {
        // 부딪힌 물체의 태그가 "Ground"라면
        if (collision.gameObject.CompareTag("Ground"))
        {
            // isGround를 true로 변경
            isGround = true;
        }
    }
}