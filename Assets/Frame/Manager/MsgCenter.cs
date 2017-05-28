using UnityEngine;
using System.Collections;

/// <summary>
/// 初始化各manager
/// 向各个Manager模块转发消息
/// 框架只用挂MsgCenter脚本，并保证其先被初始化(在exexction order里面设置default time为负的就行)
/// </summary>
public class MsgCenter : MonoBehaviour {
	public static MsgCenter Instance = null;
	void Awake(){
        //必须先初始化Manager，也就是单例的初始化
        Instance = this;
		gameObject.AddComponent<UIManager>();
		gameObject.AddComponent<NPCManager>();
		gameObject.AddComponent<AssetManager>();
		gameObject.AddComponent<ILoaderManager>();
        gameObject.AddComponent<NetManager>();
        gameObject.AddComponent<CharacterManager>();
    }
	//转发消息
	public void SendToMsg(MsgBase msgbase){
		AnasysisMsg(msgbase);
	}
	//未完
	void AnasysisMsg(MsgBase msgbase){
		ManagerId tmpId = msgbase.GetManagerId();
		switch(tmpId){
		case ManagerId.AssetManager:
                AssetManager.Instance.SendMsg(msgbase);
                break;
		case ManagerId.AudioManager:
			break;
		case ManagerId.CharacterManager:
                CharacterManager.Instance.SendMsg(msgbase);
                break;
		case ManagerId.GameManager:
			break;
		case ManagerId.LuaManager:
			break;
		case ManagerId.NetManager:
                NetManager.Instance.SendMsg(msgbase);
			break;
		case ManagerId.NpcManager:
                NPCManager.Instance.SendMsg(msgbase);
                break;
		case ManagerId.UIManager:
                UIManager.Instance.SendMsg(msgbase);
                break;
		default:
			break;
		}
	}
}
