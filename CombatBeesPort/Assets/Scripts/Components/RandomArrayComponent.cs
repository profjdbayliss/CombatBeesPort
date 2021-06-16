using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct RandomArrayComponent : IComponentData
{
    //public float randomArray;
    public int maxSize;   
}
