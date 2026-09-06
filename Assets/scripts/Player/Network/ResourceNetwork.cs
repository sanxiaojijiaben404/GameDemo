using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class ResourceNetwork
{
    private ApiSettings apiSettings;
    //背包管理器引用，网络拿到服务器数据后交给Manager更新背包
    public ResourceNetwork(ApiSettings apiSettings)
    { this.apiSettings = apiSettings; }
    //协程：POST请求，上传新增资源信息到后端
    public IEnumerator SendResource(ResourceData resource, ResourceManager manager, System.Action<bool> callback
        )
    {
        //后端新增资源API地址
        string fullUrl = apiSettings.baseUrl + "/resource/add";
        //把C#实体类序列化为JSON文本，用于网络传输
        string json = JsonUtility.ToJson(resource);
        //构造网络请求：指定目标地址+请求方法
        UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
        //将JSON字符串转为UTF-8二进制字节数组，作为请求体
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        //开辟缓冲区接收服务器返回数据
        request.downloadHandler = new DownloadHandlerBuffer();
        //关键请求体，告诉后端本次请求Body的数据格式是JSON
        request.SetRequestHeader("Content-Type", "application/json");
        //异步网格的核心，暂停当前协程直到网络往返完成（不阻塞游戏进程）
        yield return request.SendWebRequest();
        //判断请求结果
        //POST
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("服务器保存失败：" + request.error);
            callback?.Invoke(false);
            yield break;
        }
        //重新拉取服务器最新资源
        bool getSuccess = false;
        yield return GetResource(manager, (success) => { getSuccess = success; });
        //结果
        if (getSuccess) { Debug.Log("服务器资源同步成功"); }
        else { Debug.LogError("服务器保存成功，但获取最新资源失败"); }
        callback?.Invoke(getSuccess);
    }
    //协程：GET请求，获取玩家全部背包资源
    public IEnumerator GetResource(ResourceManager manager, System.Action<bool> callback = null)
    {
        string fullUrl = apiSettings.baseUrl + "/resource/list";
        //快速创建GET请求
        UnityWebRequest request = UnityWebRequest.Get(fullUrl);
        yield return request.SendWebRequest();
        Debug.Log(request.result);
        //GET失败
        if (request.result != UnityWebRequest.Result.Success)
        {
            callback?.Invoke(false);
            yield break;
        }
        //GET成功
        //将后端返回的JSON反序列化为 ResourceListData 对象
        string json = request.downloadHandler.text;
        Debug.Log(
            "服务器返回：" + json
        );
        ResourceListData data = JsonUtility.FromJson<ResourceListData>(json);
        //把服务器下发的数据解析
        if (data == null || data.resources == null)
        {
            Debug.LogError("服务器资源解析失败");
            callback?.Invoke(false);
            yield break;
        }
        //更新客户端
        manager.LoadFromServer(data.resources);
        Debug.Log("客户端资源更新完成");
        callback?.Invoke(true);
    }
    public IEnumerator RemoveResource(ResourceData resource,ResourceManager manager,System.Action<bool> callback)
    {
        string fullUrl = apiSettings.baseUrl + "/resource/remove";
        string json = JsonUtility.ToJson(resource);
        UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("资源扣除失败：" + request.error);
            callback?.Invoke(false);
            yield break;
        }
        bool getSuccess = false;
        yield return GetResource(manager, (success) => { getSuccess = success; });
        callback?.Invoke(getSuccess);
    }
}