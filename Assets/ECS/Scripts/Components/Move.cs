using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Move : MonoBehaviour
{
    public class Baker : Baker<Move>
    {
        public override void Bake(Move authoring)
        {
            AddComponent(GetEntity(authoring, TransformUsageFlags.Dynamic), new MoveComponent());
        }
    }
}

public struct MoveComponent : IComponentData
{
    public Vector3 Direction;
}

