using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SkillCtr : UIBase
{
    public override void ProgressEvent(MsgBase msgbase)
    {
        switch (msgbase.msgId)
        {
            case (ushort)(BloodEvent.Loose):
                Debug.Log("bllod");
                MsgFloat tmpMsg = msgbase as MsgFloat;
                bloodCtr.SetBlood(tmpMsg.v);
                break;
            case (ushort)(BloodEvent.Add):
               
                break;
        }
    }

    void Awake()
    {
        //定义要注册的消息
        msgIds = new ushort[]{
            (ushort)BloodEvent.Loose,
            (ushort)BloodEvent.Add
        };
        //先把自己注册进消息队列
        RegisSelf(this, msgIds);
    }

    SkillTime skillTimeScriptOne;
    SkillTime skillTimeScriptTwo;
    SkillTime skillTimeScriptThree;

    BloodCtr bloodCtr;
    MsgBase attackMsg;

    void Start()
    {
        //技能1
        GameObject skillOne  = UIManager.Instance.GetGameObject("skill01");
        skillOne.GetComponent<UIBehaviour>().AddButtonDownListener(OnSkillOneButtonDown);
        skillOne.GetComponent<UIBehaviour>().AddButtonUpListener(OnSkillOneButtonUp);
        skillTimeScriptOne = skillOne.GetComponent<SkillTime>();
        //技能2
        GameObject skillTwo = UIManager.Instance.GetGameObject("skill02");
        skillTwo.GetComponent<UIBehaviour>().AddButtonDownListener(OnSkillTwoButtonDown);
        skillTwo.GetComponent<UIBehaviour>().AddButtonUpListener(OnSkillTwoButtonUp);
        skillTimeScriptTwo = skillTwo.GetComponent<SkillTime>();
        //技能2
        GameObject skillThree = UIManager.Instance.GetGameObject("skill03");
        skillThree.GetComponent<UIBehaviour>().AddButtonDownListener(OnSkillThreeButtonDown);
        skillThree.GetComponent<UIBehaviour>().AddButtonUpListener(OnSkillThreeButtonUp);
        skillTimeScriptThree = skillThree.GetComponent<SkillTime>();

        attackMsg = new MsgBase((ushort)CharacterEvent.AttackBig);

        bloodCtr = UIManager.Instance.GetGameObject("blood").GetComponent<BloodCtr>();
    }

    public void OnSkillOneButtonDown(BaseEventData baseEventData)
    {
        skillTimeScriptOne.OnButtonDown();
        attackMsg.ChangeMsgId((ushort)CharacterEvent.AttackBig);
        SendMsg(attackMsg);
    }

    public void OnSkillOneButtonUp(BaseEventData baseEventData)
    {
        attackMsg.ChangeMsgId((ushort)CharacterEvent.Idle);
        SendMsg(attackMsg);
    }

    public void OnSkillTwoButtonDown(BaseEventData baseEventData)
    {
        skillTimeScriptTwo.OnButtonDown();
        attackMsg.ChangeMsgId((ushort)CharacterEvent.AttackBig);
        SendMsg(attackMsg);
    }

    public void OnSkillTwoButtonUp(BaseEventData baseEventData)
    {
        attackMsg.ChangeMsgId((ushort)CharacterEvent.Idle);
        SendMsg(attackMsg);
    }

    public void OnSkillThreeButtonDown(BaseEventData baseEventData)
    {
        skillTimeScriptThree.OnButtonDown();
        attackMsg.ChangeMsgId((ushort)CharacterEvent.AttackBig);
        SendMsg(attackMsg);
    }

    public void OnSkillThreeButtonUp(BaseEventData baseEventData)
    {
        attackMsg.ChangeMsgId((ushort)CharacterEvent.Idle);
        SendMsg(attackMsg);
    }

}
