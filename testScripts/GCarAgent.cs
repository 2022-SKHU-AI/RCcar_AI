using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

using System.IO.Ports;
using System.Threading;


public class GCarAgent : Agent
{
    SerialPort sp = new SerialPort();
    Rigidbody rBody;
    public WheelCollider[] wheels = new WheelCollider[4]; // 바퀴 4개
    private float power = 100f; // 바퀴를 회전시킬 힘
    private float rot = 45f; // 바퀴의 회전 각도
    public Transform Target; // 타겟의 위치 정보

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        // 무게 중심을 y축 아래방향으로 낮춘다.
        rBody.centerOfMass = new Vector3(0, -1, 0);

        sp.PortName = "COM5";     // 여기에는 아두이노 포트 넣어주면 됩니다.
        sp.BaudRate = 115200;      // 아두이노 보레이트랑 맞춰주시면 됩니다.
        sp.DataBits = 8;
        sp.Parity = Parity.None;
        sp.StopBits = StopBits.One;

        sp.Open();    //포트를 엽니다. 열고나면 닫힐동안 시리얼 모니터를 사용하지 못합니다.(여기서 점유하고있으므로)
    }

    public override void OnEpisodeBegin()
    {
        //새로운 에피소드가 시작하면 에이전트의 포지션의 초기화
        this.rBody.angularVelocity = Vector3.zero; // 에이전트 각속도 초기화
        this.rBody.velocity = Vector3.zero; // 에이전트 속도 초기화 2
        this.transform.localPosition = new Vector3(3,0,-3.5f); // 에이전트 위치 초기화
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);

        //Target.localPosition = new Vector3(3, 0.5f, 3.6f); //타겟 위치 초기화
    }


    // 강화학습 프로그램에게 관측정보를 전달
    // Behavior Parameters의 Space Size와 갯수를 맞춰야함.
    public override void CollectObservations(VectorSensor sensor)
    {
        //타겟과 에이전트의 포지션을 전달한다. 
        sensor.AddObservation(Target.localPosition); // x, y, z로 3개
        sensor.AddObservation(this.transform.localPosition); // x, y, z로 3개

        //현재 에이전트의 이동량을 전달한다.
        sensor.AddObservation(rBody.velocity.x); // 1개
        sensor.AddObservation(rBody.velocity.z); // 1개

        // 총 8개
    }


    // 정책(polocy)로 부터 결정된 행동에 따라 에이전트가 움직이도록 하는 함수
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // ContinuousActions에서 선형적인 값을 받아옴
        float forward = actionBuffers.ContinuousActions[0]; // 전진하는 값
        float rotate = actionBuffers.ContinuousActions[1]; // 좌우 회전 값

        for (int i = 0; i < wheels.Length; i++)
        {
            // 휠콜라이더 전체를 입력에 따라서 power만큼의 힘으로 움직이게한다.
            wheels[i].motorTorque = forward * power;
            if(forward * power > 0)
            {
                Debug.Log("forward");
                sp.WriteLine("w");
            } 
            else
            {
                Debug.Log("back");
                sp.WriteLine("s");
            }
      
        }
        for (int i = 0; i < 2; i++)
        {
            // 앞바퀴만 각도전환이 되어야하므로 for문을 앞바퀴만 해당되도록 설정한다.
            wheels[i].steerAngle = rotate * rot;

            if (rotate * rot > 0)
            {
                Debug.Log("forward");
                sp.WriteLine("d");
            }
            else
            {
                Debug.Log("back");
                sp.WriteLine("a");
            }
        }

        //0보다 크면 오른쪽
        //라는건 왼쪽이 움직이고 오른쪽 바퀴는 멈춰야댐
        //근데 이거 왜 안됨
        /*
        wheels[0].motorTorque = forward * power - (rotate * rot);
        wheels[1].motorTorque = forward * power + (rotate * rot);
        wheels[2].motorTorque = forward * power - (rotate * rot);
        wheels[3].motorTorque = forward * power + (rotate * rot);
        */


        // 타겟과의 거리 비례 보상 제공
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        // 타겟과의 거리가 1.4 미만이면 접촉했다고 판단
        if (distanceToTarget < 1.4f)
        {
            // 보상을 1 늘리고 에피소드를 재시작
            SetReward(1.0f);
            EndEpisode();
        }
    }

    // 이산적인 값으로 이동하게 만듦
    /*
    public void moveAgent(ActionSegment<int> act)
    {
        var forwardAxis = act[0];
        var rotateAxis = act[1];
        Debug.Log(rotateAxis);

        if (forwardAxis == 0)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                // for문을 통해서 휠콜라이더 전체를 Vertical 입력에 따라서 power만큼의 힘으로 움직이게한다.
                wheels[i].motorTorque = 1f * power;
            }
        }
       
        if (rotateAxis == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                // 앞바퀴만 각도전환이 되어야하므로 for문을 앞바퀴만 해당되도록 설정한다.
                wheels[i].steerAngle = -1f * rot;
            }
        } 
        else if (rotateAxis == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                // 앞바퀴만 각도전환이 되어야하므로 for문을 앞바퀴만 해당되도록 설정한다.
                wheels[i].steerAngle = 1f * rot;
            }
        }
    }
    */

    //만약 에이전트가 벽과 닿으면 에피소드를 재시작
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Wall"))
        {
            //겁나 신기함;;
            //SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[0] = Input.GetAxis("Vertical");
    }
}
