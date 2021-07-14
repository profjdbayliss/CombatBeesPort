using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees
{
    public struct RandomElement : IBufferElementData
    {
        // These implicit conversions are optional, but can help reduce typing.
        //public static implicit operator float(RandomElement e) { return e.Value; }
        //public static implicit operator RandomElement(float e) { return new RandomElement { Value = e }; }

        // Actual value each buffer element will store.
        public float Value;
    }

    public struct RandomArray : IComponentData
    {
    }
}