using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct EntityMoveSystem : ISystem
{
    EntityQuery query_move;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MoveComponent>();
        query_move = state.GetEntityQuery(ComponentType.ReadWrite<LocalTransform>(), ComponentType.ReadOnly<MoveComponent>());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new MoveEntityJob() { DeltaTime = SystemAPI.Time.DeltaTime }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct MoveEntityJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(ref LocalTransform transform, in MoveComponent moveComponent)
        {
            transform = transform.Translate(moveComponent.Direction * DeltaTime);
        }
    }
}
