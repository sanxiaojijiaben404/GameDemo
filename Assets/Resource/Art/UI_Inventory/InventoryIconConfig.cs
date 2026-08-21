using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "SO_InventoryIcon", menuName = "Config/背包图标配置")]
public class InventoryIconConfig : ScriptableObject
{
    public static InventoryIconConfig Instance;
    [System.Serializable]
    public struct IconItem
    {
        public int resourceId;
        public Sprite iconSprite;
    }
    public List<IconItem> iconList;
    private void OnEnable()
    {
        //只允许第一次赋值，防止被其他同类型SO覆盖
        if (Instance == null)
            Instance = this;
    }
    private void OnDisable()
    {
        if(Instance == this)
            Instance = null;
    }
    public Sprite GetIcon(int id)
    {
        foreach (var item in iconList)
        {
            if (item.resourceId == id)
                return item.iconSprite;
        }
        return null;
    }
}
