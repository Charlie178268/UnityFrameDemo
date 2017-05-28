using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;


[RequireComponent(typeof(Animator))]
public class MonsterNpcAnimator : NPCBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)(NPCEvent.Initial):

                break;
            case (ushort)(NPCEvent.Idle):
                animator.SetInteger("state", 0);
                break;
            case (ushort)(NPCEvent.Attack):

                break;
            case (ushort)(NPCEvent.Run):
                animator.SetInteger("state", 1);
                break;
            case (ushort)(NPCEvent.Death):
                animator.SetInteger("state", 2);
                break;
        }
    }

    void Awake()
    {
        msgIds = new ushort[]{
            (ushort)NPCEvent.Attack,
            (ushort)NPCEvent.Death,
            (ushort)NPCEvent.Idle,
            (ushort)NPCEvent.Initial,
            (ushort)NPCEvent.Run,
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }
    Animator animator;
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }
}
