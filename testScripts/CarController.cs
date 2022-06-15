using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider[] wheels = new WheelCollider[4];
    // 차량 모델의 바퀴 부분 4개
    GameObject[] wheelMesh = new GameObject[4];

    public float power = 100f; // 바퀴를 회전시킬 힘
    public float rot = 45f; // 바퀴의 회전 각도
    Rigidbody rb;

    void WheelPosAndAni()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }


    void Start()
    {
        // 바퀴 모델을 태그를 통해서 찾아온다.(차량이 변경되더라도 자동으로 찾기위해서)
        wheelMesh = GameObject.FindGameObjectsWithTag("WheelMesh");

        for (int i = 0; i < wheelMesh.Length; i++)
        {	// 휠콜라이더의 위치를 바퀴메쉬의 위치로 각각 이동시킨다.
            wheels[i].transform.position = wheelMesh[i].transform.position;
        }

        rb = GetComponent<Rigidbody>();
        // 무게 중심을 y축 아래방향으로 낮춘다.
        rb.centerOfMass = new Vector3(0, -1, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        WheelPosAndAni();

        for (int i = 0; i < wheels.Length; i++)
        {
            // for문을 통해서 휠콜라이더 전체를 Vertical 입력에 따라서 power만큼의 힘으로 움직이게한다.
            wheels[i].motorTorque = Input.GetAxis("Vertical") * power;
        }
        for (int i = 0; i < 2; i++)
        {
            // 앞바퀴만 각도전환이 되어야하므로 for문을 앞바퀴만 해당되도록 설정한다.
            wheels[i].steerAngle = Input.GetAxis("Horizontal") * rot;
        }
    }
}
