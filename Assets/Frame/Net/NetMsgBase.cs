using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NetMsgBase : MsgBase {
    public byte[] buffer;

    public NetMsgBase(byte[] arr)
    {
        buffer = arr;
        //把buffer的从第5个字节起(前4个是消息头)转换为无符号整形，即把底层的byte转换为上层识别的msgid
        this.msgId = BitConverter.ToUInt16(arr, 4);
    }

    public byte[] GetBytes()
    {
        return buffer;
    }
}
