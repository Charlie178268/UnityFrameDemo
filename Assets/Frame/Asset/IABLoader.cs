using UnityEngine;
using System.Collections;

//每一帧的回调，代理模式，通知上层
public delegate void LoadProgress(string bundle, float progress);
public delegate void LoadFinished(string bundle);//加载完成的回调

public class IABLoader{
	private string bundleName;
	private string commonBundlePath;//assetBundle路径
	private WWW commonLoader;//加载器
	private float commonLoadProcess;//加载资源进度 
	private LoadProgress loadProgress;
	private LoadFinished loadFinished;
	private IABResLoader abResLoader;//使用解压器
	/// <summary>
	/// Initializes a new instance of the <see cref="IABLoader"/> class.
	/// </summary>
	/// <param name="func1">加载资源时的回调</param>
	/// <param name="func2">加载完成后的回调</param>
	public IABLoader(LoadProgress func1, LoadFinished func2){
		bundleName = "";
		commonBundlePath = "";

		commonLoadProcess = 0.0f;
		loadProgress = func1;
		loadFinished = func2;
		abResLoader = null;
	}
	//要求上层传递完整路径
	public void LoadResources(string path){
		commonBundlePath = path;
	}

	//设置包名，如SceneOne/test.prefeb
	public void SetBundleName(string name){
		this.bundleName = name;
	}

	//协程加载,提供loadprogress和loadFinished代理返回
	public IEnumerator CommonLoad(){
		commonLoader = new WWW(commonBundlePath);//视频39_1代码有修改，说WWW在安卓下不能加载，必须用LoadFromFileAsyn
		while (!commonLoader.isDone){
			commonLoadProcess = commonLoader.progress;
			if (loadProgress != null){
				loadProgress(bundleName, commonLoadProcess);
			}
			yield return commonLoader.progress;
			commonLoadProcess = commonLoader.progress;
		}

		if (commonLoadProcess >= 1.0f){//加载完成

			abResLoader = new IABResLoader(commonLoader.assetBundle);

			if (loadProgress != null){
				loadProgress(bundleName, commonLoadProcess);
			}
			if (loadFinished != null){
				loadFinished(bundleName);
			}
		}else{
			Debug.LogError("load bundle error == "+ bundleName );
		}
		commonLoader = null;//加载完成之后释放
	}
	#region 下层提供api
	/// <summary>
	/// 调用下层提供的功能
	/// </summary>
	//打印资源名称
	public void DebugLoader(){
		if (abResLoader != null){
			abResLoader.DebugAllRes();
		}
	}
	//获取解压到内存中的单个资源
	public UnityEngine.Object GetResource(string name){
		if (abResLoader != null){
			return abResLoader[name];
		}else{
			return null;
		}
	}
	//获取多个资源
	public UnityEngine.Object[] GetMultiResource(string name){
		if (abResLoader != null){
			return abResLoader.LoadMultiRes(name);
		}else{
			return null;
		}
	}
	//释放加载到内存中未被解压的包
	public void Dispose(){
		if (abResLoader != null){
			abResLoader.Dispose();
			abResLoader = null;
		}
	}
	//释放加载到内存中已经被解压的包
	public void UnloadAssetRes(UnityEngine.Object obj){
		if (abResLoader != null){
			abResLoader.UnloadRes(obj);
		}
	}
	#endregion
}
