using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public delegate void CallBackRecvOver(byte[] allData);
/// <summary>
/// 这个类处理tcp传输过程中因为以流来传输，有可能把多个包合在一起一次传输，也有可能一次传输不足一个包，
/// 因此出现的需要我们拆分数据的问题，叫做沾包和拆包。
/// </summary>
public class SocketBuffer {
    
    private byte[] headByte;//存储消息头

    private byte headLength = 6;//消息头长度默认是6

    private byte[] allRecvData;//存储接收到的数据

    private int curRecvLength;//当前接收到的数据长度

    private int allDataLength;//总共接收的数据长度

    //传入消息头的长度和消息接收完的回调函数
    public SocketBuffer(byte tmpHeadLength, CallBackRecvOver tmpCallBack) {
        this.headLength = tmpHeadLength;
        headByte = new byte[headLength];
        callBackRecvOver = tmpCallBack;
        curRecvLength = 0;
    }
    //接收数据的处理，接收到的所有数据和数据长度
    public void RecvByte(byte[] recvData, int realLength) {
        if (realLength <= 0) {
            return;
        }
        //若接收到的消息小于头部的长度，则进行处理
        if (curRecvLength < realLength)
        {
            RecvHead(recvData, realLength);
        }
        else {
            //接收的总字节数=头部+消息内容
            int tmpLength = curRecvLength + realLength;

            if (tmpLength == allDataLength)
            {
                RecvOneAll(recvData, realLength);
            }
            //接收的数据比这个消息长
            else if (tmpLength > allDataLength)
            {
                RecvLarger(recvData, realLength);
            }
            else {
                RecvSmall(recvData, realLength);
            }
        }
    }

    private void RecvLarger(byte[] recvByte, int realLength) {
        int tmpLength = allDataLength - curRecvLength;
        //先取多的消息
        Buffer.BlockCopy(recvByte, 0, allRecvData, curRecvLength, tmpLength);
        curRecvLength += tmpLength;
        RecvOneMsgOver();
        //另一条消息,重新处理
        int remainLength = realLength - tmpLength;
        byte[] remainByte = new byte[remainLength];
        Buffer.BlockCopy(recvByte, tmpLength, remainByte, 0, remainLength);
        RecvByte(remainByte, remainLength);
    }


    private void RecvSmall(byte[] recvByte, int realLength)
    {
        Buffer.BlockCopy(recvByte, 0, allRecvData, curRecvLength, realLength);
        curRecvLength += realLength;
    }
    //接收完一条消息
    private void RecvOneAll(byte[] recvByte, int realLength) {
        Buffer.BlockCopy(recvByte, 0, allRecvData, curRecvLength, realLength);
        curRecvLength += realLength;
        RecvOneMsgOver();
    }

    public void RecvHead(byte[] recByte, int realLength) {
        int tmpReal = headByte.Length - curRecvLength;

        int tmpLengh = curRecvLength + realLength;

        if (tmpLengh < headByte.Length)//接收到的数据小于头部的长度
        {

            Buffer.BlockCopy(recByte, 0, headByte, curRecvLength, realLength);
            curRecvLength += realLength;
        }
        else//接收到的数据大于等于头
        {
            Buffer.BlockCopy(recByte, 0, headByte, curRecvLength, tmpReal);
            curRecvLength += tmpReal;
            //头部已凑齐
            //取四个字节，转换为Int
            allDataLength = BitConverter.ToInt32(headByte, 0) + headLength;
            allRecvData = new byte[allDataLength];
            Buffer.BlockCopy(headByte, 0, allRecvData, 0, headLength);
            int tmpRemin = realLength - tmpReal;
            //recByte还有数据
            if (tmpRemin > 0)
            {
                byte[] tmpByte = new byte[tmpRemin];
                Buffer.BlockCopy(recByte, tmpReal, tmpByte, 0, tmpRemin);
                RecvByte(tmpByte, tmpRemin);
            }
            else {
                //只有消息头
                RecvOneMsgOver();
            }
        }
    }
    #region recv over back to


    CallBackRecvOver callBackRecvOver;
    
    //接收完一条消息后,回调给上层
    private void RecvOneMsgOver() {
        if (callBackRecvOver != null) {
            callBackRecvOver(allRecvData);
        }
        curRecvLength = 0;
        allDataLength = 0;
        allRecvData = null;
    }
    #endregion
}
