using Unity.Entities;
using Unity.Mathematics;

public class BrickInfo : IComponentData
{
    public int _id;
    public bool _isExist;
    public string _meshName;
    public string _matCategory;
    public string _matName;
    public float3 _rotationValue;
    public float _scaleFactor;
}
