using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Debuger {
    public static bool EnableLog = true;//允许打印的开关
    private static UDPSocket udpSocket = null;
    public static UDPSocket UDPSocket
    {
        get
        {
            if (udpSocket == null)
            {
                udpSocket = new UDPSocket();
                udpSocket.BindSocket(18001, 1024, null);
            }
            return udpSocket;
        }
    }
    public static void Log(object message, Object context)
    {
        if (EnableLog)
        {
            //如果在编辑器上直接打印
            if (Application.platform==RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.Log(message, context);
            }
            else//如果不在编辑器上，把打印信息发送到编辑器所在的电脑上
            {
                //string转换为byte[]
                byte[] data = Encoding.Default.GetBytes(message.ToString());
                udpSocket.SendData("192.168.1.117", 18001, data); 
            }
        }
    }
    public static void LogError(object message, Object context)
    {
        if (EnableLog)
        {
            //如果在编辑器上直接打印
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                Debug.LogError(message, context);
            }
            else//如果不在编辑器上，把打印信息发送到编辑器所在的电脑上
            {
                //string转换为byte[]
                byte[] data = Encoding.Default.GetBytes(message.ToString());
                udpSocket.SendData("255.255.255.255", 18001, data);//广播到全部局域网
            }
        }
    }
}
