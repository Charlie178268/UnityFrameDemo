using UnityEngine;
using System.Collections;

/// <summary>
/// 继承Mono的脚本
/// 作为所有挂在物体上脚本的父类，方便后面扩展
/// </summary>
public abstract class MonoBase : MonoBehaviour {
	public abstract void ProgressEvent(MsgBase msgbase);


}
