using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;

public class JoyStickMsg : MsgBase
{
    public Vector3 vec;
    public JoyStickMsg(ushort msgId, Vector3 tmpVec)
    {
        this.msgId = msgId;
        this.vec = tmpVec;
    }
}

public class JoyStickCtl : CharacterBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)(CharacterEvent.joyStick):
               
                break;
            case (ushort)(CharacterEvent.Jump):
               
                break;
        }
    }

    void OnJoyStickMoveBegin(MovingJoystick move)
    {
        joystickMsg.ChangeMsgId((ushort)CharacterEvent.joyStickBegin);
        SendMsg(joystickMsg);
    }

    void OnJoyStickMove(MovingJoystick move)
    {
        joystickMsg.vec = move.joystickAxis;
        joystickMsg.ChangeMsgId((ushort)CharacterEvent.joyStick);
        SendMsg(joystickMsg);
    }

    void OnJoyStickMoveEnd(MovingJoystick move)
    {
        joystickMsg.ChangeMsgId((ushort)CharacterEvent.joyStickEnd);
        SendMsg(joystickMsg);
    }

    void OnButtonDown(string btnName)
    {
        buttonMsg.ChangeMsgId((ushort)CharacterEvent.Attack);
        SendMsg(buttonMsg);
    }

    void OnButtonUp(string btnName)
    {
        buttonMsg.ChangeMsgId((ushort)CharacterEvent.Idle);
        SendMsg(buttonMsg);
    }


    JoyStickMsg joystickMsg;//因为此消息要重复利用，因此设为全局变量
    MsgBase buttonMsg;
    void Awake()
    {
        //定义要注册的消息
        msgIds = new ushort[]{
            (ushort)CharacterEvent.joyStick,
            (ushort)CharacterEvent.Jump
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }

    void Start()
    {
        //滑杆
        EasyJoystick.On_JoystickMoveStart += OnJoyStickMoveBegin;
        EasyJoystick.On_JoystickMove += OnJoyStickMove;
        EasyJoystick.On_JoystickMoveEnd += OnJoyStickMoveEnd;

        //按钮
        EasyButton.On_ButtonDown += OnButtonDown;
        EasyButton.On_ButtonUp += OnButtonUp;

        joystickMsg = new JoyStickMsg((ushort)CharacterEvent.joyStick, Vector3.zero);
        buttonMsg = new MsgBase((ushort)CharacterEvent.Attack);
    }
}
