using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

//自定义两个菜单选项
//一个是打包到指定文件夹下
//另外一个是标记资源bundle名为场景名/资源名，比如SceneOne/load,并记录在文本文件中
public class AssetBundleEditor {
	[MenuItem("Itools/BuildAssetBundle")]//自定义扩展编辑器，必须把此脚本放在Editor目录下，作为扩展编辑器的脚本
	//打包assetBundle到输出目录
	public static void BuildAssetBundle(){
		string outpath = IPathTools.GetAssetBundlePath();//输出到文件夹streamingassetpath / windows
		Debug.Log(outpath);
		BuildPipeline.BuildAssetBundles(outpath, 0, EditorUserBuildSettings.activeBuildTarget);//打包成assetbundle
	}
	//标记所有的assetbundle为
	[MenuItem("Itools/MarkAssetBundle")]
	public static void MarkAssetBundle(){
		AssetDatabase.RemoveUnusedAssetBundleNames();//移除不用的标记名
		string path = Application.dataPath+ "/Art/Scenes/";//打包的路径
		Debug.Log ("path: "+ path);
		//path: E:/Unity3d/Project/TestFrame/Assets/Art/Scenes/
		DirectoryInfo dir = new DirectoryInfo(path);
		FileSystemInfo[] fileInfor = dir.GetFileSystemInfos();
		for (int i=0; i<fileInfor.Length; i++){
			FileSystemInfo tmpFile = fileInfor[i];
			if (tmpFile is DirectoryInfo){
				string tmpPath = Path.Combine(path, tmpFile.Name);
				Debug.Log("tmpPath: "+tmpPath);
				//tmpPath: E:/Unity3d/Project/TestFrame/Assets/Art/Scenes/SceneOne
				SceneOverView(tmpPath);
			}
		}

		string outpath = IPathTools.GetAssetBundlePath();
		CopyRecord(path, outpath);//复制record.txt到assetstreaming文件夹下相应目录

		AssetDatabase.Refresh();//刷新Asset资源管理器
	}

	public static void CopyRecord(string srcPath, string disPath){
		DirectoryInfo dir = new DirectoryInfo(srcPath);
		if (!dir.Exists){
			Debug.Log("is not exsit");
			return;
		}
		if (!Directory.Exists(disPath)){
			Directory.CreateDirectory(disPath);
		}

		FileSystemInfo[] files = dir.GetFileSystemInfos();
		for (int i=0; i<files.Length; i++){
			FileInfo file = files[i] as FileInfo;
			//对于文件的操作
			if (file!=null && file.Extension == ".txt"){
				string sourceFile = srcPath+file.Name;
				string disFile = disPath + "/" + file.Name;
				Debug.Log("sourceFile==="+sourceFile);
				Debug.Log("disFile==="+ disFile);
				File.Copy(sourceFile, disFile, true);
			}
		}
	}

	//遍历场景，创建Record.txt,将同一assetbundle下的文件都记录下来(不分文件夹)
	public static void SceneOverView(string scenePath){
		string path = scenePath+ "Record.txt";
		//注意记录文件位置，最后面没有斜杠，路径和名字是连在一起的：E:/Unity3d/Project/TestFrame/Assets/Art/Scenes/SceneTwoRecord.txt
		Debug.Log("记录文件位置："+path);
		FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
		StreamWriter sw = new StreamWriter(fs);
		//存储对应关系
		Dictionary<string, string> dic = new Dictionary<string, string>();
		ChangedHead(scenePath, dic);
 	
		//将dic里面存储的写入文件
		sw.WriteLine(dic.Count);//写入总行数
		foreach (string key in dic.Keys){
			sw.Write(key);
			sw.Write(" ");
			sw.Write(dic[key]);
			sw.Write("\n");
		}

		sw.Close();
		fs.Close();
	}

