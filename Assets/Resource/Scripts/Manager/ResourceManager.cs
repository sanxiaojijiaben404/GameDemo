using System.Collections.Generic;
using UnityEngine;
public class ResourseManager : MonoBehaviour
{   //玩家拥有的资源
    public List<ResourseData> resourses = new List<ResourseData>();
    public int maxSlot = 10;
    //资源数据库：存放所有资源静态配置
    public ResourceDatabase database;
    //服务器通信：负责客户端和服务端资源数据同步
    public ResourceNetwork network;
    //判断背包是否满
    public bool IsFull()
    {
        return resourses.Count >= maxSlot;
    }
    //增加资源
    public void AddResource(int id, int count)
    {
        Debug.Log(
            "AddResource调用 id:"
            + id
            + " count:"
            + count
        );
        //在背包列表中查找已有该ID资源
        ResourseData resource = resourses.Find(
            r => r.id == id
        );
        //背包里没有则新建一条资源数据
        if (resource == null)
        {
            Debug.Log("没有找到资源，创建新资源");
            resource = new ResourseData();
            resource.id = id;
            resource.name = "未知资源";
            resource.count = 0;
            resourses.Add(resource);
        }
        Debug.Log(
            "增加前:"
            + resource.name
            + " 数量:"
            + resource.count
        );
        //增加数量
        resource.count += count;
        Debug.Log(
            "增加后:"
            + resource.name
            + " 数量:"
            + resource.count
        );
        //向服务器同步本次新增资源
        if (network != null)
        {
            ResourseData data = new ResourseData();
            data.id = resource.id;
            data.name = resource.name;
            data.count = count;
            network.AddResourceToServer(data);
        }
    }
    //减少资源，返回bool代表操作是否成功
    public bool RemoveResource(int id, int count)
    {
        ResourseData resourse = resourses.Find(r => r.id == id);
        //背包不存在该资源
        if (resourse == null)
        { return false; }
        //当前数量不足
        if (resourse.count < count)
        {
            return false;
        }
        //校验全部通过
        resourse.count -= count;
        Debug.Log(
            resourse.name
            +
            "-"
            +
            count
        );
        return true;
    }
    //查询指定ID资源当前数量
    public int GetResourceCount(int id)
    {
        ResourseData resourse = resourses.Find(
            r => r.id == id);
        if (resourse != null)
        {
            return resourse.count;
        }
        //找不到资源默认返回0
        return 0;
    }
    //判断资源是否够用
    public bool HasEnough(int id, int needCount)
    {
        return GetResourceCount(id) >= needCount;
    }
    //读取背包内全部资源
    public List<ResourseData> GetAllResource()
    {
        return resourses;
    }
    //本地存档：保存玩家数据
    public void Save()
    {
        PlayerSaveData saveData = new PlayerSaveData();
        //保存背包
        saveData.resourses = resourses;
        //暂时测试数据
        saveData.hp = 3;
        //当前场景
        saveData.sceneName=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        //任务进度
        saveData.taskId = 0;
        SaveSystem.Save(saveData);
        Debug.Log("玩家存档完成");
    }
    //读取本地存档
    public void Load()
    {
        PlayerSaveData data = SaveSystem.Load();
        //不存在直接退出
        if (data == null)
        {
            return;
        }
        resourses = data.resourses;
        Debug.Log("资源读取完成");
        foreach(ResourseData resourse in resourses)
        {
            Debug.Log(resourse.name + ": " + resourse.count);
        }
    }
    //Unity生命周期：游戏关闭时自动执行存档
    private void OnApplicationQuit()
    {
        Save();
    }
    //打开游戏自动读档
    private void Start()
    {
        //Load();等待服务器加载，优先拉服务器
    }
    //从服务器加载资源，覆盖本地背包
    public void LoadFromServer(List<ResourseData> serverResources)
    {
        resourses = serverResources;
        //根据数据库刷新资源名称
        foreach (ResourseData resource in resourses)
        {
            ResourceConfig config =
                database.GetResource(resource.id);
            if (config != null)
            {
                resource.name = config.itemName;
                resource.type = config.type;
            }
        }
        Debug.Log("服务器资源加载完成");
    }
}