using UnityEngine;
using System.Collections;
using System.IO;

public class IPathTools{

	public static string GetPlatformFolderName(RuntimePlatform platform){
		switch(platform){
		case RuntimePlatform.Android:	return "Android"; break;
		case RuntimePlatform.IPhonePlayer: return "IOS"; break;
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor: return "Windows"; break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:	return "OSX"; break;
		default:return null;
		}
	} 

	public static string GetAppFilePath(){
		string tmpPath = "";
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform ==RuntimePlatform.WindowsPlayer){
			tmpPath = Application.streamingAssetsPath;
		}else{
			tmpPath = Application.persistentDataPath;
		}
		return tmpPath;
	}

	//获取AssetBundle的路径
	public static string GetAssetBundlePath(){
		string platFolder = GetPlatformFolderName(Application.platform);
		string allPath = Path.Combine(GetAppFilePath(), platFolder);
		return allPath;
	}

	//
	public static string GetWWWAssetBundlePath(){
		string tmpStr = "";
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor){
			tmpStr = "file:///"+ GetAssetBundlePath();
		}else{
			string tmpPath = GetAssetBundlePath();
#if UNITY_ANDROID
			tmpStr = "jar:file://"+tmpPath;
#elif UNITY_STANDALONE_WIN
			tmpStr = "file:///"+tmpPath;
#else
			tmpStr = "file://"+tmpPath;
#endif
		}
		return tmpStr;
	}
}
