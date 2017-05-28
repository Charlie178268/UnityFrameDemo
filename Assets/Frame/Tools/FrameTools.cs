using UnityEngine;
using System.Collections;

public enum ManagerId{
	GameManager = 0,
    /*游戏流程的管理，进度的控制
	1.资源的拷贝
    2.初始化操作
    脚本：
    1.初始化模块
    2.消息处理，功能逻辑
    3.Reset()，比如npc的重复利用
    4.程序退出
    */
    UIManager = FrameTools.msgSpan,
	CharacterManager = FrameTools.msgSpan*2,
	NpcManager = FrameTools.msgSpan*3,
    /*
     * 如果一个模块的逻辑太复杂，可以分为多个base，比如：
     animaltor:NPCBase
     control:NPCBase
     data:NPCBase
     */
	AudioManager = FrameTools.msgSpan*4,
	LuaManager = FrameTools.msgSpan*5,
	AssetManager = FrameTools.msgSpan*6,
	NetManager = FrameTools.msgSpan*7
}

public class FrameTools{
	public const int msgSpan = 3000;//一种消息的所有条数
}
