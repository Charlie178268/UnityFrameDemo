
using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 从内存中解压加载的资源
/// 实现IDisposable接口，提供自动释放未解压的assetbundle包的功能
/// </summary>
public class IABResLoader:IDisposable{
	private AssetBundle ABRes;
	public IABResLoader(AssetBundle ab)
	{
		this.ABRes = ab;
	}
	//建造者模式，提供基础功能，从内存中解压单个资源
	//索引器
	public UnityEngine.Object this[string resName]{
		get{
			if (ABRes==null || !ABRes.Contains(resName)){
				Debug.Log("this res is not exist!");
				return null;
			}
			return ABRes.LoadAsset(resName);
		}
	}
	//从内存中解压多个资源，如合图
	public UnityEngine.Object[] LoadMultiRes(string resName){
		if (ABRes==null || !ABRes.Contains(resName)){
			Debug.Log("this res is not exist!");
			return null;
		}

		return ABRes.LoadAssetWithSubAssets(resName);
	}
	//卸载解压后的资源，无论引用过的还是没有引用过的资源都会被卸载
	public void UnloadRes(UnityEngine.Object obj){
		Resources.UnloadAsset(obj);
	}
	//释放原来未解压的AssetBundle包
	public void Dispose(){
		if (this.ABRes == null){
			return;
		}
		ABRes.Unload(false);//这里参数为false，表示只释放未解压的，不释放已经解压的，为true表示都释放
	}
	//打印出AssetBundle包含的所有资源
	public void DebugAllRes(){
		string[] tmpAssetName = ABRes.GetAllAssetNames();
		for (int i=0; i<tmpAssetName.Length; i++){
			Debug.Log("ABRes Contain asset name="+ tmpAssetName[i]);
		}
	}

}
