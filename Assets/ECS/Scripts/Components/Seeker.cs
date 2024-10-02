using Unity.Entities;
using UnityEngine;

public class Seeker : MonoBehaviour
{
    public class Baker : Baker<Seeker>
    {
        public override void Bake(Seeker authoring)
        {
            AddComponent(GetEntity(authoring, TransformUsageFlags.Dynamic), new SeekerComponent());
        }
    }
}

public struct SeekerComponent : IComponentData
{
}
