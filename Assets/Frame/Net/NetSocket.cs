using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;
using System.Threading;

public class NetSocket{

    public delegate void CallBackNormal(bool success, ErrorSocket error, string exception);

    //public delegate void CallBackSend(bool success, ErrorSocket error, string exception);

    public delegate void CallBackRecv(bool success, ErrorSocket error, string exception, byte[] byteMsg, string strMsg);

    //public delegate void CallBackDisConnect(bool success, ErrorSocket error, string exception);

    private CallBackNormal callBackConnect;

    private CallBackNormal callBackSend;

    private CallBackNormal callBackDisconnect;

    private CallBackRecv callBackRecv;

    //枚举Socket常见的状态
    public enum ErrorSocket {
        Success = 0,
        TimeOut,
        SocketNull,

        SocketUnConnect,
        ConnectSuccess,
        ConnectUnSuccessUnkonw,
        ConnectError,

        SendSuccess,
        SendUnSuccessUnKnow,

        RecvUnSuccessUnKnow,

        DisConnectSuccess,
        DisConnectUnKown
    }

    private ErrorSocket errorSocket;
    private Socket clientSocket;
    private string addressIp;
    private ushort port;

    SocketBuffer recvBuffer;//对接收到的消息进行沾包、拆包处理

    public NetSocket() {
        recvBuffer = new SocketBuffer(6, RecvMsgOver);
        buffer = new byte[1024];
    }



    #region Connect

    public bool IsConnected()
    {
        if (clientSocket != null && clientSocket.Connected)
        {
            return true;
        }
        return false;
    }

