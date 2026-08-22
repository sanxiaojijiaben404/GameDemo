using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BgControl : MonoBehaviour
{
    // 云层自身的移动速度（正数向左）
    public float scrollSpeed = -0.3f;

    // 云层跟随相机的速度系数（负数，产生远景相对移动）
    public float followSpeed = 1f;

    // 云层的宽度（用于循环复位）
    public float cloudWidth = 19.2f;

    // 云层初始 X 偏移
    public float startOffset = 0f;

    private float cycleOffset = 0f;

    void Start()
    {
        cycleOffset = startOffset;
    }

    void Update()
    {
        //自身滚动：持续向左累积偏移
        cycleOffset += scrollSpeed * Time.deltaTime;

       //循环复位：超过宽度时拉回
        if (cycleOffset > cloudWidth)
        {
            cycleOffset -= cloudWidth;
        }
        else if (cycleOffset < 0)
        {
            cycleOffset += cloudWidth;
        }

        // 3. 获取主相机 X 位置
        float cameraX = Camera.main.transform.position.x;

        // 4. 云的最终 X = 相机 X × 跟随系数 + 自身滚动偏移
        float cloudX = cameraX * followSpeed -cycleOffset;

        // 5. 设置云的位置
        transform.position = new Vector3(cloudX, transform.position.y, transform.position.z);
    }
}
