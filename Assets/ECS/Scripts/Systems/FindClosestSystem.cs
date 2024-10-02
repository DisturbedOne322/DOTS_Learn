using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FindClosestSystem : ISystem, ISystemStartStop
{
    private NativeArray<LocalTransform> _closestNativeArray;
    private NativeArray<LocalTransform> _targetTransformNativeArray;
    private NativeArray<LocalTransform> _seekersTransformNativeArray;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TargetComponent>();
        state.RequireForUpdate<SeekerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UpdateTargetTransforms(ref state);
        UpdateSeekersTransforms(ref state);

        QuickSort(0, SystemAPI.GetSingleton<SpawnerData>().AmountToSpawn - 1);
        var findNearestJob = new FindNearestJob()
        {
            SeekerPositions = _seekersTransformNativeArray,
            TargetPositions = _targetTransformNativeArray,
            NearestTargetPositions = _closestNativeArray
        };

        findNearestJob.Schedule(SystemAPI.GetSingleton<SpawnerData>().AmountToSpawn, 512).Complete();

        DrawLinesToTargets();
    }

    private void QuickSort(int start, int end)
    {
        if (start >= end)
            return;

        int partition = Partition(start, end);
        QuickSort(start, partition - 1);
        QuickSort(partition + 1, end);
    }

    private int Partition(int start, int end)
    {
        float pivot = _targetTransformNativeArray[end].Position.x;
        int pointer = start - 1;

        for (; start < end; start++)
        {
            if (_targetTransformNativeArray[start].Position.x < pivot)
            {
                pointer++;
                Swap(pointer, start);
            }
        }

        Swap(pointer + 1, end);

        return pointer + 1;
    }

    private void Swap(int i, int j)
    {
        (_targetTransformNativeArray[i], _targetTransformNativeArray[j]) = (_targetTransformNativeArray[j], _targetTransformNativeArray[i]);
    }

    private void UpdateTargetTransforms(ref SystemState state)
    {
        int id = 0;
        foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<TargetComponent>())
        {
            _targetTransformNativeArray[id] = transform.ValueRO;
            id++;
        }
    }

    private void UpdateSeekersTransforms(ref SystemState state)
    {
        int id = 0;
        foreach (var transform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<SeekerComponent>())
        {
            _seekersTransformNativeArray[id] = transform.ValueRO;
            id++;
        }
    }

    private void DrawLinesToTargets()
    {
        int size = _seekersTransformNativeArray.Length;
        for(int i = 0; i < size; i++)
        {
            Debug.DrawLine(_seekersTransformNativeArray[i].Position, _closestNativeArray[i].Position);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        _closestNativeArray.Dispose();
        _seekersTransformNativeArray.Dispose();
        _targetTransformNativeArray.Dispose();
    }

    public void OnStartRunning(ref SystemState state)
    {
        _closestNativeArray = new (SystemAPI.GetSingleton<SpawnerData>().AmountToSpawn, Allocator.Persistent);
        _targetTransformNativeArray = new (SystemAPI.GetSingleton<SpawnerData>().AmountToSpawn, Allocator.Persistent);
        _seekersTransformNativeArray = new(SystemAPI.GetSingleton<SpawnerData>().AmountToSpawn, Allocator.Persistent);
    }

    public void OnStopRunning(ref SystemState state)
    {
    }

    [BurstCompile]
    private struct FindNearestJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<LocalTransform> SeekerPositions;
        [ReadOnly]
        public NativeArray<LocalTransform> TargetPositions;

        public NativeArray<LocalTransform> NearestTargetPositions;

        public void Execute(int index)
        {
            float3 pos = SeekerPositions[index].Position;
            int closestTargetID = BinarySearch(0, TargetPositions.Length, pos.x);

            LocalTransform nearestTarget = TargetPositions[closestTargetID];
            float nearestDistSq = math.distancesq(pos, nearestTarget.Position);

            // Searching upwards through the array for a closer target.
            Search(pos, closestTargetID + 1, TargetPositions.Length, +1, ref nearestTarget, ref nearestDistSq);

            // Search downwards through the array for a closer target.
            Search(pos, closestTargetID - 1, -1, -1, ref nearestTarget, ref nearestDistSq);

            NearestTargetPositions[index] = nearestTarget;
        }

        private int BinarySearch(int start, int end, float x)
        {
            int middle = start + (end - start) / 2;

            while (true)
            {
                middle = start + (end - start) / 2;
                float middlePosX = TargetPositions[middle].Position.x;

                if (middlePosX < x)
                {
                    start = middle + 1;
                }
                else
                {
                    end = middle - 1;
                }

                if (start >= end)
                    break;
            }

            return middle;
        }

        void Search(float3 seekerPos, int startIdx, int endIdx, int step,
                ref LocalTransform nearestTargetPos, ref float nearestDistSq)
        {
            for (int i = startIdx; i != endIdx; i += step)
            {
                LocalTransform target = TargetPositions[i];
                float xdiff = seekerPos.x - target.Position.x;

                // If the square of the x distance is greater than the current nearest, we can stop searching.
                if ((xdiff * xdiff) > nearestDistSq) break;

                float distSq = math.distancesq(target.Position, seekerPos);

                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestTargetPos = target;
                }
            }
        }
    }
}
