using UnityEngine;
using System.IO;
//本地存档持久化
//保存玩家完整状态
public static class SaveSystem
{
    private static string path =
       Path.Combine(
           Application.persistentDataPath,
           "player.json"
       );
    //静态保存方法，写入本地JSON存档
    public static void Save(PlayerSaveData data)
    {
        //将数据类转为格式化JSON文本，参数true=带换行缩进
        string json = JsonUtility.ToJson(data, true);
        Debug.Log(json);
        //拼接完整存档路径
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        //将字符串写入磁盘文件
        File.WriteAllText(path, json);
        Debug.Log("保存成功：" + path);
    }
    //静态读取方法，加载本地存档
    public static PlayerSaveData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        //判断存档文件是否存在
        if (!File.Exists(path))
        {
            Debug.Log("没有找到存档");
            return null;
        }
        //读取文件全部文本
        string json = File.ReadAllText(path);
        // JSON到C#对象反序列化
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        Debug.Log("读取成功，共"+data.resourses.Count+"种资源");
        return data;
    }
}
