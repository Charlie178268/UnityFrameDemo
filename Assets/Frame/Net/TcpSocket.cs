using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TcpEvent
{
    TcpConnect = ManagerId.NetManager+1,
    TcpSendMsg,
    MaxValue
}

public class TcpConnectMsg : MsgBase
{
    public string ip;
    public ushort port;
    public TcpConnectMsg(ushort tmpId, string ip, ushort port)
    {
        this.msgId = tmpId;
        this.ip = ip;
        this.port = port;
    }
}

public class TcpSendMsg : MsgBase
{
    public NetMsgBase netMsg;
    public TcpSendMsg(ushort tmpId, NetMsgBase netMsg)
    {
        this.msgId = tmpId;
        this.netMsg = netMsg;
    }
}

public class TcpSocket : NetBase {

    NetWorkToServer socket;

    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)TcpEvent.TcpConnect:
                {
                    Debug.Log("连接中");
                    TcpConnectMsg tcpMsg = msgbase as TcpConnectMsg;
                    //放入发送消息队列
                    socket = new NetWorkToServer(tcpMsg.ip, tcpMsg.port);
                }
                break;
            case (ushort)TcpEvent.TcpSendMsg:
                {
                    Debug.Log("发送消息中");
                    TcpSendMsg sendMsg = msgbase as TcpSendMsg;
                    socket.PutMsgSendedToPool(sendMsg.netMsg);

                }
                break;
        }
    }

    void Awake()
    {
        msgIds = new ushort[]
        {
            (ushort)TcpEvent.TcpConnect,
            (ushort)TcpEvent.TcpSendMsg
        };
        RegisSelf(this, msgIds);
    }

    void Start () {
		
	}
	
	void Update () {
		if (socket != null)
        {
            //每一帧接收消息
            socket.Update();
        }
	}
}
