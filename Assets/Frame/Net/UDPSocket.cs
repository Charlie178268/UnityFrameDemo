using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
//udp用来进程间通信(两exe之间)
//用来真机调试，在移动设备上将打印的信息传到电脑编辑器显示
public class UDPSocket : MonoBehaviour {

    public delegate void UDPSocketDelegate(byte[] pBuf, int dwCount, string tmpIp, ushort tmpPort);

    IPEndPoint udpIp;

    Socket udpSocket;

    UDPSocketDelegate udpDelegate;

    byte[] recvData;

    Thread recvThread;

    public bool BindSocket(ushort port, int bufferLength, UDPSocketDelegate tmpDelegate)
    {
        udpIp = new IPEndPoint(IPAddress.Any, port);
        UDPConnect();
        udpDelegate = tmpDelegate;
        recvData = new byte[bufferLength];
        recvThread = new Thread(UDPConnect);
        recvThread.Start();
        return true;
    }

    public void UDPConnect()
    {
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpSocket.Bind(udpIp);//提供服务需要绑定端口
    }

    #region 专门接收消息的线程
    bool isRunning = true;//退出时置为false
    public void RecvDataThread()
    {
        while (isRunning)
        {
            //没有数据可接收即睡眠
            if (udpSocket==null || udpSocket.Available < 1)
            {
                Thread.Sleep(100);
                continue;
            }
            lock (this)
            {
                //初始化EndPoint以接收对方的ip地址和端口号
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)sender;

                int recvLen = udpSocket.ReceiveFrom(recvData, ref remote);
                if (udpDelegate != null)
                {
                    udpDelegate(recvData, recvLen, remote.AddressFamily.ToString(), (ushort)sender.Port);
                }
            }
        }
    }
    #endregion

    public int SendData(string ip, ushort uport, byte[] data)
    {
        IPEndPoint sendToIp = new IPEndPoint(IPAddress.Parse(ip), uport);
        //如果断开后重连udp
        if (!udpSocket.Connected)
        {
            UDPConnect();
        }
        int sendLen = udpSocket.SendTo(data, data.Length, SocketFlags.None, sendToIp);
        return sendLen;
    }
}
