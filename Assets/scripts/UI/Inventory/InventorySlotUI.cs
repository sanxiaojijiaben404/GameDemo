using TMPro;
using UnityEngine;
using UnityEngine.UI;
//背包单格子UI脚本，挂载在每一个背包格子预制体上，负责渲染单个资源的名称、数量
public class InventorySlotUI : MonoBehaviour
{
    //资源数量文本组件（TMP文字）
    public TMP_Text countText;
    //资源名称文本组件（TMP文字）
    public TMP_Text nameText;

    public Image slotBg;
    public Image iconImage;
    public ResourceUseManager resourceUseManager;
    private ResourceData currentData;
    // 外部调用入口：给格子填充资源数据，刷新UI显示
    public void SetData(ResourceData data, InventoryIconConfig config)
    {
        
        currentData = data;
        //安全空值判断，防止组件未拖拽出现空引用报错
        if (nameText != null)
        {
            nameText.text = data.name;
        }
        if (countText != null)
        {
            countText.text = "x" + data.count;
        }
        if (iconImage != null && config != null)
        {
            Sprite icon = config.GetIcon(data.id);
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }
        else
        {
            Debug.LogError("无法加载图标！");
        }
    }
    //点击格子使用资源
    public void UseItem()
    {
        if (currentData == null)
        {
            Debug.Log("当前格子没有资源");
            return;
        }
        if (resourceUseManager == null)
        {
            Debug.LogError("ResourceUseManager没有绑定");
            return;
        }
        resourceUseManager.UseResource(
            currentData.id
        );
    }
    public void ClearSlot()
    {
        currentData = null;
        if (nameText) nameText.text = "";
        if (countText) countText.text = "";
        if (iconImage)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}