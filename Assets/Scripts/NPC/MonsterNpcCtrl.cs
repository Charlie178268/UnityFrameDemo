using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterNpcCtrl : NPCBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)(NPCEvent.Initial):
              
                break;
            case (ushort)(NPCEvent.Idle):
               
                break;
            case (ushort)(NPCEvent.Attack):

                break;
            case (ushort)(NPCEvent.Run):

                break;
            case (ushort)(NPCEvent.Death):

                break;
        }
    }

    void Awake()
    {
        //定义要注册的消息
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

    public enum NPCState
    {
        idle,
        run,
        attack,
        die
    }
    NavMeshAgent navMeshAgent;
    Transform target;
    NPCState npcState;
    float timeCout;
    MsgBase npcMsg;
    public float deltaDist = 4.0f;
    MsgFloat bloodReduceMsg;
    void Start()
    {
        timeCout = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player").transform;
        npcMsg = new MsgBase((ushort)NPCEvent.Idle);
        npcState = NPCState.run;
        bloodReduceMsg = new MsgFloat((ushort)CharacterEvent.looseBlood, 0);
    }

    public void Update()
    {
        if (npcState == NPCState.attack)
        {
            timeCout += Time.deltaTime;
            if (timeCout > 2.0f)//攻击2秒减一次血
            {
                timeCout = 0;
                bloodReduceMsg.v = 20;
                SendMsg(bloodReduceMsg);
                Debug.Log("见血");
            }

        }else if(npcState == NPCState.idle) {
            
        }
        else if (npcState == NPCState.die)
        {
            timeCout += Time.deltaTime;
            if (timeCout > 2.0f)
            {
                timeCout = 0f;
                npcState = NPCState.idle;
            }
        }
        
            MoveNpc();
    }

    public void MoveNpc()
    {
        Vector3 tmpDist = (target.position - transform.position);//npc到玩家的方向向量
        Debug.Log(tmpDist.magnitude);
        //如果距离小于2就攻击
        if (tmpDist.magnitude < 4)
        {
            if (npcState == NPCState.run)
            {
                timeCout = 0;
                npcState = NPCState.attack;
                npcMsg.ChangeMsgId((ushort)NPCEvent.Attack);
                SendMsg(npcMsg);
                Debug.Log("小于2");
            }
        }
        else
        {
            if (npcState == NPCState.attack)
            {
                timeCout = 0;
                npcState = NPCState.run;
                npcMsg.ChangeMsgId((ushort)NPCEvent.Run);
                SendMsg(npcMsg);
            }
        }

        Debug.DrawLine(transform.position, target.position, Color.red);
        Vector3 tmpLoc = tmpDist - deltaDist * tmpDist.normalized;
        //.normalized是将向量方向不变，而长度变为1，注意只返回改变后的向量而不改变原来的向量，而Normalize()是改变原来的向量
        navMeshAgent.destination = tmpLoc + transform.position;//每次走距离的一部分

        Debug.DrawLine(transform.position, navMeshAgent.transform.position, Color.green);
        transform.LookAt(target.position);//默认up方向为y轴，然后forward朝向目标
    }
}
