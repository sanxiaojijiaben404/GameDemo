//资源定义
using System;
[Serializable]
public class ResourceConfig
{
    public int id;
    public string itemName;
    public int maxCount;
    public ResourceEffectType effectType;
    public float effectValue;
    public float effectDuration;
}