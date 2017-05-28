using UnityEngine;
using System.Collections;

//消息的基类
public class MsgBase{
	//65535个消息
	public ushort msgId;
	//由消息的id获得消息的类型
	public ManagerId GetManagerId(){
		int tmpId = msgId/FrameTools.msgSpan;
		return (ManagerId)(tmpId*FrameTools.msgSpan);
	}
    //为了使消息可以重复利用
    public void ChangeMsgId(ushort tmpId)
    {
        this.msgId = tmpId;
    } 
    //默认构造函数
	public MsgBase(){
		msgId = 0;
	}
	//构造函数，传递消息的id
	public MsgBase(ushort msg){
		msgId = msg;
	}

}

//自定义消息类型，带参数的消息
public class ParamsMsg:MsgBase{
	public Transform trans;
	//base是调用基类的构造函数的意思，0是参数
	public ParamsMsg():base(0){

	}
	public ParamsMsg(ushort Id, Transform trans):base(Id){
		this.msgId = Id;
		this.trans = trans;
	}
}
//定义一些常用的消息类型
public class MsgFloat : MsgBase
{
    public float v;
    public MsgFloat(ushort msgId, float tmpV)
    {
        this.msgId = msgId;
        this.v = tmpV;
    }
}

public class MsgInt : MsgBase
{
    public int v;
    public MsgInt(ushort msgId, int tmpV)
    {
        this.msgId = msgId;
        this.v = tmpV;
    }
}