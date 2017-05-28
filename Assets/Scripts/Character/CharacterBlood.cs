using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//数据层
public class CharacterBlood{
    float blood = 100.0f;
    public void LooseBlood(float tmpBlood) {
        blood -= tmpBlood;
    }
    public float GetBlood() {
        return blood / 100.0f;
    }
}
