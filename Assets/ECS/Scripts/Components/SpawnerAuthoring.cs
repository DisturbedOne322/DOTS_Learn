using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public int AmountToSpawn = 20000;
    public GameObject SeekerPrefab;
    public GameObject TargetPrefab;

    class SpawnerBaker: Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.None);

            var spawner = new SpawnerData()
            {
                AmountToSpawn = authoring.AmountToSpawn,
                SeekerPrefab = GetEntity(authoring.SeekerPrefab, TransformUsageFlags.Dynamic),
                TargetPrefab = GetEntity(authoring.TargetPrefab, TransformUsageFlags.Dynamic)
            };

            AddComponent(entity, spawner);
        }
    }
}

public struct SpawnerData : IComponentData
{
    public int AmountToSpawn;
    public Entity SeekerPrefab;
    public Entity TargetPrefab;
}
