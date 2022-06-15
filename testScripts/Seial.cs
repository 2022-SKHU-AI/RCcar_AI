using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using System.IO.Ports;

using System.Threading;

using System;



public class Seial : MonoBehaviour

{

    SerialPort sp = new SerialPort();

    void Start()

    {

        sp.PortName = "COM5";     // ���⿡�� �Ƶ��̳� ��Ʈ �־��ָ� �˴ϴ�.

        sp.BaudRate = 115200;      // �Ƶ��̳� ������Ʈ�� �����ֽø� �˴ϴ�.

        sp.DataBits = 8;

        sp.Parity = Parity.None;

        sp.StopBits = StopBits.One;



        sp.Open();    //��Ʈ�� ���ϴ�. ������ �������� �ø��� ����͸� ������� ���մϴ�.(���⼭ �����ϰ������Ƿ�)

    }



    // Update is called once per frame

    void Update()

    {

        switch (Input.inputString)

        {

            case "W":

            case "w":

                Debug.Log("press w");

                sp.WriteLine("w");

                break;



            case "A":

            case "a":

                Debug.Log("press a");

                sp.WriteLine("a");

                break;



            case "S":

            case "s":

                Debug.Log("press s");

                sp.WriteLine("s");

                break;



            case "D":

            case "d":

                Debug.Log("press d");

                sp.WriteLine("d");

                break;

        }

    }



    private void OnApplicationQuit()

    {

        sp.Close();    //������ ������ �ݾ��ݴϴ�.

    }

}