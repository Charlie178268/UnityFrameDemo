using UnityEngine;
using System.Collections;

public class NPCBase : MonoBase {

	public void RegisSelf(MonoBase mono, params ushort[] msgs){
		NPCManager.Instance.RegistMsg(mono, msgs);
	}
	public void UnRegisSelf(MonoBase mono, params ushort[] msgs){
		NPCManager.Instance.UnRegistMsg(mono, msgs);
	}
	public void SendMsg(MsgBase msg){
		NPCManager.Instance.SendMsg(msg);
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
