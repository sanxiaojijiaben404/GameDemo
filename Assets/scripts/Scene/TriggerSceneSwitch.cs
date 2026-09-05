using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class TriggerSceneSwitch : MonoBehaviour
{
    [Tooltip("要跳转的场景索引号，交给CGControl，等CG播完再加载")]
    public int index;

    [Tooltip("把层级里的CG_Canvas拖进来")]
    public CGControl cgControl;

    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_triggered) return;
        if (collider.CompareTag("Player"))
        {
            if (cgControl == null)
            {
                Debug.LogError("请在本物体Inspector拖入CG_Canvas！");
                return;
            }

            _triggered = true;
            // 设置CG结束后要跳转的场景索引
            cgControl.endingTargetSceneIndex = index;
            // 只启动结局CG，加载场景由CGControl的OnEndingFinish回调处理
            cgControl.PlayEnding();
        }
    }
}
