using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;

//发送给动画的消息
public class ValueMsg : MsgBase
{
    public float value;
    public ValueMsg(ushort msgId, float v)
    {
        this.msgId = msgId;
        this.value = v;
    }
}

//挂上物体时，自动增加组件
[RequireComponent(typeof(CharacterController))]
public class MonsterPlayer : CharacterBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)CharacterEvent.joyStickBegin:
                {
    
                    break;
                }
            case (ushort)CharacterEvent.joyStick:
                {
                    Debug.Log("start rotate");
                    JoyStickMsg tmpMsg = msgbase as JoyStickMsg;
                    MoveCtr(tmpMsg.vec);
                    valueMsg.value = tmpSpeed;
                    valueMsg.ChangeMsgId((ushort)CharacterEvent.Run);
                    SendMsg(valueMsg);
                    break;
                }
            case (ushort)CharacterEvent.joyStickEnd:
                {
                    Debug.Log("end");
                    valueMsg.ChangeMsgId((ushort)CharacterEvent.Idle);
                    SendMsg(valueMsg);
                    break;
                }
            case (ushort)CharacterEvent.Initial:
              
                break;
            case (ushort)CharacterEvent.Attack:

                break;
            case (ushort)CharacterEvent.Die:

                break;
            case (ushort)CharacterEvent.Idle:

                break;
            case (ushort)CharacterEvent.AttackBig:

                break;
            case (ushort)CharacterEvent.looseBlood:
                Debug.Log("loosenBo");
                MsgFloat tmpBlood = msgbase as MsgFloat;//传来受攻击的伤害量
                bloodData.LooseBlood(tmpBlood.v);
                MsgFloat msgBlood = new MsgFloat((ushort)BloodEvent.Loose, bloodData.GetBlood());
                SendMsg(msgBlood);
                break;
        }
    }

    void Awake()
    {
        //定义要注册的消息
        msgIds = new ushort[]{
            (ushort)CharacterEvent.Attack,
            (ushort)CharacterEvent.AttackBig,
            (ushort)CharacterEvent.Die,
            (ushort)CharacterEvent.Idle,
            (ushort)CharacterEvent.Initial,
            (ushort)CharacterEvent.Run,
            (ushort)CharacterEvent.joyStick,
            (ushort)CharacterEvent.joyStickBegin,
            (ushort)CharacterEvent.joyStickEnd,
            (ushort)CharacterEvent.looseBlood
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }

    Vector3 moveDir = Vector3.zero;
    Vector3 moveSpeed = Vector3.zero;
    float tmpSpeed = 0f;
    void MoveCtr(Vector2 vec)
    {
        float speedx = Mathf.Abs(vec.x);
        float speedy = Mathf.Abs(vec.y);
        tmpSpeed = Mathf.Sqrt(speedx*speedx+speedy*speedy);//求出半径

        moveSpeed.x = vec.x;
        moveSpeed.y = 0;
        moveSpeed.z = vec.y;

        moveControl.SimpleMove(moveSpeed*tmpSpeed*3);//简单移动，自动适应重力

        //旋转
        float angle = Mathf.Atan2(vec.x, vec.y);//反切函数得出弧度
        angle = angle * Mathf.Rad2Deg;//弧度到角度的转化
        moveDir.y = angle;
        transform.rotation = Quaternion.Euler(moveDir);
        
    }
    ValueMsg valueMsg;
    private CharacterController moveControl;//人物控制器
    private CharacterBlood bloodData;//人物血量
    void Start()
    {
        moveControl = transform.GetComponent<CharacterController>();
        valueMsg = new ValueMsg((ushort)CharacterEvent.Run, 0f);
        bloodData = new CharacterBlood();
    }
}
