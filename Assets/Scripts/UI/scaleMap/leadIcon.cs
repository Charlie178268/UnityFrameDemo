using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leadIcon : MonoBehaviour {

    Terrain terrain;
    GameObject player;
    RectTransform rectTransParent;//父级的布局
    RectTransform rectTransSelf;//自己的布局
    void Start () {

        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        player = GameObject.FindGameObjectWithTag("Player");
        rectTransParent = this.transform.parent.GetComponent<RectTransform>();
        rectTransSelf = this.transform.GetComponent<RectTransform>();
    }
	

	void Update () {
	    if (player)
        {
            //设置小图标的朝向和人物的朝向一致，Quaternion.Euler将欧拉角转换为四元数
            this.transform.rotation = Quaternion.Euler(0, 0, -player.transform.rotation.eulerAngles.y);
            //获得人物位置点在地图上的比例
            //terrain.terrainData.size.x获取地形的宽
            float scaleX = (float)player.transform.position.x / terrain.terrainData.size.x;
            float scaleZ = (float)player.transform.position.z / terrain.terrainData.size.z;

            scaleX = rectTransParent.rect.width*scaleX;
            scaleZ = rectTransParent.rect.height*scaleZ;
 
            rectTransSelf.localPosition = new Vector3(scaleX, scaleZ, 0);//设置坐标
        }
	}
}
