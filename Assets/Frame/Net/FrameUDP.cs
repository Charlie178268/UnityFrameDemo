using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
/// <summary>
/// 框架开发模板
/// </summary>

// 自定义消息类型
public enum UDPEvent
{
    Initial = TcpEvent.MaxValue+1,
    SendTo,
    MaxVaule
}

public class UdpIniMsg : MsgBase
{
    public ushort port;
    public int recvBufferLength;
    public UDPSocket.UDPSocketDelegate recvDelegate;
    public UdpIniMsg(ushort msgId, ushort port, int recvBuffLen, UDPSocket.UDPSocketDelegate recvDelegate)
    {
        this.msgId = msgId;
        this.port = port;
        this.recvBufferLength = recvBuffLen;
        this.recvDelegate = recvDelegate;
    }
}

public class UdpSendMsg : MsgBase
{
    public string ip;
    public ushort port;
    public byte[] sendBuff;
    public UdpSendMsg(ushort msgId, ushort port, byte[] sendBuffer)
    {
        this.msgId = msgId;
        this.port = port;
        this.sendBuff = sendBuffer;
    }
}

public class FrameUDP : UIBase
{
    UDPSocket udpSocket;
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)(UDPEvent.Initial):
                {
                    UdpIniMsg tmpMsg = msgbase as UdpIniMsg;
                    udpSocket = new UDPSocket();
                    udpSocket.BindSocket(tmpMsg.port, tmpMsg.recvBufferLength, tmpMsg.recvDelegate);
                    break;
                }
            case (ushort)(UDPEvent.SendTo):
                {
                    UdpSendMsg tmpMsg = msgbase as UdpSendMsg;
                    udpSocket.SendData(tmpMsg.ip, tmpMsg.port, tmpMsg.sendBuff);
                    break;
                }
        }
    }

    void Awake()
    {
        //定义要注册的消息
        msgIds = new ushort[]{
            (ushort)UDPEvent.Initial,
            (ushort)UDPEvent.SendTo
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }

    void Start()
    {
        
    }
}
