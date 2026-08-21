using UnityEngine;
using System.Collections.Generic;
public class SaveManager : MonoBehaviour
{
    public ResourseManager resourceManager;
    //玩家当前血量
    public int hp = 1;
    //当前任务
    public int taskId = 0;
    //保存游戏
    public void SaveGame()
    {
        PlayerSaveData data =new PlayerSaveData();
        //保存资源
        data.resourses = new List<ResourseData>(    resourceManager.GetAllResource());
        //保存血量
        data.hp = hp;
        //保存当前场景
        data.sceneName =UnityEngine.SceneManagement .SceneManager .GetActiveScene().name;
        //保存任务
        data.taskId = taskId;
        SaveSystem.Save(data);
        Debug.Log("游戏保存完成");
    }
    //读取游戏
    public void LoadGame()
    {
        PlayerSaveData data =SaveSystem.Load();
        if (data == null)
        {
            return;
        }
        //恢复资源
        resourceManager .LoadFromServer(data.resourses);
        hp = data.hp;
        taskId = data.taskId;
        Debug.Log(
            "游戏读取完成"
        );
    }
}
