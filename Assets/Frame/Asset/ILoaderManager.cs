using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 加载IABManifest，assetbundle
/// </summary>
public class ILoaderManager : MonoBehaviour {

	public static ILoaderManager Instance = null;


	void Awake(){
		Instance = this;
		//第一步，协程加载IABManifest
		StartCoroutine(IABManifestLoader.Instance.LoadManifest());

	}

	IABSceneManager sceneManger;
	//sceneName, manager
	private Dictionary<string, IABSceneManager> loadManager = new Dictionary<string, IABSceneManager>();
	//第二步，读取配置文件，找到依赖关系
	public void ReadConfiger(string sceneName){
		if (!loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = new IABSceneManager();
			tmpManager.ReadConfiger(sceneName);
			loadManager.Add(sceneName, tmpManager);
		}
	}
	//传递给IABManager的回调函数，启动协程加载
	public void LoadCallBack(string sceneName, string bundleName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			StartCoroutine(tmpManager.LoadAssetSys(bundleName));//只有loadmanager有启动协程的功能，其他层要启动只有回调回这一层
		}else{
			Debug.LogError("bundle name is not contain == "+ bundleName);
		}
	}

	public void LoadAsset(string sceneName, string bundleName, LoadProgress progress){
		if (!loadManager.ContainsKey(sceneName)){
			ReadConfiger(sceneName);
		}
		IABSceneManager tmpManager = loadManager[sceneName];
		tmpManager.LoadAsset(bundleName, progress, LoadCallBack);
	}

	#region 下层提供功能

	public string GetBundleRelateName(string sceneName, string bundleName){
		IABSceneManager tmpManager = loadManager[sceneName];
		if (tmpManager != null){
			return tmpManager.GetBundleRelaName(bundleName);
		}else{
			return null;
		}
	}
	/// <summary>
	/// Gets the single resources.
	/// </summary>
	/// <returns>The single resources.</returns>
	/// <param name="sceneName">SceneOne</param>
	/// <param name="bundleName">Load</param>
	/// <param name="resName">cube.prefeb</param>
	public Object GetSingleResources(string sceneName, string bundleName, string resName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			return tmpManager.GetSingleResources(bundleName, resName);
		}else{
			Debug.Log("SceneName=="+sceneName+"BundleName"+bundleName+"is not load");
			return null;
		}
	}

	public Object[] GetMultiResources(string sceneName, string bundleName, string resName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			return tmpManager.GetMultiResources(bundleName, resName);
		}else{
			Debug.Log("SceneName=="+sceneName+"BundleName"+bundleName+"is not load");
			return null;
		}
	}
	//释放某场景某包内的某资源
	public void UnLoadResObj(string sceneName, string bundleName, string res){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DisposeResObj(bundleName, res);
		}
	}
	//释放某场景的整个包的objs
	public void UnLoadBundleResObj(string sceneName, string bundleName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DisposeResObj(sceneName, bundleName);
		}
	}
	//释放整个场景的objs
	public void UnLoadAllResObjs(string sceneName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DisposeAllRes();
		}
	}
	//释放某场景的某个包
	public void UnLoadAssetBundle(string sceneName, string bundleName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DisposeBundle(bundleName);
		}
	}
	//释放一个场景的全部bundle
	public void UnloadAllAssetBundle(string sceneName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DisposeAllBundle();
			System.GC.Collect();//启动垃圾回收
		}
	}
	//释放一个场景的全部bundle和obj
	public void UnLoadAllAssetBundleAndResObjs(string sceneName)
	{
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DisposeAllBundleAndRes();
			System.GC.Collect();
		}
	}

	#endregion
	//输出某个场景加载的所有资源
	public void DebugAllAssetBundle(string sceneName){
		if (loadManager.ContainsKey(sceneName)){
			IABSceneManager tmpManager = loadManager[sceneName];
			tmpManager.DebugAllAsset();
		}
	}

	public bool IsLoadingBundleFinish(string sceneName, string bundleName){
		bool tmpBool = loadManager.ContainsKey(sceneName);
		if (tmpBool){
			IABSceneManager tmpManager = loadManager[sceneName];
			return tmpManager.IsLoadingFinish(bundleName);
		}

		return false;
	}

	public bool IsLoadingAssetBundle(string sceneName, string bundleName){
		bool tmpBool = loadManager.ContainsKey(sceneName);
		if (tmpBool){
			IABSceneManager tmpManager = loadManager[sceneName];
			return tmpManager.IsLoadingAssetBundle(bundleName);
		}
		
		return false;
	}

	void OnDestroy(){
		loadManager.Clear();
		System.GC.Collect();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
