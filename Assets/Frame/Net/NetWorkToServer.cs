using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//保证消息的流畅接收和发送
//
public class NetWorkToServer {
    //消息的接收队列
    private Queue<NetMsgBase> recvMsgBool = null;
    //消息的发送队列
    private Queue<NetMsgBase> sendMsgBool = null;

    NetSocket clientSocket;

    public NetWorkToServer(string ip, ushort port)
    {
        recvMsgBool = new Queue<NetMsgBase>();
        sendMsgBool = new Queue<NetMsgBase>();
        clientSocket = new NetSocket();
        clientSocket.AsycConnect(ip, port, AsynConnCallBack, AsynRecvCallBack);
    }
    #region Send
    Thread sendThread;
    //放入发送队列中
    public void PutMsgSendedToPool(NetMsgBase msgBase)
    {
        lock (sendMsgBool)
        {
            sendMsgBool.Enqueue(msgBase);
        }
    }
    //新开一个线程进行发送
    void AsynConnCallBack(bool success, NetSocket.ErrorSocket error, string msgStr)
    {
        if (success)
        {
            sendThread = new Thread(LoopSendMsg);
            sendThread.Start();

        }
    }

    void SendCallBack(bool success, NetSocket.ErrorSocket error, string exception)
    {
        if (success)
        {

        }
        else
        {
            //处理错误信息
        }
    }
    //从消息队列中取出消息发送
    void LoopSendMsg()
    {
        while (clientSocket!=null && clientSocket.IsConnected())
        {
            lock (sendMsgBool)
            {
                while (sendMsgBool.Count > 0)
                {
                    NetMsgBase tmpBody = sendMsgBool.Dequeue();
                    clientSocket.AsynSend(tmpBody.GetBytes(), SendCallBack);
                }
            }
            Thread.Sleep(100);
        }
    }
    #endregion
    #region Recv
    void AsynRecvCallBack(bool success, NetSocket.ErrorSocket error, string exception, byte[] byteMsg, string strMsg){
        if (success)
        {

        }
        else
        {
            //处理错误信息
        }
    }

    void PutMsgRecvedToPool(byte[] recvMsg)
    {
        NetMsgBase tmpMsg = new NetMsgBase(recvMsg);
        recvMsgBool.Enqueue(tmpMsg);    
    }
    //每一帧调用
    public void Update()
    {
        if (recvMsgBool != null)
        {
            while (recvMsgBool.Count > 0)
            {
                NetMsgBase tmpMsg = recvMsgBool.Dequeue();
                AnalyseData(tmpMsg);
            }
        }
    }
    //交往消息处理中心进行处理
    void AnalyseData(NetMsgBase msg)
    {
        MsgCenter.Instance.SendToMsg(msg);
    }
    #endregion

    #region Disconnect

    void CallBackDisconnect(bool success, NetSocket.ErrorSocket error, string exception)
    {
        if (success)
        {
            sendThread.Abort();//关闭线程
        }
        else
        {
            //处理错误消息
        }
    }

    public void DisConnect()
    {
        if (clientSocket != null && clientSocket.IsConnected())
        {
            clientSocket.AsynDisConnect(CallBackDisconnect);
        }
    }

    #endregion
}
