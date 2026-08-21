using System;
using System.Collections.Generic;
[Serializable]
public class PlayerSaveData
{
    //玩家生命
    public int hp;
    //当前场景
    public string sceneName;
    //游戏任务进度
    public int taskId;
    //背包资源
    public List<ResourseData> resourses;
}
