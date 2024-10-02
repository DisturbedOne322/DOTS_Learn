using Unity.Entities;
using UnityEngine;

public class Target : MonoBehaviour
{
    public class Baker : Baker<Target>
    {
        public override void Bake(Target authoring)
        {
            AddComponent(GetEntity(authoring, TransformUsageFlags.Dynamic), new TargetComponent());
        }
    }
}

public struct TargetComponent : IComponentData
{
}
