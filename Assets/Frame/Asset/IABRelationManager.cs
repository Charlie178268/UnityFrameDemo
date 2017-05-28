using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 管理assetBundle包之间依赖关系
/// </summary>
public class IABRelationManager{
	/// <summary>
	/// 比如materia依赖于Png,其dependanceBundle就是png
	/// </summary>
	List<string> dependanceBundle;//依赖的包
	List<string> referBundle;//被依赖的包
	IABLoader assetLoader;
	string theBundleName;
	LoadProgress loadProgress;
	bool isLoadFinish;
	public IABRelationManager(){
		dependanceBundle = new List<string>();
		referBundle = new List<string>();
	}

	//初始化assetLoader，		//这里只给上层提供了loadprogress的回调，没有提供Loadfinished的回调，必要时再写。
	public void Initial(string bundle, LoadProgress progress){
		isLoadFinish = false;
		theBundleName = bundle;
		loadProgress = progress;

		assetLoader = new IABLoader(progress, BundleLoadFinish);

		assetLoader.SetBundleName(bundle);
		string bundlePath = IPathTools.GetWWWAssetBundlePath()+"/"+bundle;
		assetLoader.LoadResources(bundlePath);
	}
	
	public string GetBundleName(){
		return theBundleName;
	}

	//添加Ref关系
	public void AddReffer(string name){
		referBundle.Add(name);
	}
	//获取Ref关系
	public List<string> GetReffer(){
		return referBundle;
	}
	//移除被依赖
	public bool RemoveReffer(string name){
		for (int i=0; i<referBundle.Count; i++){
			if ( name.Equals(referBundle[i])){
				referBundle.RemoveAt(i);
			}
		}
		//如果该包没有被任何包依赖，则释放该包
		if (referBundle.Count <= 0){
			Dispose();
			return true;
		}
		return false;
	}
	//设置依赖包
	public void SetDependance(string[] depence){
		if (depence.Length > 0){
			dependanceBundle.AddRange(depence);
		}
	}
	//获得依赖链
	public List<string> GetDependance(){
		return dependanceBundle;
	}

	public void RemoveDepence(string name){
		for (int i=0; i<dependanceBundle.Count; i++){
			if (name.Equals(dependanceBundle[i])){
				dependanceBundle.RemoveAt(i);
			}
		}
	}


	public bool IsLoadFinish(string name){
		return isLoadFinish;
	}

	void BundleLoadFinish(string bundleName){
		isLoadFinish = true;
	}

	public LoadProgress GetProgress(){
		return loadProgress;
	}



	#region 下层提供api
	//释放功能由下层提供
	public void Dispose(){
		assetLoader.Dispose();
	}

	//获取加载到内存中的单个资源
	public UnityEngine.Object GetResource(string name){
		if (assetLoader != null){
			return assetLoader.GetResource(name);
		}else{
			return null;
		}
	}
	//获取多个资源
	public UnityEngine.Object[] GetMultiResource(string name){
		if (assetLoader != null){
			return assetLoader.GetMultiResource(name);
		}else{
			return null;
		}
	}
	//这种协程的调用方式unity3d 5.3以上协程才可以这么使用
	public IEnumerator LoadAssetBundle(){
		yield return assetLoader.CommonLoad();
	}
	//打印资源名称
	public void DebugAsset(){
		if (assetLoader != null){
			assetLoader.DebugLoader();
		}else{
			Debug.Log("asset loader is null");
		}
	}
	#endregion
}
