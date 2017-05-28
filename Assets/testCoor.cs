using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testCoor : MonoBehaviour {

    void Start()
    {
        StartCoroutine(TestMove());/*开始执行协程*/
    }
    IEnumerator TestMove()
    {
        Debug.Log("TestMove");
        yield return new WaitForSeconds(2f); //延迟2秒执行
        Debug.Log("延迟两秒");
        for (int i = 0; i < 100; i++)
        {
            this.transform.Translate(0f, 0.1f, 0f);
            Debug.Log("协程");
            yield return null;/*表示这一帧挂起，下一帧继续执行，也就是让移动每帧执行一次*/
        }
        this.transform.position = new Vector3(-1.98f, 0.9f, -1.47f);
        Debug.Log("移动完成");
    }

}
