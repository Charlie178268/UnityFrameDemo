using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// pannel挂载
/// 负责存储子控件和消息分类处理
/// </summary>
public class UIManager : ManagerBase {
	//单例模式
	public static UIManager Instance = null;
	//存储子UI控件，以便直接对子控件发送消息，而不是一级一级的找
	private Dictionary<string, GameObject> ChildrenDic = new Dictionary<string, GameObject>(); 
	void Awake(){
		Instance = this;
	}

	public void SendMsg(MsgBase msgbase){
		//如果是本类消息类型，直接通过MsgBase发送消息
		if (msgbase.GetManagerId() == ManagerId.UIManager){
			ProgressEvent(msgbase);//这里调用的是基类的
		}else{//如果不是本类消息，则交由msgcenter去处理ProgressEvent，也是相当于全部遍历，没有达到分模块传递的效果？
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
