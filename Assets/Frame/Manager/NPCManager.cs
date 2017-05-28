using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : ManagerBase {

	//单例模式
	public static NPCManager Instance = null;
	//存储子UI控件，以便直接对子控件发送消息，而不是一级一级的找
	private Dictionary<string, GameObject> ChildrenDic = new Dictionary<string, GameObject>(); 
	void Awake(){
		Instance = this;
	}
	
	public override void ProgressEvent(MsgBase msgbase){
		
	}
	
	public void SendMsg(MsgBase msgbase){
		//如果是本类消息类型，直接通过MsgBase发送消息
		if (msgbase.GetManagerId() == ManagerId.NpcManager){
			ProgressEvent(msgbase);
		}else{//如果不是本类消息，则交由msgcenter去处理
			MsgCenter.Instance.SendToMsg(msgbase);
		}
	}
	
	public GameObject GetGameObject(string name){
		if (ChildrenDic.ContainsKey(name)){
			return ChildrenDic[name];
		}
		return null;
	}
	
	public void RegistGameObject(string name, GameObject obj){
		if (!ChildrenDic.ContainsKey(name)){
			ChildrenDic.Add(name, obj);
		}
	}
	
	public void UnRegistGameObject(string name, GameObject obj){
		if (ChildrenDic.ContainsKey(name)){
			ChildrenDic.Remove(name);
		}
	}
}
