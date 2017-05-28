using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTime : MonoBehaviour {

    bool isPressed = false;
    float timeCount = 0;
    Image maskImage;

    void Start () {
        maskImage = this.transform.GetComponent<Image>();
    }
	
	void Update () {
        if (isPressed)
        {
            timeCount += Time.deltaTime;
            maskImage.fillAmount = timeCount / 3.0f;
            if (timeCount >= 3.0f)
            {
                timeCount = 0;
                isPressed = false;
                maskImage.fillAmount = 0;//让其隐藏
            }
        }
    }
    public void OnButtonDown()
    {
        if (timeCount <= 0)
        {
            maskImage.fillAmount = 100;
            isPressed = true;
        }
    }
}
