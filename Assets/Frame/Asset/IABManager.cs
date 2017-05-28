using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 对一个场景的所有AssetBundle包进行管理
/// </summary>

//对上层的接口
public delegate void LoadAssetBundleCallBack(string sceneName, string bundleName);

public class IABManager{
	//存储AssetBundle包
	Dictionary<string, IABRelationManager> loadHelper = new Dictionary<string, IABRelationManager>();

	//存储AssetBundle包里的所有已load的obj
	Dictionary<string, AssetResObj>loadObjDic = new Dictionary<string, AssetResObj>();

	string sceneName;//场景名字
	public IABManager(string tmpSceneName){
		sceneName = tmpSceneName; 
	}

	#region 释放缓存
	//释放包内的某资源
	public void DisposeResObj(string bundleName, string resName){
		if (loadObjDic.ContainsKey(bundleName)){
			AssetResObj tmpObj = loadObjDic[bundleName];
			tmpObj.ReleaseResObj(resName);
		}
	}
	//释放整个包解压出来的资源
	public void DisposeResObj(string bundleName){
		if (loadObjDic.ContainsKey(bundleName)){
			AssetResObj tmpObj = loadObjDic[bundleName];
			tmpObj.ReleaseAllResObj();
		}
		Resources.UnloadUnusedAssets();
	}
	//释放所有缓存的包
	public void DisposeAllObj(){
		List<string> keys = new List<string>();
		keys.AddRange(loadObjDic.Keys);
		for (int i=0; i<keys.Count; i++){
			DisposeResObj(keys[i]);
		}
		loadObjDic.Clear();//清除dictionary
	}
	#endregion


	//外部加载调用接口，上层需提供场景名和包名来加载
	public void LoadAssetBundle(string bundleName, LoadProgress progress, LoadAssetBundleCallBack callBack){
		if (!loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = new IABRelationManager();
			loader.Initial(bundleName, progress);

			loadHelper.Add(bundleName, loader);
			callBack(sceneName, bundleName);
		}else{
			Debug.Log("IABManager have contained bundle name=="+bundleName);
		}
	}

	string[] GetDependances(string bundleName){
		return IABManifestLoader.Instance.GetDepences(bundleName);
	}

	//加载依赖关系
	public IEnumerator LoadAssetBundleDependences(string bundleName, string refName, LoadProgress progress){
		if (!loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = new IABRelationManager();
			loader.Initial(bundleName, progress);

			//记录被依赖的
			if (refName != null){
				loader.AddReffer(refName);
			}

			loadHelper.Add(bundleName, loader);
			yield return LoadAssetBundles(bundleName);
		}else{
			if (refName != null){
				IABRelationManager loader = loadHelper[bundleName];
				loader.AddReffer(refName);
			}
		}
	}

	/// <summary>
	/// 加载AssetBundle，需要先加载manifest
	/// 上层调用callBack
	/// </summary>
	/// <returns>The asset bundle.</returns>
	/// <param name="bundleName">Bundle name.</param>
	public IEnumerator LoadAssetBundles(string bundleName){
		//如果manifest文件没有加载完，就一直等它加载完
		while (!IABManifestLoader.Instance.IsLoadFinish()){
			yield return null;
		}
		IABRelationManager RelationLoader = loadHelper[bundleName];
		//获取依赖关系
		string[] depences = GetDependances(bundleName);
		RelationLoader.SetDependance(depences);
		//加载所有的依赖包
		for (int i=0; i<depences.Length; i++){
			yield return LoadAssetBundleDependences(depences[i], bundleName, RelationLoader.GetProgress());
		}

		yield return RelationLoader.LoadAssetBundle();
	}


	#region 由下层提供api
	//打印一个包里面要加载的资源
	public void DebugAssetBundle(string bundleName){
		if (loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = loadHelper[bundleName];
			loader.DebugAsset();
		}
	}

	//是否加载了assetbundle
	public bool IsLoadFinish(string bundleName){
		if (loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = loadHelper[bundleName];
			return loader.IsLoadFinish(bundleName);
		}
		Debug.Log ("IABRelation don't contain bundle!");
		return false;
	}
	//是否正在加载assetbundle
	public bool IsLoadingAssetBundle(string bundleName){
		if (!loadHelper.ContainsKey(bundleName)){
			return false;
		}
		return true;
	}