    public void AsycConnect(string ip, ushort port, CallBackNormal BackConn, CallBackRecv BackRecv)
    {
        errorSocket = ErrorSocket.Success;
        this.callBackConnect = BackConn;
        this.callBackRecv = BackRecv;

        //Socket已经连接
        if (clientSocket!=null && clientSocket.Connected)
        {
            this.callBackConnect(false, ErrorSocket.ConnectError, "Connect repeat");
        }else if (clientSocket == null || !clientSocket.Connected)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipadd = IPAddress.Parse(ip);
            IPEndPoint ipEnd = new IPEndPoint(ipadd, port);
            IAsyncResult connect = clientSocket.BeginConnect(ipEnd, connCallBack, clientSocket);
            //检测是否连接超时
            if (CheckTimeOut(connect))
            {
                callBackConnect(false, errorSocket, "Connect Time out");
            }
        }

       
    }

    void connCallBack(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndConnect(ar);
            if (clientSocket.Connected == false)
            {
                errorSocket = ErrorSocket.ConnectUnSuccessUnkonw;
                callBackConnect(false, errorSocket, "Connect Time out");
                return;
            }
            else
            {
                errorSocket = ErrorSocket.ConnectSuccess;
                callBackConnect(true, errorSocket, "Connect Success");
                //接收消息
            }
        }
        catch(Exception ee)
        {
            callBackConnect(false, errorSocket, ee.ToString());
        }
    }


    #endregion

    #region Recv

    byte[] buffer;

    public void Recieve()
    {
        if (clientSocket !=null && clientSocket.Connected)
        {
            IAsyncResult recvAsyn = clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, RecieveCallBack, clientSocket);
            if (CheckTimeOut(recvAsyn))
            {
                errorSocket = ErrorSocket.RecvUnSuccessUnKnow;
                callBackRecv(false, errorSocket, "Recieve Time out", null, "");
            }
        }
    }

    void RecieveCallBack(IAsyncResult ar)
    {
        try
        {
            //在接收消息时断开连接
            if (!clientSocket.Connected)
            {
                errorSocket = ErrorSocket.RecvUnSuccessUnKnow;
                callBackRecv(false, errorSocket, "Connect break", null, "");
                return;
            }
            int len = clientSocket.EndReceive(ar);
            if (len == 0)
            {
                return;
            }
            recvBuffer.RecvByte(buffer, len);//处理沾包问题。
        }
        catch(Exception ee)
        {
            errorSocket = ErrorSocket.RecvUnSuccessUnKnow;
            callBackRecv(false, errorSocket, ee.ToString(), null, "");
        }
    }

    #region RecvMsgOver

    public void RecvMsgOver(byte[] allByte)
    {
        errorSocket = ErrorSocket.Success;
        callBackRecv(true, errorSocket, "", null, "recv success");
    }

    #endregion

    #endregion

    #region SendMsg
    //发送请求
    public void AsynSend(byte[] sendBuffer, CallBackNormal tmpSendBack)
    {
        errorSocket = ErrorSocket.Success;
        this.callBackSend = tmpSendBack;
        if (clientSocket == null)
        {
            errorSocket = ErrorSocket.SocketNull;
            this.callBackSend(false, errorSocket, "");
        }else if (!clientSocket.Connected)
        {
            errorSocket = ErrorSocket.SocketNull;
            callBackSend(false, errorSocket, "");
        }
        else
        {
           IAsyncResult asySend = clientSocket.BeginSend(sendBuffer,0, sendBuffer.Length, SocketFlags.None, SendCallBack, clientSocket);
            if (CheckTimeOut(asySend))
            {
                errorSocket = ErrorSocket.SendUnSuccessUnKnow;
                callBackSend(false, errorSocket, "Send Time out");
                return;
            }
        }
    }

    void SendCallBack(IAsyncResult ar)
    {
        try
        {
            //在发送消息时断开连接
            if (!clientSocket.Connected)
            {
                errorSocket = ErrorSocket.RecvUnSuccessUnKnow;
                callBackRecv(false, errorSocket, "Connect break", null, "");
                return;
            }
            int len = clientSocket.EndSend(ar);
            if (len > 0)
            {
                errorSocket = ErrorSocket.SendSuccess;
                callBackRecv(true, errorSocket, "Send Success", null, "");
            }
            else {
                errorSocket = ErrorSocket.SendUnSuccessUnKnow;
                callBackRecv(false, errorSocket, "Send Error", null, "");
            }
        }
        catch (Exception ee)
        {
            errorSocket = ErrorSocket.SendUnSuccessUnKnow;
            callBackRecv(false, errorSocket, ee.ToString(), null, "");
        }
    }

    #endregion

    #region TimeOut Check
    //设置2秒超时
    bool CheckTimeOut(IAsyncResult ar) {
        int i = 0;
        while (!ar.IsCompleted == true)
        {
            i++;
            if (i > 20)
            {
                errorSocket = ErrorSocket.TimeOut;
                return true;
            }
            Thread.Sleep(100);
        }
        return false;
    }
    #endregion

    #region DisConnect

    void DisConnCallBack(IAsyncResult ar)
    {
        try
        {
            clientSocket.EndDisconnect(ar);
            clientSocket.Close();
            clientSocket = null;
            errorSocket = ErrorSocket.DisConnectSuccess;
            this.callBackDisconnect(true, errorSocket, "Socket disconnect success");
        }
        catch (Exception ee)
        {
            errorSocket = ErrorSocket.DisConnectUnKown;
            this.callBackDisconnect(false, errorSocket, ee.ToString());
        }
}

    public void AsynDisConnect(CallBackNormal tmpDisConnBack)
    {
        try
        {
            errorSocket = ErrorSocket.Success;
            this.callBackDisconnect = tmpDisConnBack;
            if (clientSocket == null)
            {
                errorSocket = ErrorSocket.DisConnectUnKown;
                this.callBackDisconnect(false, errorSocket, "Socket is null");
            }
            else if (!clientSocket.Connected)
            {
                errorSocket = ErrorSocket.DisConnectUnKown;
                this.callBackDisconnect(false, errorSocket, "Socket has been disconnected!");
            }
            else
            {
                IAsyncResult asyDisconn = clientSocket.BeginDisconnect(false, DisConnCallBack, clientSocket);
                if (CheckTimeOut(asyDisconn))
                {
                    errorSocket = ErrorSocket.DisConnectUnKown;
                    this.callBackDisconnect(false, errorSocket, "Socket disconnect is time out");
                }
            }
        }
        catch(Exception ee)
        {
            errorSocket = ErrorSocket.DisConnectUnKown;
            this.callBackDisconnect(false, errorSocket, ee.ToString());
        }
    }

    #endregion
}
