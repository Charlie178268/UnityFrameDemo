using UnityEngine;
using System.Collections;
//加载manifest
public class IABManifestLoader{
	public AssetBundleManifest assetManifest;
	public string manifestPath;//manifest路径
	private bool isLoadFinish;

	public AssetBundle manifestLoader;

	public IABManifestLoader(){
		assetManifest = null;
		manifestLoader = null;
		isLoadFinish = false;
		//file:///E:\Unity3d\Project\TestFrame\Assets\StreamingAssets\Windows\Windows,Windows平台下必有有个Windows.manifest
		manifestPath =  IPathTools.GetWWWAssetBundlePath()+"/"+IPathTools.GetPlatformFolderName(Application.platform);
	}
	//异步加载manifest
	public IEnumerator LoadManifest(){
		WWW manifest = new WWW(manifestPath);
		yield return manifest;
		if (!string.IsNullOrEmpty(manifest.error)){
			Debug.LogError(manifest.error);
		}else{
			if (manifest.progress >= 1.0f){
				manifestLoader = manifest.assetBundle;
				assetManifest = manifestLoader.LoadAsset("AssetBundleManifest") as AssetBundleManifest;//参数是固定的
				isLoadFinish = true;
			}
		}
	}
	//获取存储在manifest的依赖关系
	public string[] GetDepences(string name){
		return assetManifest.GetAllDependencies(name);
	}
	//卸载manifest
	public void UnloadManifest(){
		manifestLoader.Unload(true);
	}

	public void SetManifestPath(string path){
		manifestPath = path;
	}
	//单例
	private static IABManifestLoader instance = null;
	public static IABManifestLoader Instance{
		get{
			if (instance == null){
				instance = new IABManifestLoader();
			}
			return instance;
		}
	}

	public bool IsLoadFinish(){
		return isLoadFinish;	
	}

}
