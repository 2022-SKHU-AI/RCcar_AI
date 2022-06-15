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

        sp.PortName = "COM5";     // 여기에는 아두이노 포트 넣어주면 됩니다.

        sp.BaudRate = 115200;      // 아두이노 보레이트랑 맞춰주시면 됩니다.

        sp.DataBits = 8;

        sp.Parity = Parity.None;

        sp.StopBits = StopBits.One;



        sp.Open();    //포트를 엽니다. 열고나면 닫힐동안 시리얼 모니터를 사용하지 못합니다.(여기서 점유하고있으므로)

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

        sp.Close();    //꺼질때 소켓을 닫아줍니다.

    }

}