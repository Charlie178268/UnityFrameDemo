using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// pannel挂载
/// 负责存储子控件和消息分类处理
/// </summary>
public class AssetManager : ManagerBase {
	//单例模式
	public static AssetManager Instance = null;
	void Awake(){
		Instance = this;
	}
	
	public void SendMsg(MsgBase msgbase){
		//如果是本类消息类型，直接通过MsgBase发送消息
		if (msgbase.GetManagerId() == ManagerId.AssetManager){
			ProgressEvent(msgbase);//???这里调用的是基类的
		}else{//如果不是本类消息，则交由msgcenter去处理ProgressEvent，也是相当于全部遍历，没有达到分模块传递的效果？
			MsgCenter.Instance.SendToMsg(msgbase);
		}
	}
}
