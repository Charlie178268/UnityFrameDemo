using UnityEngine;
using System.Collections;


public class NetBase : MonoBase
{
    public void RegisSelf(MonoBase mono, params ushort[] msgs)
    {
        NetManager.Instance.RegistMsg(mono, msgs);
    }
    public void UnRegisSelf(MonoBase mono, params ushort[] msgs)
    {
        NetManager.Instance.UnRegistMsg(mono, msgs);
    }
    public void SendMsg(MsgBase msg)
    {
        NetManager.Instance.SendMsg(msg);
    }
    public ushort[] msgIds;
    //自动销毁本身
    void OnDestroy()
    {
        if (msgIds != null)
        {
            UnRegisSelf(this, msgIds);
        }
    }
    public override void ProgressEvent(MsgBase msgbase)
    {

    }
}
