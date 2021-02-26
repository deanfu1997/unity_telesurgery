
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    private UdpConnection connection;

    void Start()
    {
        string sendIp = "127.0.0.1";
        int sendPort = 12345;
        // int receivePort = 11000;

        connection = new UdpConnection();
        connection.StartConnection(sendIp, sendPort);
    }

    void Update()
    {
        // foreach (var message in connection.getMessages()) Debug.Log(message);

        connection.Send("Hi!");
    }

    void OnDestroy()
    {
        connection.Stop();
    }



}