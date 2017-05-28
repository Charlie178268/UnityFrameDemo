using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void NativeResCallBack(NativeResCallBackNode tmpNode);

//存放加载assetbundle的消息链表节点
public class NativeResCallBackNode{
	public string sceneName;
	public string bundleName;
	public string resName;
	public ushort backMsgId;
	public bool isSingle;
	public NativeResCallBackNode next;
	public NativeResCallBack callBack;
	public NativeResCallBackNode(bool tmpSingle, string tmpSceneName, string tmpBundle, 
	                             string tmpRes, ushort tmpBackId,NativeResCallBackNode nextNode, NativeResCallBack callback){
		this.isSingle = tmpSingle;
		this.sceneName = tmpSceneName;
		this.bundleName = tmpBundle;
		this.backMsgId = tmpBackId;
		this.resName = tmpRes;
		this.callBack = callback;
		this.next = nextNode;
	}

	public void Dispose(){
		this.bundleName = null;
		this.bundleName = null;
		this.resName = null;
		this.next = null;
	}
}



public class NativeResCallBackManager{
	Dictionary<string, NativeResCallBackNode> manager = null;
	public NativeResCallBackManager(){
		manager = new Dictionary<string, NativeResCallBackNode>();
	}
	//添加入消息队列
	public void AddBundle(string bundle, NativeResCallBackNode curNode){
		if (manager.ContainsKey(bundle)){
			NativeResCallBackNode tmpNode = manager[bundle];
			while (tmpNode.next != null){
				tmpNode = tmpNode.next;
			}
			tmpNode.next = curNode;
		}else{
			manager.Add(bundle, curNode);
		}
	}
	//加载完成后，释放缓存的命令
	public void Dispose(string bundle){
		if (manager.ContainsKey(bundle)){
			NativeResCallBackNode tmpNode = manager[bundle];
			//释放整个链表
			while (tmpNode.next != null){
				NativeResCallBackNode curNode= tmpNode; 
				tmpNode = tmpNode.next;
				curNode.Dispose();
			}
			tmpNode.Dispose();

			manager.Remove(bundle);
		}
	}
	//传递给上层的回调函数
	public void CallBackRes(string bundle){
		if (manager.ContainsKey(bundle)){
			NativeResCallBackNode tmpNode = manager[bundle];
			do {
				tmpNode.callBack(tmpNode);
				tmpNode = tmpNode.next;
			}while(tmpNode != null);
		}
	}
}

//

public class NativeResLoader : AssetBase {

	public override void ProgressEvent(MsgBase recMsg){
		HunkAssetRes tmpMsg = recMsg as HunkAssetRes;
		switch(recMsg.msgId){
		case (ushort)AssetEvent.ReleaseSceneObj:
			ILoaderManager.Instance.UnLoadAllResObjs(tmpMsg.sceneName);
			break;
		case (ushort)AssetEvent.ReleaseAll:
			ILoaderManager.Instance.UnLoadAllAssetBundleAndResObjs(tmpMsg.sceneName);
			break;
		case (ushort)AssetEvent.ReleaseBundleObj:
			ILoaderManager.Instance.UnLoadAllResObjs(tmpMsg.sceneName);
			break;
		case (ushort)AssetEvent.ReleaseSceneBundle:
			ILoaderManager.Instance.UnloadAllAssetBundle(tmpMsg.sceneName);
			break;
		case (ushort)AssetEvent.ReleaseSingleBundle:
	
			ILoaderManager.Instance.UnLoadAssetBundle(tmpMsg.sceneName, tmpMsg.bundleName);

			break;
		case (ushort)AssetEvent.ReleaseSingleObj:
			ILoaderManager.Instance.UnLoadResObj(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.resName);
			break;
		//请求资源
		case (ushort)AssetEvent.HunkRes:
			GetResources(tmpMsg.sceneName, tmpMsg.bundleName, tmpMsg.resName, tmpMsg.isSingle, tmpMsg.backMsgId);
			break;
        case (ushort)AssetEvent.HunkRes + 10:
                Debug.Log("收到了消息");
                GameObject obj1 = resBackMsg.value[0] as GameObject;
                Debug.Log(resBackMsg);
                Instantiate(obj1);
                obj1.transform.position = new Vector3(0, 0, 0);
               // obj1.GetComponent<Renderer>().material.color = Color.green;
        
                break;

        }
}

