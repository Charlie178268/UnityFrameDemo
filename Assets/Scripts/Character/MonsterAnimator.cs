using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;

//当此脚本没挂在一个物体上，就在这个物体上添加一个Animator组件
[RequireComponent(typeof(Animator))]
public class MonsterAnimator : CharacterBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)CharacterEvent.Initial:
               
                break;
            case (ushort)CharacterEvent.walk:
                animator.SetInteger("idleOrWalkOrRun", 3);
                break;
            case (ushort)CharacterEvent.Jump:
                animator.SetBool("isJump", true);
                break;
            case (ushort)CharacterEvent.Attack:
                animator.SetInteger("idleOrWalkOrRun", 4);
                
                break;
            case (ushort)CharacterEvent.Die:
                Debug.Log("die");
                animator.SetBool("isDie", true);
                break;
            case (ushort)CharacterEvent.Idle:
                Debug.Log("idle");
                animator.SetInteger("idleOrWalkOrRun", 1);
                break;
            case (ushort)CharacterEvent.AttackBig:
                Debug.Log("大招");
                animator.SetInteger("idleOrWalkOrRun", 5);
                break;
            case (ushort)CharacterEvent.Run:
                Debug.Log("RUN");
                ValueMsg tmpMsg = msgbase as ValueMsg;
                
                animator.SetInteger("idleOrWalkOrRun", 3);
                //设置动画播放速度
                AnimatorStateInfo animatorInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (animatorInfo.IsName("Run"))//注意这里指的不是动画的名字而是动画状态的名字
                {
                    Debug.Log("设置速度中");
                    animator.speed = tmpMsg.value;

                }
                break;
        }
    }

    public Animator animator;

    void Awake()
    {
        //定义要注册的消息
        msgIds = new ushort[]{
            (ushort)CharacterEvent.Attack,
            (ushort)CharacterEvent.walk,
            (ushort)CharacterEvent.Jump,
            (ushort)CharacterEvent.AttackBig,
            (ushort)CharacterEvent.Die,
            (ushort)CharacterEvent.Idle,
            (ushort)CharacterEvent.Initial,
            (ushort)CharacterEvent.Run
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }

    void Start()
    {
        animator = transform.GetComponent<Animator>();
        MsgBase msg = new MsgBase((ushort)CharacterEvent.Die);
        //SendMsg(msg);
    }
}