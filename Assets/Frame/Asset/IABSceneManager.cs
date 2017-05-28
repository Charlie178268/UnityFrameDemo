using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 通过配置文件找到包名
/// </summary>
public class IABSceneManager{
	IABManager abManager;

	
	private Dictionary<string, string> allAsset;
	//fileName是场景名，如SceneOne
	public void ReadConfiger(string sceneName){
		string txtFileName = "Record.txt";
		//找到配置文件路径
		string path = IPathTools.GetAssetBundlePath()+"/"+sceneName+txtFileName;
		allAsset = new Dictionary<string, string>();
		abManager = new IABManager(sceneName);
		ReadConfig(path);
	}

	//读取存储的包名对应关系
	private void ReadConfig(string path){
		//读取stream，做项目时应该读取二进制
		FileStream fs = new FileStream(path, FileMode.Open);
		StreamReader br = new StreamReader(fs);
		string line = br.ReadLine();
		int allCount = int.Parse(line);
		for (int i=0; i<allCount; i++){
			string tmpStr = br.ReadLine();
			string[] tmpArr = tmpStr.Split(" ".ToCharArray());
			allAsset.Add (tmpArr[0], tmpArr[1]);
		}
		br.Close();
		fs.Close();
	}
	//传入load
	public void LoadAsset(string bundleName, LoadProgress progress, LoadAssetBundleCallBack callback){

		if (allAsset.ContainsKey(bundleName)){
			string tmpValue = allAsset[bundleName];//得到sceneOne/load.ld
			abManager.LoadAssetBundle(tmpValue, progress, callback);
		}else{
			Debug.Log("Don't contain the bundle =="+ bundleName);
		}
	}

	#region 由下层提供功能

	public string GetBundleRelaName(string bundleName){
		if (allAsset.ContainsKey(bundleName)){
			return allAsset[bundleName];
		}else{
			return null;
		}
	}

	public IEnumerator LoadAssetSys(string bundleName){
		yield return abManager.LoadAssetBundles(bundleName);
	}

	public Object GetSingleResources(string bundleName, string resName){
		if (allAsset.ContainsKey(bundleName)){
			return abManager.GetSingleRes(allAsset[bundleName], resName);
		}else{
			Debug.Log("Don't contain bundleName =="+ bundleName);
			return null;
		}
	}

	public Object[] GetMultiResources(string bundleName, string resName){
		if (allAsset.ContainsKey(bundleName)){
			return abManager.GetMultiRes(allAsset[bundleName], resName);
		}else{
			Debug.Log("Don't contain bundleName =="+ bundleName);
			return null;
		}
	}
	//释放某包解压出来的某资源
	public void DisposeResObj(string bundleName, string resName){
		if (allAsset.ContainsKey(bundleName)){
			abManager.DisposeResObj(allAsset[bundleName], resName);
		}else{
			Debug.Log("Don't contain bundleName =="+ bundleName);
		}
	}
	//释放整个包解压出来的资源
	public void DisposeBundleRes(string bundleName){
		if (allAsset.ContainsKey(bundleName)){
			abManager.DisposeResObj(allAsset[bundleName]);
		}else{
			Debug.Log("Don't contain bundleName =="+ bundleName);
		}
	}
	//释放所有已解压的资源
	public void DisposeAllRes(){
		abManager.DisposeAllObj();
	}
	//释放特定包
	public void DisposeBundle(string bundleName){
		if (allAsset.ContainsKey(bundleName)){
			abManager.DisposeBundle(bundleName);
		}
	}
	//释放所有包
	public void DisposeAllBundle(){
		abManager.DisposeAllBundle();
		allAsset.Clear();
	}
	//释放所有包和解压的资源
	public void DisposeAllBundleAndRes(){
		abManager.DisposeAllBundleAndObj();
	}

	public void DebugAllAsset(){
		List<string> keys = new List<string>();
		keys.AddRange(allAsset.Keys);
		for (int i=0; i<keys.Count; i++){
			abManager.DebugAssetBundle(allAsset[keys[i]]);
		}
	}

	//sceneone/test.ld
	//bundlename = test
	public bool IsLoadingFinish(string bundleName){
		if (allAsset.ContainsKey(bundleName)){
			return abManager.IsLoadFinish(allAsset[bundleName]);
		}else{
			Debug.Log("is not contain bundle =="+ bundleName);
			return false;
		}
	}

	public bool IsLoadingAssetBundle(string bundleName){
		if (allAsset.ContainsKey(bundleName)){
			return abManager.IsLoadingAssetBundle(allAsset[bundleName]);
		}else{
			Debug.Log("is not contain bundle =="+ bundleName);
			return false;
		}
	}

	#endregion

}