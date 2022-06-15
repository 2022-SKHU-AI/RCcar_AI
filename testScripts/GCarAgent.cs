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
    public WheelCollider[] wheels = new WheelCollider[4]; // ���� 4��
    private float power = 100f; // ������ ȸ����ų ��
    private float rot = 45f; // ������ ȸ�� ����
    public Transform Target; // Ÿ���� ��ġ ����

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        // ���� �߽��� y�� �Ʒ��������� �����.
        rBody.centerOfMass = new Vector3(0, -1, 0);

        sp.PortName = "COM5";     // ���⿡�� �Ƶ��̳� ��Ʈ �־��ָ� �˴ϴ�.
        sp.BaudRate = 115200;      // �Ƶ��̳� ������Ʈ�� �����ֽø� �˴ϴ�.
        sp.DataBits = 8;
        sp.Parity = Parity.None;
        sp.StopBits = StopBits.One;

        sp.Open();    //��Ʈ�� ���ϴ�. ������ �������� �ø��� ����͸� ������� ���մϴ�.(���⼭ �����ϰ������Ƿ�)
    }

    public override void OnEpisodeBegin()
    {
        //���ο� ���Ǽҵ尡 �����ϸ� ������Ʈ�� �������� �ʱ�ȭ
        this.rBody.angularVelocity = Vector3.zero; // ������Ʈ ���ӵ� �ʱ�ȭ
        this.rBody.velocity = Vector3.zero; // ������Ʈ �ӵ� �ʱ�ȭ 2
        this.transform.localPosition = new Vector3(3,0,-3.5f); // ������Ʈ ��ġ �ʱ�ȭ
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);

        //Target.localPosition = new Vector3(3, 0.5f, 3.6f); //Ÿ�� ��ġ �ʱ�ȭ
    }


    // ��ȭ�н� ���α׷����� ���������� ����
    // Behavior Parameters�� Space Size�� ������ �������.
    public override void CollectObservations(VectorSensor sensor)
    {
        //Ÿ�ٰ� ������Ʈ�� �������� �����Ѵ�. 
        sensor.AddObservation(Target.localPosition); // x, y, z�� 3��
        sensor.AddObservation(this.transform.localPosition); // x, y, z�� 3��

        //���� ������Ʈ�� �̵����� �����Ѵ�.
        sensor.AddObservation(rBody.velocity.x); // 1��
        sensor.AddObservation(rBody.velocity.z); // 1��

        // �� 8��
    }


    // ��å(polocy)�� ���� ������ �ൿ�� ���� ������Ʈ�� �����̵��� �ϴ� �Լ�
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // ContinuousActions���� �������� ���� �޾ƿ�
        float forward = actionBuffers.ContinuousActions[0]; // �����ϴ� ��
        float rotate = actionBuffers.ContinuousActions[1]; // �¿� ȸ�� ��

        for (int i = 0; i < wheels.Length; i++)
        {
            // ���ݶ��̴� ��ü�� �Է¿� ���� power��ŭ�� ������ �����̰��Ѵ�.
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
            // �չ����� ������ȯ�� �Ǿ���ϹǷ� for���� �չ����� �ش�ǵ��� �����Ѵ�.
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

        //0���� ũ�� ������
        //��°� ������ �����̰� ������ ������ ����ߴ�
        //�ٵ� �̰� �� �ȵ�
        /*
        wheels[0].motorTorque = forward * power - (rotate * rot);
        wheels[1].motorTorque = forward * power + (rotate * rot);
        wheels[2].motorTorque = forward * power - (rotate * rot);
        wheels[3].motorTorque = forward * power + (rotate * rot);
        */


        // Ÿ�ٰ��� �Ÿ� ��� ���� ����
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        // Ÿ�ٰ��� �Ÿ��� 1.4 �̸��̸� �����ߴٰ� �Ǵ�
        if (distanceToTarget < 1.4f)
        {
            // ������ 1 �ø��� ���Ǽҵ带 �����
            SetReward(1.0f);
            EndEpisode();
        }
    }

    // �̻����� ������ �̵��ϰ� ����
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
                // for���� ���ؼ� ���ݶ��̴� ��ü�� Vertical �Է¿� ���� power��ŭ�� ������ �����̰��Ѵ�.
                wheels[i].motorTorque = 1f * power;
            }
        }
       
        if (rotateAxis == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                // �չ����� ������ȯ�� �Ǿ���ϹǷ� for���� �չ����� �ش�ǵ��� �����Ѵ�.
                wheels[i].steerAngle = -1f * rot;
            }
        } 
        else if (rotateAxis == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                // �չ����� ������ȯ�� �Ǿ���ϹǷ� for���� �չ����� �ش�ǵ��� �����Ѵ�.
                wheels[i].steerAngle = 1f * rot;
            }
        }
    }
    */

    //���� ������Ʈ�� ���� ������ ���Ǽҵ带 �����
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Wall"))
        {
            //�̳� �ű���;;
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
