using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmaraFallow : MonoBehaviour
{

    //目标物体
    public Transform target;
    //跟随偏移
    public Vector3 offset = new Vector3(0, 0, -10);
    //平滑速度
    public float smoothSpeed = 5f;
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: 没有指定跟随目标 (Target)！");
            return;
        }

        // 强制将摄像机位置初始化为“目标位置 + 偏移”
        transform.position = target.position + offset;
    }
    void LateUpdate()
    {
        if (target == null) return;

        // 只计算 X 轴的目标位置，Y轴永远保持摄像机当前的 Y 值
        float targetX = target.position.x + offset.x;
        targetX = Mathf.Clamp(targetX, 0, 38);
        float fixedY = transform.position.y; 
        float fixedZ = offset.z;            

        Vector3 desiredPosition = new Vector3(targetX, fixedY, fixedZ);

        if (smoothSpeed <= 0)
            transform.position = desiredPosition;
        else
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
