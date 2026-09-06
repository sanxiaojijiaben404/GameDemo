using UnityEngine;
[CreateAssetMenu(fileName ="ResourceEffectConfig",menuName ="Resource/EffectConfig")]
public class ResourceEffectConfig : ScriptableObject
{
    public int resourceID;
    public ResourceEffectType type;
    public float value;
}
