using UnityEngine;
public class SaveManager : MonoBehaviour
{
    public ResourceSystemHost resourceSystem;
    public Transform player;
    private void Start()
    {
        LoadGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SaveGame();
        }
    }

    //保存游戏
    public void SaveGame()
    {
        PlayerSaveData data =new PlayerSaveData();
        //获取资源
        data.resources = resourceSystem.Manager.GetAllResource();
        //获取玩家位置
        data.playerPosition = new PlayerPosition();
        data.playerPosition.x = player.position.x;
        data.playerPosition.y= player.position.y;
        //暂时打印测试
        Debug.Log(

            "准备保存游戏，资源数量：" + data.resources.Count + ",玩家位置：(" + data.playerPosition.x + "," + data.playerPosition.y + ")");

        SaveNetwork saveNetwork = new SaveNetwork(resourceSystem.apiSettings);
        StartCoroutine(saveNetwork.SaveGame(data));
    }
    //读取游戏
    public void LoadGame()
    {
        
        SaveNetwork saveNetwork = new SaveNetwork(resourceSystem.apiSettings);
        StartCoroutine(saveNetwork.LoadGame(OnLoadGameSuccess));
    }
    private void OnLoadGameSuccess(PlayerSaveData data)
    {
        Debug.Log(
        "存档加载成功，资源数量："+ data.resources.Count+ "，玩家位置：("+ data.playerPosition.x+ ", "+ data.playerPosition.y+ ")" );
        player.position=new Vector3(data.playerPosition.x,data.playerPosition.y,player.position.z);
        Debug.Log("玩家位置恢复完成：（" + player.position.x + "," + player.position.y + ")");
    }

    
}
