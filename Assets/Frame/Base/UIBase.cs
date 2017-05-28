using UnityEngine;
using System.Collections;

/// <summary>
/// 向UIManager注册脚本和消息
/// 全部引用UIManager的api,中介者模式
/// 一般是panel挂载
/// </summary>
public class UIBase : MonoBase{
	public void RegisSelf(MonoBase mono, params ushort[] msgs){
		UIManager.Instance.RegistMsg(mono, msgs);
	}
	public void UnRegisSelf(MonoBase mono, params ushort[] msgs){
		UIManager.Instance.UnRegistMsg(mono, msgs);
	}
	public void SendMsg(MsgBase msg){
		UIManager.Instance.SendMsg(msg);
	}
	public ushort[] msgIds;
	//自动销毁本身
	void OnDestroy(){
		if (msgIds != null){
			UnRegisSelf(this, msgIds);
		}
	}
	public override void ProgressEvent(MsgBase msgbase){

	}
}
