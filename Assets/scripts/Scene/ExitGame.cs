using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ExitGame : MonoBehaviour
{
    public ResourceSystemHost resourceSystem;
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 在编辑器模式：退出播放模式
        EditorApplication.isPlaying = false;
#else
        //打包exe之后：真正关闭游戏窗口
        Application.Quit();
        esourceSystem.Manager.ClearResources();
#endif
    }
}
