using UnityEngine;
using System.Collections.Generic;
public class TreasureChest : MonoBehaviour
{
    //宝箱奖励列表
    public List<ChestReward> rewards = new List<ChestReward>();
    //资源管理器
    public ResourseManager manager;
    //是否已经开启
    private bool opened = false;
    //音效
    public AudioClip openChestClip;
    private AudioSource AudioSource;
    void Awake()
    {
        //获取自身AudioSource组件
        AudioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("碰到宝箱:" + other.name);
        if (other.CompareTag("Player"))
        {
            OpenChest();
        }
    }
    void OpenChest()
    {
        if(opened) return;
        opened = true;
        //播放音效
        if(openChestClip != null && AudioSource != null)
        {
            AudioSource.PlayOneShot(openChestClip);
        }
        foreach(ChestReward reward in rewards)
        {
            manager.AddResource(reward.resourceID,reward.amount);
        }
        Debug.Log("宝箱开启");
        Destroy(gameObject,openChestClip != null ? openChestClip.length : 0.1f);
    }
}