	public Object GetSingleRes(string bundleName, string resName){
		//判断是否已经缓存了物体，如果有直接返回缓存中的物体
		if (loadObjDic.ContainsKey(bundleName)){
			AssetResObj tmpRes = loadObjDic[bundleName];
			List<Object> tmpObj = tmpRes.GetResObj(resName);
			if (tmpObj != null){
				return tmpObj[0];
			}
		}
		//如果缓存没有，则判断是否加载过了，没有加载或返回null，加载或了则添加到缓存中，再返回这个物体
		//判断是否加载过bundle
		if (loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = loadHelper[bundleName];
			Object tmpObj = loader.GetResource(resName);
			AssetObj tmpAssetObj = new AssetObj(tmpObj);
			//判断缓存里是否有这个包，如果有，加入到这个包的缓存里
			if (loadObjDic.ContainsKey(bundleName)){
				AssetResObj tmpRes = loadObjDic[bundleName];
				tmpRes.AddResObj(resName, tmpAssetObj);
			}else{
				//缓存没有这个包，创建这个缓存包
				AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
				loadObjDic.Add(bundleName, tmpRes);
			}
			return tmpObj;
		}else{
			return null; 
		}
	}

	public Object[] GetMultiRes(string bundleName, string resName){
		//判断是否已经缓存了物体
		if (loadObjDic.ContainsKey(bundleName)){
			AssetResObj tmpRes = loadObjDic[bundleName];
			List<Object> tmpObj = tmpRes.GetResObj(resName);
			if (tmpObj != null){
				return tmpObj.ToArray();
			}
		}
		//表示已经加载过bundle
		if (loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = loadHelper[bundleName];
			Object[] tmpObj = loader.GetMultiResource(resName);
			AssetObj tmpAssetObj = new AssetObj(tmpObj);
			//加入缓存
			if (loadObjDic.ContainsKey(bundleName)){
				AssetResObj tmpRes = loadObjDic[bundleName];
				tmpRes.AddResObj(resName, tmpAssetObj);
			}else{
				//没有加载过这个包
				AssetResObj tmpRes = new AssetResObj(resName, tmpAssetObj);
				loadObjDic.Add(bundleName, tmpRes);
			}
			return tmpObj;
		}else{
			return null; 
		}
	}

	//先卸载依赖关系，再卸载本身,循环处理依赖关系
	public void DisposeBundle(string bundleName){
		if (loadHelper.ContainsKey(bundleName)){
			IABRelationManager loader = loadHelper[bundleName];
			List<string> depences = loader.GetDependance();
			for (int i=0; i<depences.Count; i++){
				if (loadHelper.ContainsKey(depences[i])){
					IABRelationManager dependManger = loadHelper[depences[i]];
					if (dependManger.RemoveReffer(bundleName)){//取出list记录的引用
						DisposeBundle(dependManger.GetBundleName());//释放assetBundle
					}
				}
			}
			if (loader.GetReffer().Count <= 0){
				loader.Dispose();
				loadHelper.Remove(bundleName);
			}
		}
	}

	//卸载Bundle和已加载的所有obj，不必考虑依赖关系
	public void DisposeAllBundleAndObj(){
		DisposeAllObj();
		DisposeAllBundle();
	}

	public void DisposeAllBundle(){
		List<string> keys = new List<string>();
		keys.AddRange(loadHelper.Keys);
		for (int i=0; i<loadHelper.Count; i++){
			IABRelationManager loader = loadHelper[keys[i]];
			loader.Dispose();
		}
		loadHelper.Clear();
	}

	#endregion
}
//存储单个obj，有可能是单个obj，也有可能是多个(合图)
public class AssetObj{
	public List<Object> objs;
	public AssetObj(params Object[] tmpObj){
        objs = new List<Object>();
		objs.AddRange(tmpObj);
	}
	public void ReleaseObj(){
		for (int i=0; i<objs.Count; i++){
            if (objs[i].GetType() != typeof(UnityEngine.GameObject))
            {
                Resources.UnloadAsset(objs[i]);//只能卸载非Instantiate加载的asset类型的资源，比如texture
            }
		}
        objs.Clear();
	}
}
//存储一个assetbundle里的obj
public class AssetResObj{
	public Dictionary<string, AssetObj> resObjs;
	public AssetResObj(string name, AssetObj res){
		resObjs = new Dictionary<string, AssetObj>();
		resObjs.Add(name, res);
	}
	public void AddResObj(string name, AssetObj res){
		resObjs.Add(name, res);
	}
	//释放所有
	public void ReleaseAllResObj(){
		List<string> keys = new List<string>();
		keys.AddRange(resObjs.Keys);
		for (int i=0; i<keys.Count; i++){
			ReleaseResObj(keys[i]);
		}
	}
	//释放单个
	public void ReleaseResObj(string name){
		if (resObjs.ContainsKey(name)){
			AssetObj obj = resObjs[name];
			obj.ReleaseObj();
		}else{
			Debug.Log("Release object is not exit == "+name);
		}
	}
	//获取
	public List<Object>GetResObj(string name){
		if (resObjs.ContainsKey(name)){
			return resObjs[name].objs;
		}else{
			return null;
		}
	}
}
