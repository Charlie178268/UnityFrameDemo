using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 负责维护一个链式脚本消息队列
/// 脚本节点,存储各继承Mono的脚本
/// 负责脚本的存储和处理
/// </summary>
public class EventNode{
	public MonoBase data;
	public EventNode next;
	public EventNode(MonoBase monobase){
		data = monobase;
		next = null;
	}
}



public class ManagerBase:MonoBase{
	public Dictionary<ushort, EventNode> msgList = new Dictionary<ushort, EventNode>();//存储注册的各类型消息链表
	/// <summary>
	///给脚本注册消息，即将对应消息id的脚本挂在以消息id为区别的链表后面
	/// </summary>
	/// <param name="monoscript">Monoscript.</param>
	/// <param name="msgs">一个脚本可以包含多个消息</param>
	public void RegistMsg(MonoBase monoscript, params ushort[] msgids){//params表示可以传递任意数量的ushort参数，比直接传数组好用
		for (int i=0; i<msgids.Length; i++){
			EventNode node = new EventNode(monoscript);//生成脚本节点
			InsertNode(node, msgids[i]);//将脚本挂在相应消息类型的链表中，注意一个脚本会挂载到不同的id链表中，因此仅仅是引用的挂载，并不是内存的拷贝
		}
	}
	/// <summary>
	/// Inserts the node.
	/// </summary>
	/// <param name="node">节点</param>
	/// <param name="id">消息类型，即链表类型</param>
	void InsertNode(EventNode node, ushort id){
		if (!msgList.ContainsKey(id)){
			msgList.Add(id, node);
			return;
		}
		//如果存在这个消息类型的脚本，就插入
		EventNode temp = msgList[id];//找到头结点
		//找到最后一个
		while (temp.next != null){
			temp = temp.next;
		}
		temp.next = node;
	}
	/// <summary>
	/// 删除消息
	/// </summary>
	/// <param name="monoscript">Monoscript.</param>
	/// <param name="msgids">Msgids.</param>
	public void UnRegistMsg(MonoBase monoscript, params ushort[] msgids){
		for (int i=0; i<msgids.Length; i++){
			DeleteNode(monoscript, msgids[i]);
		}
	}
	void DeleteNode(MonoBase data, ushort id){
		if (!msgList.ContainsKey(id)){
			Debug.LogError("There is not contain id"+id);
			return;
		}

		EventNode temp = msgList[id];
		//如果该脚本是头部
		if (temp.data == data){
			//如果后面没有脚本了
			if (temp.next == null){
				msgList.Remove(id);
			}else{
				//去掉头结点
				temp.data = temp.next.data;
				temp.next = temp.next.next;
				//或者这样子释放
				//msgList[id] = temp.next;
				//temp.next = null;
			}
		}else{//如果该节点在中间或者尾部
			//找到该节点前面的节点
			while (temp.next!=null && temp.next.data != data){
				temp = temp.next;
			}
			EventNode del = temp.next;
			temp.next = temp.next.next;
			del.next = null;
		}
	}
	/// <summary>
	/// 来了消息，通知整个消息列表
	/// </summary>
	/// <param name="msgbase">Msgbase.</param>
	public override void ProgressEvent(MsgBase msgbase){
		if (!msgList.ContainsKey(msgbase.msgId)){
			Debug.LogError("msgs don't contain msgid:"+msgbase.msgId);
			Debug.LogError("msgs don't contain msgid:"+msgbase.GetManagerId());
			return;
		}else{
			EventNode tmp = msgList[msgbase.msgId];
			do{
				//策略模式
				tmp.data.ProgressEvent(msgbase);
				tmp = tmp.next;
			}while(tmp!=null);
		}
	}
}
