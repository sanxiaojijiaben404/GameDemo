using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    //已经打开的宝箱
    private HashSet<string> openedChestIds = new HashSet<string>();
    //已经击败的敌人
    private HashSet<string> defeatedEnemyIds = new HashSet<string>();
    public bool IsInitialized { get; private set; } = false;
    //记录宝箱已经打开
    public void RegisterOpenedChest(string chestId)
    {
        if(string.IsNullOrEmpty(chestId))
        {
            Debug.LogWarning("宝箱ID为空，无法记录");
            return;
        }
        openedChestIds.Add(chestId);
        Debug.Log("记录宝箱已打开：" + chestId);
    }
    //记录敌人已经死亡
    public void RegisterDefeatedEnemy(string enemyId)
    {
        if(string.IsNullOrEmpty(enemyId))
        {
            Debug.LogWarning("敌人ID为空，无法记录");
            return;
        }
        defeatedEnemyIds.Add(enemyId);
        Debug.Log("记录敌人已击败：" + enemyId);
    }
    //判断宝箱是否已经打开
    public bool IsChestOpened(string chestId)
    {
        return openedChestIds.Contains(chestId);
    }
    //判断敌人是否已经死亡
    public bool IsEnemyDefeated(string enemyId)
    { 
        return defeatedEnemyIds.Contains(enemyId);
    }
    //获取宝箱状态
    public List<string> GetOpenedChestIds()
    {
        return new List<string>(openedChestIds);
    }
    //获取敌人状态
    public List<string> GetDefeatedEnemyIds()
    {
        return new List<string>(defeatedEnemyIds);
    }
    //从存档加载世界状态
    public void LoadWorldState(List<string> chestIds,List<string> enemyIds)
    {
        openedChestIds.Clear();
        defeatedEnemyIds.Clear();
        if (chestIds != null)
        {
            foreach (string id in chestIds)
            {
                openedChestIds.Add(id);
            }
        }
        if (enemyIds != null)
        {
            foreach (string id in enemyIds)
            {
                defeatedEnemyIds.Add(id);
            }
        }
        IsInitialized = true;
        Debug.Log(
            "世界状态加载完成，已打开宝箱：" +
            openedChestIds.Count +
            "，已击败敌人：" +
            defeatedEnemyIds.Count
        );
    }
    //清空当前游戏的世界状态
    public void ClearWorldState()
    {
        openedChestIds.Clear();
        defeatedEnemyIds.Clear();
        Debug.Log("世界状态已清空");
    }
}
