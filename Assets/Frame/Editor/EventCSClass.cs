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
public enum YourEvent
{
    Load = ManagerId.UIManager,
    Regist,
    Log,
    MaxVaule
}

public class EventCSClass : UIBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)(YourEvent.Load):
              
                break;
            case (ushort)(YourEvent.Log):
               
                break;
        }
    }

    void Awake()
    {
        //定义要注册的消息
        msgIds = new ushort[]{
            (ushort)YourEvent.Load,
            (ushort)YourEvent.Log
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }

    void Start()
    {
        
    }
}
