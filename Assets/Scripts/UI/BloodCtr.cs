using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodCtr : MonoBehaviour {

    Image bloodImg;
	void Start () {
        bloodImg = gameObject.GetComponent<Image>();
	}
    //设置血量
    public void SetBlood(float v) {
        bloodImg.fillAmount = Mathf.Clamp01(v);
        //限制value在0,1之间并返回value。如果value小于0，返回0。如果value大于1,返回1，否则返回value 。 
    }
}
