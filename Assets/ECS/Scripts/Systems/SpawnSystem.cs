using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SpawnSystem : ISystem
{
    private NativeArray<Entity> _seekersNativeArray;
    private NativeArray<Entity> _targetsNativeArray;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnerData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        SpawnEntities(ref state, SystemAPI.GetSingleton<SpawnerData>().SeekerPrefab, _seekersNativeArray);
        SpawnEntities(ref state, SystemAPI.GetSingleton<SpawnerData>().TargetPrefab, _targetsNativeArray);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        _seekersNativeArray.Dispose();
        _targetsNativeArray.Dispose();
    }

    private void SpawnEntities(ref SystemState state, Entity prefab, NativeArray<Entity> array)
    {
        EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);

        array = new NativeArray<Entity>(SystemAPI.GetSingleton<SpawnerData>().AmountToSpawn, Allocator.Persistent);
        buffer.Instantiate(prefab, array);
        var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(0,999));

        foreach (var entity in array)
        {
            buffer.SetComponent<LocalTransform>(entity, new LocalTransform()
            {
                Position = random.NextFloat3(new float3(-990, 0, -520), new float3(990, 0, 520)),
                Scale = 1,
                Rotation = UnityEngine.Quaternion.identity,
            });

            buffer.SetComponent<MoveComponent>(entity, new MoveComponent()
            {
                Direction = random.NextFloat3(new float3(-1, 0, -1), new float3(1, 0, 1)),
            });
        }
        buffer.Playback(state.EntityManager);
    }
}
