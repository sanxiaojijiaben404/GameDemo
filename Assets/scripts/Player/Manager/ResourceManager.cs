using System.Collections.Generic;
using UnityEngine;
public class ResourceManager
{   //玩家拥有的资源
    public List<ResourceData> resources = new List<ResourceData>();
    public int maxSlot = 10;
    //资源数据库：存放所有资源静态配置
    public ResourceDatabase database;
    //服务器通信：负责客户端和服务端资源数据同步
    public bool IsInitialized { get; private set;  }
    public System.Action OnResourceChanged;
    private System.Action<ResourceData,System.Action<bool>> syncToServer;
    private System.Action<ResourceData, System.Action<bool>> syncRemoveToServer;
    //构造函数
    public ResourceManager(ResourceDatabase database, System.Action<ResourceData,System.Action<bool>> syncToServer,System.Action<ResourceData,System.Action<bool>> syncRemoveToServer)
    {
        this.database = database;
        this.syncToServer = syncToServer;
        this.syncRemoveToServer = syncRemoveToServer;
    }
    //判断背包是否满
    public bool IsFull()
    {
        return resources.Count >= maxSlot;
    }
    //请求服务器增加资源
    public void RequestAddResource(int id,int count,System.Action<bool> calllback)
    {
        ResourceData data = new ResourceData();
        data.id= id;
        data.count = count;
        //通知发送请求
        syncToServer?.Invoke(data,calllback);
    }
    //请求服务器减少资源
    public void RequestRemoveResource(int id,int count,System.Action<bool> callback)
    {
        if(!HasEnough(id,count))
        {
            Debug.Log("资源不足");
            callback?.Invoke(false);
            return;
        }
        ResourceData data = new ResourceData();
        data.id= id;
        data.count= count;
        syncRemoveToServer?.Invoke(data,callback);
    }
    //减少资源(备用）
    public bool RemoveResource(int id, int count)
    {
        ResourceData resourse = resources.Find(r => r.id == id);
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
        ResourceData resourse = resources.Find(
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
    public List<ResourceData> GetAllResource()
    {
        return resources;
    }
    //从服务器加载资源，覆盖本地背包
    public void LoadFromServer(List<ResourceData> serverResources)
    {
        resources = serverResources;
        IsInitialized = true;
        Debug.Log("服务器资源加载完成");
        //根据数据库刷新资源名称
        foreach (ResourceData resource in resources)
        {
            ResourceConfig config =
                database.GetResource(resource.id);
            if (config != null)
            {
                resource.name = config.itemName;
            }
        }
        OnResourceChanged?.Invoke();
    }
    //服务器返回最新资源后更新客户端
    public void UpdateFromServer(List<ResourceData> serverResources)
    {
        Debug.Log(
       "进入UpdateFromServer"
        );
        resources = serverResources;
        //debug
        Debug.Log("客户端当前资源数量:" + resources.Count);
        foreach (ResourceData resource in resources)
        {
            Debug.Log(
                resource.name
                +
                ":"
                +
                resource.count
            );
        }
        //
        foreach (ResourceData resource in resources)
        {
            ResourceConfig config =
                database.GetResource(resource.id);
            if (config != null)
            {
                resource.name = config.itemName;
            }
        }
        Debug.Log("客户端资源更新完成");
    }
    public void LoadFromSave(List<ResourceData> saveResources)
    {
        resources =new List<ResourceData>(saveResources);
        Debug.Log(
        "本地存档资源加载完成，资源数量："
        + resources.Count
        );
        //根据资源数据库刷新资源名称
        foreach (ResourceData resource in resources)
        {
            ResourceConfig config = database.GetResource(resource.id);
            if (config != null)
            {
                resource.name = config.itemName;
            }
        }
    }
    //结束清空
    public void ClearResources()
    {
        resources.Clear();
    }
}