	public static void ChangedHead(string fullPath, Dictionary<string, string> dic){
		//得到//Assets/Art/Scenes/SceneOne
		int tmpCount = fullPath.IndexOf("Assets");
		int tmpLength = fullPath.Length;
		string replacePath = fullPath.Substring(tmpCount, tmpLength-tmpCount);
		DirectoryInfo dir = new DirectoryInfo(fullPath);
		if (dir != null){
			ListFiles(dir, replacePath, dic);
		}else{
			Debug.Log("this path is not exsit!");
		}
	}

	//遍历场景中每一个功能文件夹
	public static void ListFiles(FileSystemInfo info, string replacePath, Dictionary<string, string> dic){
		if (!info.Exists){
			Debug.Log("fileinfo not exits");
			return;
		}
		DirectoryInfo dir = (DirectoryInfo)info;
		FileSystemInfo[] files = dir.GetFileSystemInfos();
		for (int i=0; i<files.Length; i++){
			FileInfo file = files[i] as FileInfo;
			//对于文件的操作
			if (file != null){
				ChangeMark(file, replacePath, dic);
			}else{//对于目录的操作
				ListFiles(files[i], replacePath, dic);
			}
		}
	}

	public static string ReplaceWindowsPath(string path){
		path = path.Replace("\\", "/");//转义,将"\"替换成"/"
		return path;
	}
	//得出mark标记值,path为//Assets/Art/Scenes/SceneOne
	public static string GetBundlePath(FileInfo info, string path){
		string tmpPath = info.FullName;
		//得到完全的路径，unity中得到的路径是"/",此api得到的是"\"，所以要对路径替换一下字符
		Debug.Log("windowsPath=:"+ tmpPath);//		windowsPath=:E:\Unity3d\Project\TestFrame\Assets\Art\Scenes\SceneOne\Regist\test.prefab
		tmpPath = ReplaceWindowsPath(tmpPath);
		int assetCount = tmpPath.IndexOf(path);
		assetCount += path.Length+1;
		int nameCount = tmpPath.LastIndexOf(info.Name);
		int tmpCount = path.LastIndexOf("/");
		string sceneHead = path.Substring(tmpCount+1, path.Length-tmpCount-1);

		int tmpLength = nameCount-assetCount;
		if (tmpLength > 0){
			string subString = tmpPath.Substring(assetCount, tmpPath.Length-assetCount);
			string[] result = subString.Split("/".ToCharArray());
			return sceneHead + "/" + result[0];
		}else{
			return sceneHead;
		}
	}

	public static void ChangeMark(FileInfo tmpFile, string replacePath, Dictionary<string, string> dic){
		if (tmpFile.Extension == ".meta"){
			return;
		}
		string markStr = GetBundlePath(tmpFile, replacePath);
		Debug.Log("markStr=: "+markStr);
		ChangeAssetMark(tmpFile, markStr, dic);
	}

	public static void ChangeAssetMark(FileInfo tmpfile, string markstr, Dictionary<string, string> theWriter){
		string fullPath = tmpfile.FullName;
		int assetCount = fullPath.IndexOf("Assets");
		string assetPath = fullPath.Substring(assetCount, fullPath.Length-assetCount);
		AssetImporter importer = AssetImporter.GetAtPath(assetPath);
			//AssetImporter作为资源导入器的基类，GetAtPath用于获取资源路径
		//改变标记
		importer.assetBundleName = markstr;
		if (tmpfile.Extension == ".unity"){
			importer.assetBundleVariant = "u3d";//设置标记的后缀
		}else{
			importer.assetBundleVariant = "ld";
		}
		//添加到dic
		string modleName = "";
		string[] subMark = markstr.Split("/".ToCharArray());
		if (subMark.Length > 1){
			modleName =subMark[1];
		}else{
			modleName = markstr;
		}

		string modelPath = markstr.ToLower()+"."+importer.assetBundleVariant;
		if (!theWriter.ContainsKey(modleName)){
			theWriter.Add(modleName, modelPath);
		}
	}
}
