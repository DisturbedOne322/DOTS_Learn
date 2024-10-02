using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FindClosestMono : MonoBehaviour
{
    private int _amount;
    public int BatchesPerJob = 50;

    private Transform[] _seekerTransforms;
    private Transform[] _targetTransforms;

    private JobHandle _jobHandle;

    private NativeArray<float3> _seekerPositionNativeArray;
    private NativeArray<float3> _targetPositionNativeArray;
    private NativeArray<float3> _nearestPositionNativeArray;

    public void Initialize(int amount, Transform[] seekers, Transform[] targets)
    {
        _amount = amount;

        _seekerTransforms = seekers;
        _targetTransforms = targets;

        _seekerPositionNativeArray = new NativeArray<float3>(_amount, Allocator.Persistent);
        _targetPositionNativeArray = new NativeArray<float3>(_amount, Allocator.Persistent);
        _nearestPositionNativeArray = new NativeArray<float3>(_amount, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        _seekerPositionNativeArray.Dispose();
        _targetPositionNativeArray.Dispose();
        _nearestPositionNativeArray.Dispose();
    }

    private void LateUpdate()
    {
        _jobHandle.Complete();
        for (int i = 0; i < _amount; i++)
        {
            Debug.DrawLine(_seekerPositionNativeArray[i], _nearestPositionNativeArray[i]);
        }
    }

    public void Update()
    {
        CopyPositions();
        QuickSort(0, _amount - 1);
        ScheduleJob();
    }

    private void CopyPositions()
    {
        for (int i = 0; i < _amount; i++)
        {
            _seekerPositionNativeArray[i] = _seekerTransforms[i].position;
            _targetPositionNativeArray[i] = _targetTransforms[i].position;
        }
    }

    private void ScheduleJob()
    {
        var findNearestJob = new FindNearestJob()
        {
            SeekerPositions = _seekerPositionNativeArray,
            TargetPositions = _targetPositionNativeArray,
            NearestTargetPositions = _nearestPositionNativeArray
        };
        _jobHandle = findNearestJob.Schedule(_amount, BatchesPerJob);
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
        float pivot = _targetPositionNativeArray[end].x;
        int pointer = start - 1;

        for (; start < end; start++)
        {
            if (_targetPositionNativeArray[start].x < pivot)
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
        (_targetPositionNativeArray[i], _targetPositionNativeArray[j]) = (_targetPositionNativeArray[j], _targetPositionNativeArray[i]);
    }

    [BurstCompile]
    private struct FindNearestJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float3> SeekerPositions;
        [ReadOnly]
        public NativeArray<float3> TargetPositions;

        public NativeArray<float3> NearestTargetPositions;

        public void Execute(int index)
        {
            float3 pos = SeekerPositions[index];
            int closestTargetID = BinarySearch(0, TargetPositions.Length, pos.x);

            // The position of the target with the closest X coord.
            float3 nearestTargetPos = TargetPositions[closestTargetID];
            float nearestDistSq = math.distancesq(pos, nearestTargetPos);

            // Searching upwards through the array for a closer target.
            Search(pos, closestTargetID + 1, TargetPositions.Length, +1, ref nearestTargetPos, ref nearestDistSq);

            // Search downwards through the array for a closer target.
            Search(pos, closestTargetID - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);

            NearestTargetPositions[index] = nearestTargetPos;
        }

        private int BinarySearch(int start, int end, float x)
        {
            int middle = start + (end - start) / 2;

            while (true)
            {
                middle = start + (end - start) / 2;
                float middlePosX = TargetPositions[middle].x;

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
                ref float3 nearestTargetPos, ref float nearestDistSq)
        {
            for (int i = startIdx; i != endIdx; i += step)
            {
                float3 targetPos = TargetPositions[i];
                float xdiff = seekerPos.x - targetPos.x;

                // If the square of the x distance is greater than the current nearest, we can stop searching.
                if ((xdiff * xdiff) > nearestDistSq) break;

                float distSq = math.distancesq(targetPos, seekerPos);

                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestTargetPos = targetPos;
                }
            }
        }
    }
}
