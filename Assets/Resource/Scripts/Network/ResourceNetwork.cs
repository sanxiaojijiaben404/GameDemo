using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class ResourceNetwork : MonoBehaviour
{
    public ApiSettings apiSettings;
    //背包管理器引用，网络拿到服务器数据后交给Manager更新背包
    public ResourseManager manager;
    void Start()
    {
        //脚本加载启动时，自动向服务器拉取全部资源数据
        LoadResourceFromServer();
    }
    //对外暴露方法，向服务器提交新增资源
    public void AddResourceToServer(ResourseData resource)
    {
        //启动协程，UnityWebRequest网络请求必须用协程异步执行，不能直接在普通方法同步执行
        StartCoroutine(SendResource(resource));
    }
    //对外暴露方法，从服务器加载完整背包资源列表
    public void LoadResourceFromServer()
    {
        StartCoroutine(GetResource());
    }
    //协程：POST请求，上传新增资源信息到后端
    IEnumerator SendResource(ResourseData resource)
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
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("服务器保存成功：" + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
    //协程：GET请求，获取玩家全部背包资源
    IEnumerator GetResource()
    {
        string fullUrl =apiSettings.baseUrl+ "/resource/list";
        //快速创建GET请求
        UnityWebRequest request = UnityWebRequest.Get(fullUrl);
        yield return request.SendWebRequest();
        Debug.Log(request.result);
        Debug.Log(request.downloadHandler.text);
        //将后端返回的JSON反序列化为 ResourceListData 对象
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log(
                "服务器资源数据：" + json
            );
            ResourceListData data = JsonUtility.FromJson<ResourceListData>(json);
            //把服务器下发的数据传递给背包管理器，覆盖本地背包
            if (data != null)
            {
                Debug.Log("资源数量：" + data.resources.Count);
                if (manager != null)
                {
                    manager.LoadFromServer(data.resources);
                }
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}