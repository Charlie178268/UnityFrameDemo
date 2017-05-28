using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : ManagerBase
{
    public static CharacterManager Instance = null;
    void Awake()
    {
        Instance = this;
    }

    public void SendMsg(MsgBase msgbase)
    {
        //如果是本类消息类型，直接通过MsgBase发送消息
        if (msgbase.GetManagerId() == ManagerId.CharacterManager)
        {
            ProgressEvent(msgbase);
        }
        else
        {
            MsgCenter.Instance.SendToMsg(msgbase);
        }
    }
}
