using UnityEngine;
using System.Collections;

public enum AssetEvent{
	HunkRes = ManagerId.AssetManager + 1,
	ReleaseSingleObj,
	ReleaseBundleObj,
	ReleaseSceneObj,
	ReleaseSingleBundle,
	ReleaseSceneBundle,
	ReleaseAll
}

//上层需要发给我的消息
public class HunkAssetRes: MsgBase {
	public string sceneName;
	public string bundleName;
	public string resName;
	public ushort backMsgId;
	public bool isSingle;
	public HunkAssetRes(bool tmpSingle, ushort msgId, string tmpSceneName, string tmpBundle, string tmpRes, ushort tmpBackId){
		this.isSingle = tmpSingle;
		this.msgId = msgId;
		this.sceneName = tmpSceneName;
		this.bundleName = tmpBundle;
		this.resName = tmpRes;
		this.backMsgId = tmpBackId;
	}
}
//返回给上层的消息，资源加载后发送给上层的消息
	public class HunkAssetBack:MsgBase{
		public Object[] value;
		public HunkAssetBack(){
			this.msgId = 0;
			this.value = null;
		}
		public void Changer(ushort msgId, params Object[] tmpValue){
			this.msgId = msgId;
			this.value = tmpValue;
		}
		public void Changer(ushort msgId){
			this.msgId = msgId;
		}
		public void Changer(params Object[] tmpValue){
			this.value = tmpValue;
		}
	}
