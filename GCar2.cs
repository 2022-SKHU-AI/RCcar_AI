using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

using System.IO.Ports;
using System.Threading;


public class GCar2 : Agent
{
    SerialPort sp = new SerialPort();
    Rigidbody rBody;
    private float power = 100f; // 바퀴를 회전시킬 힘
    private float rot = 45f; // 바퀴의 회전 각도
    public Transform Target; // 타겟의 위치 정보

    private bool spBool = false;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        // 무게 중심을 y축 아래방향으로 낮춘다.
        //rBody.centerOfMass = new Vector3(0, -1, 0);

        try
        {

            sp.PortName = "COM5";     // 여기에는 아두이노 포트 넣어주면 됩니다.
            sp.BaudRate = 115200;      // 아두이노 보레이트랑 맞춰주시면 됩니다.
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            spBool = true;

            sp.Open();    //포트를 엽니다. 열고나면 닫힐동안 시리얼 모니터를 사용하지 못합니다.(여기서 점유하고있으므로)
        }

        catch (Exception ex)
        {
            spBool = false;
        }
    }

    public override void OnEpisodeBegin()
    {
        //새로운 에피소드가 시작하면 에이전트의 포지션의 초기화
        this.rBody.angularVelocity = Vector3.zero; // 에이전트 각속도 초기화
        this.rBody.velocity = Vector3.zero; // 에이전트 속도 초기화 2
        this.transform.localPosition = new Vector3(3, 0, -3.5f); // 에이전트 위치 초기화
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

    public float forceMultiplier = 1;
    float m_LateralSpeed = 1.0f;
    float m_ForwardSpeed = 1.0f;

    // 정책(polocy)로 부터 결정된 행동에 따라 에이전트가 움직이도록 하는 함수
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // ContinuousActions에서 선형적인 값을 받아옴
        moveAgent(actionBuffers.DiscreteActions);       


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
    public void moveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = act[0];
        //var rotateAxis = act[1];
        Debug.Log("forwardAxis" + forwardAxis);

        Vector3 dir = this.transform.localRotation * Vector3.forward;
        Debug.Log(dir);

        switch (forwardAxis)
        {

            case 0:
                //dirToGo = transform.forward * -m_ForwardSpeed;
                this.transform.Translate(dir * 1f * Time.deltaTime);
                if(spBool)
                    sp.WriteLine("s");
                break;
            case 1:
                //dirToGo = transform.forward * m_ForwardSpeed;
                this.transform.Translate(dir * 1f * Time.deltaTime);
                if (spBool)
                    sp.WriteLine("w");
                break;

            case 2:
                rotateDir = transform.up * -1f;
                transform.Rotate(rotateDir, Time.deltaTime * 50f);
                if (spBool)
                    sp.WriteLine("a");
                break;
            case 3:
                rotateDir = transform.up * 1f;
                transform.Rotate(rotateDir, Time.deltaTime * 50f);
                if (spBool)
                    sp.WriteLine("d");
                break;

        }
         
    }

    //만약 에이전트가 벽과 닿으면 에피소드를 재시작
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Wall"))
        {
            //겁나 신기함;;
            //SetReward(-0.1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }
}
