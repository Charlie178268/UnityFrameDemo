using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机跟随物体
/// lateupdate中更新相机的位置
/// 
/// </summary>
public class FollowMonster : MonoBehaviour {

    private Transform target;

    public float smoothTime = 0.01f;

    private Vector3 cameraVelocity = Vector3.zero;

    public Vector3 Distance;

	void Start () {
        target = GameObject.FindWithTag("Player").transform;
	}
	//最后一次更新，以防相机抖动
	void LateUpdate () {
        Distance = new Vector3(0, 4f, -4f);
        if (target)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position + Distance, ref cameraVelocity, smoothTime);
        }
	}
}
