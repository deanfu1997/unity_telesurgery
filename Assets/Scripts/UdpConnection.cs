using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpConnection
{
    private UdpClient udpClient;

    private readonly Queue<string> incomingQueue = new Queue<string>();
    private string senderIp;
    private int senderPort;

    public void StartConnection(string sendIp, int sendPort)
    {
        this.senderIp = sendIp;
        this.senderPort = sendPort;

        Debug.Log("Set sendee at ip " + sendIp + " and port " + sendPort);

    }


    public void Send(string message)
    {
        Debug.Log(String.Format("Send msg to ip:{0} port:{1} msg:{2}", senderIp, senderPort, message));
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(senderIp), senderPort);
        Byte[] sendBytes = Encoding.UTF8.GetBytes(message);
        udpClient.Send(sendBytes, sendBytes.Length, serverEndpoint);
    }

    public void Stop()
    {
        udpClient.Close();
    }
}
