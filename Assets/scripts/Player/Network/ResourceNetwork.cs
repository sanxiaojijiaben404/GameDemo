using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class ResourceNetwork
{
    private ApiSettings apiSettings;
    public ResourceNetwork(ApiSettings apiSettings)
    {
        this.apiSettings = apiSettings;
    }
    // 增加资源
    public IEnumerator SendResource( ResourceData resource, ResourceManager manager,System.Action<bool> callback)
    {
        string fullUrl = apiSettings.baseUrl +"/resource/add?slot=" + SaveSlotManager.CurrentSlot;
        string json = JsonUtility.ToJson(resource);
        UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("资源增加失败：" + request.error + "，HTTP状态：" + request.responseCode + "，服务器返回：" + request.downloadHandler.text);
            callback?.Invoke(false);
            yield break;
        }
        Debug.Log("服务器增加资源成功：" + request.downloadHandler.text);
        callback?.Invoke(true);
    }
    // 获取当前存档资源
    public IEnumerator GetResource( ResourceManager manager, System.Action<bool> callback = null)
    {
        string fullUrl =apiSettings.baseUrl + "/resource/list?slot=" + SaveSlotManager.CurrentSlot;
        UnityWebRequest request = UnityWebRequest.Get(fullUrl);
        yield return request.SendWebRequest();
        Debug.Log(request.result);
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("获取服务器资源失败：" + request.error);
            callback?.Invoke(false);
            yield break;
        }
        string json = request.downloadHandler.text;
        Debug.Log("服务器返回：" + json);
        ResourceListData data =JsonUtility.FromJson<ResourceListData>(json);
        if (data == null || data.resources == null)
        {
            Debug.LogError("服务器资源解析失败");
            callback?.Invoke(false);
            yield break;
        }
        manager.LoadFromServer(data.resources);
        Debug.Log("客户端资源更新完成");
        callback?.Invoke(true);
    }
    // 扣除资源
    public IEnumerator RemoveResource(ResourceData resource,ResourceManager manager, System.Action<bool> callback)
    {
        string fullUrl = apiSettings.baseUrl +"/resource/remove?slot=" + SaveSlotManager.CurrentSlot;
        string json = JsonUtility.ToJson(resource);
        UnityWebRequest request = new UnityWebRequest(fullUrl, "POST");
        byte[] body =System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler =new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader( "Content-Type","application/json" );
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError( "资源扣除失败：" + request.error +"，服务器返回：" +request.downloadHandler.text);
            callback?.Invoke(false);
            yield break;
        }
        Debug.Log("服务器扣除资源成功：" +request.downloadHandler.text
        );
        // 扣除成功后重新获取服务器最新资源
        bool getSuccess = false;
        yield return GetResource( manager,success =>{ getSuccess = success;});
        callback?.Invoke(getSuccess);
    }
}