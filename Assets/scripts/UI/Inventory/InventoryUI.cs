using System.Collections.Generic;
using UnityEngine;
//背包总面板控制器
//监听按键开关背包、刷新所有格子、从ResourseManager拉取数据并生成UI格子
public class InventoryUI : MonoBehaviour
{
    //背包面板根物体
    public GameObject panel;
    //ScrollView内部Content容器，所有格子作为它的子物体
    public Transform content;
    //格子Prefab（挂载InventorySlotUI脚本）
    public GameObject slotPrefab;
    public InventoryIconConfig iconConfig;
    public ResourceUseManager resourceUseManager;
    //逻辑管理器
    public ResourceSystemHost resourceSystem;
    private ResourceManager manager;
    private void Awake()
    {      
        manager = resourceSystem.Manager;
    }
    void Update()
    {
        //按I打开关闭背包
        if (Input.GetKeyDown(KeyCode.I))
        {
            //取反当前激活状态
            panel.SetActive(!panel.activeSelf);
            //打开时刷新
            if (panel.activeSelf)
            {
                Refresh();
            }
        }
    }
    private void Start()
    {
        // 如果 manager 还没赋值，重新获取一次
        if (manager == null && resourceSystem != null)
        {
            manager = resourceSystem.Manager;
        }
        if(manager != null)
        {
            manager.OnResourceChanged += Refresh;
        }
    }
    private void OnDestroy()
    {
        if(manager!=null)
        {
            manager.OnResourceChanged -= Refresh;
        }
    }
    //刷新背包
    public void Refresh()
    {
        if (manager == null)
        {
            Debug.LogWarning("manager 未初始化，跳过刷新");
            return;
        }
        if (!manager.IsInitialized)
        {
            Debug.Log("资源数据尚未加载完成，暂不刷新背包");
            return;
        }
        Debug.Log("背包刷新");
        //清空Content里所有旧格子
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        //从管理器获取最新背包资源列表
        List<ResourceData> resources = manager.GetAllResource();
        Debug.Log("UI读取资源数量:" + resources.Count);
        //遍历资源，实例化格子并填充数据
        foreach (ResourceData resource in resources)
        {
            //生成格子预制体
            GameObject slot = Instantiate(slotPrefab);
            //挂载到Content下，false消除世界坐标偏移，强制使用父物体本地UI坐标
            slot.transform.SetParent(content.transform, false);
            //强制归零缩放、位置，彻底消除偏移
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localScale = Vector3.one;
            slot.SetActive(true); // 强制激活
            //获取格子上的UI组件
            InventorySlotUI ui = slot.GetComponent<InventorySlotUI>();
            ui.resourceUseManager = resourceUseManager;
            //把资源数据传递给格子渲染文字
            ui.SetData(resource,iconConfig);
        }
    }
}