	HunkAssetBack resBackMsg = null;
	HunkAssetBack ReleaseBack{
		get {
			if (resBackMsg == null){
				resBackMsg = new HunkAssetBack();
			}
			return resBackMsg;
		}
	}

	NativeResCallBackManager callBack = null;
	NativeResCallBackManager CallBack{
		get{
			if (callBack == null){
				callBack = new NativeResCallBackManager();
			}
			return callBack;
		}
	}


	void Awake(){
		msgIds = new ushort[]{
			(ushort)AssetEvent.ReleaseSceneObj,
			(ushort)AssetEvent.ReleaseAll,
			(ushort)AssetEvent.ReleaseBundleObj,
			(ushort)AssetEvent.ReleaseSceneBundle,
			(ushort)AssetEvent.ReleaseSingleBundle,
			(ushort)AssetEvent.ReleaseSingleObj,
			(ushort)AssetEvent.HunkRes,
            (ushort)AssetEvent.HunkRes+10,
        };
		RegisSelf(this, msgIds);
	}

	void Start(){
		Debug.Log("Start ResLoad!");
		//MsgBase msg = new HunkAssetRes(true, (ushort)AssetEvent.HunkRes, "sceneone", "Load", "Cube", (ushort)AssetEvent.HunkRes+10);
		//SendMsg(msg);
		//GameObject obj1 = resBackMsg.value[0] as GameObject;
		//Debug.Log(resBackMsg);
		//Instantiate(obj1);
		//obj1.transform.position = new Vector3(0, 0, 0);
	}

	//node回调
	public void SendTOBackMsg(NativeResCallBackNode tmpNode){
		if (tmpNode.isSingle){
			Object tmpObj = ILoaderManager.Instance.GetSingleResources(tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);
			this.ReleaseBack.Changer(tmpNode.backMsgId, tmpObj);
			SendMsg(ReleaseBack);
		}else{
			Object[] tmpObj = ILoaderManager.Instance.GetMultiResources(tmpNode.sceneName, tmpNode.bundleName, tmpNode.resName);
			this.ReleaseBack.Changer(tmpNode.backMsgId, tmpObj);
			SendMsg(ReleaseBack);
		}
	}

	void LoaderProgress(string bundleName, float progress){
		if (progress >= 1.0f){
			callBack.CallBackRes(bundleName);
			callBack.Dispose(bundleName);
		}
	}
	//获取资源统一的api
	public void GetResources(string sceneName, string bundleName, string res, bool single, ushort backid){
		//没有加载，把命令存起来，等加载完成后返回
		if (!ILoaderManager.Instance.IsLoadingAssetBundle(sceneName, bundleName)){
			//加载
			ILoaderManager.Instance.LoadAsset(sceneName, bundleName, LoaderProgress);
			string bundleFullName = ILoaderManager.Instance.GetBundleRelateName(sceneName, bundleName);
			if (bundleFullName != null){
				NativeResCallBackNode tmpNode = new NativeResCallBackNode(single, sceneName, bundleName, res,
				                                                backid, null, SendTOBackMsg);
			
				//bundlename sceneOne/load.ld
				CallBack.AddBundle(bundleFullName, tmpNode);
			}else{
				Debug.LogWarning("Don't contain bundle =="+bundleName);
			}
		}//已经加载完成,直接返回
		else if (ILoaderManager.Instance.IsLoadingBundleFinish(sceneName, bundleName)){
			if (single){
				Object tmpObj = ILoaderManager.Instance.GetSingleResources(sceneName, bundleName, res);
				this.ReleaseBack.Changer(backid, tmpObj);
				SendMsg(ReleaseBack);
			}else{
				Object[] tmpObj = ILoaderManager.Instance.GetMultiResources(sceneName, bundleName, res);
				this.ReleaseBack.Changer(backid, tmpObj);
				SendMsg(ReleaseBack);
			}
		}else{//正在加载，把消息放入队列中，等加载完了再执行
			string bundleFullName = ILoaderManager.Instance.GetBundleRelateName(sceneName, bundleName);
			if (bundleFullName != null){
				NativeResCallBackNode tmpNode = new NativeResCallBackNode(single, sceneName, bundleName, res,
				                                                          backid, null, SendTOBackMsg);
				
				CallBack.AddBundle(bundleName, tmpNode);
			}else{
				Debug.LogWarning("Don't contain bundle =="+bundleName);
			}
		}
	}
}
