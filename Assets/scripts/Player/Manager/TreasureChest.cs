using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TreasureChest : MonoBehaviour
{
    //宝箱奖励列表
    public List<ChestReward> rewards = new List<ChestReward>();
    //资源管理器
    public ResourceSystemHost resourceSystem;
    private ResourceManager manager;
    //世界状态管理器
    public WorldStateManager worldStateManager;
    // 是否正在开启
    private bool opening = false;
    //是否已经开启
    private bool opened = false;
    [Header("宝箱唯一ID")]
    public string chestId;
    //音效
    public AudioClip openChestClip;
    private AudioSource AudioSource;
    private IEnumerator Start()
    {
        if(resourceSystem!=null)
        {
            manager = resourceSystem.Manager;
        }
        if(worldStateManager!=null)
        {
            yield return new WaitUntil(() => worldStateManager.IsInitialized);
            if(worldStateManager.IsChestOpened(chestId))
            {
                opened = true;
                gameObject.SetActive(false);
                Debug.Log("宝箱已经打开过：" + chestId);
            }
        }
    }
   private void Awake()
    {
        //获取自身AudioSource组件
        AudioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        Debug.Log("碰到宝箱：" + other.name);
        OpenChest();
    }
    void OpenChest()
    {
        // 已经打开或者正在开启
        if (opened || opening)
        {
            return;
        }
        if (manager == null)
        {
            Debug.LogError("manager为空");
            return;
        }
        if (rewards == null || rewards.Count == 0)
        {
            Debug.LogWarning("宝箱没有奖励：" + chestId);
            return;
        }
        // 开始开启流程
        opening = true;
        //播放音效
        if (openChestClip != null && AudioSource != null)
        {
            AudioSource.PlayOneShot(openChestClip);
        }
        // 一个一个发送奖励
        StartCoroutine(SendRewardsSequentially());
    }
    private IEnumerator SendRewardsSequentially()
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            ChestReward reward = rewards[i];
            bool requestFinished = false;
            bool requestSuccess = false;
            Debug.Log( "正在发送宝箱奖励：" +reward.resourceID + " +" + reward.amount);
            manager.RequestAddResource(reward.resourceID, reward.amount,success =>{requestSuccess = success;requestFinished = true; } );
            // 等待当前奖励请求完成
            yield return new WaitUntil(() => requestFinished );
            // 当前奖励失败
            if (!requestSuccess)
            {
                Debug.LogError( "宝箱奖励发送失败：" + reward.resourceID);
                opening = false;
                yield break;
            }
            Debug.Log( "宝箱奖励发送成功：" +reward.resourceID);
        }
        // 所有奖励全部发送成功
        Debug.Log("宝箱全部奖励发送成功");
        // 最后统一获取一次服务器最新数据
        bool loadFinished = false;
        bool loadSuccess = false;
        StartCoroutine(
            resourceSystem.Network.GetResource(
                manager,
                success =>{loadSuccess = success;loadFinished = true;}
            )
        );
        yield return new WaitUntil(
            () => loadFinished
        );
        if (!loadSuccess)
        {
            Debug.LogError(
                "宝箱奖励已经写入服务器，但刷新客户端背包失败"
            );
            opening = false;
            yield break;
        }
        // 认为宝箱开启成功
        opened = true;
        opening = false;
        // 记录世界状态
        if (worldStateManager != null)
        {
            worldStateManager.RegisterOpenedChest(chestId);
        }
        // 显示获得奖励
        ResourceGetTip.Instance?.Show(
            rewards,
            manager
        );
        Debug.Log(
            "宝箱开启完成：" + chestId
        );
        // 播放完音效后销毁
        float destroyDelay =openChestClip != null ? openChestClip.length : 0.1f;
        Destroy(gameObject, destroyDelay);
    }
}

