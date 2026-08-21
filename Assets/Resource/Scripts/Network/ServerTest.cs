using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
public class ServerTest : MonoBehaviour
{
    //接口地址变量
    string url = "http://192.168.247.134:8080/resource/list";
    void Start()
    {
        //开启协程
        StartCoroutine(GetResource());
    }
    IEnumerator GetResource()
    {
        //创建GET请求对象
        UnityWebRequest request = UnityWebRequest.Get(url);
        //发送网络请求，yeild暂停协程，等待请求完成
        yield return request.SendWebRequest();
        //判断请求结果
        if(request.result == UnityWebRequest.Result.Success)
        {
            //成功，打印后端返回的文本
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            //失败，打印错误信息
            Debug.LogError(request.error);
        }
    }

}
