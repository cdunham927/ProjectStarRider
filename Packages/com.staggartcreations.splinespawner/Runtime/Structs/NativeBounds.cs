using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace sc.splines.spawner.runtime
{
    [BurstCompile]
    public struct NativeBounds 
    {
        public float3 min;
        public float3 max;
        public float3 size;
        public float3 center;
        
        public void SetMinMax(float3 newMin, float3 newMax)
        {
            min = newMin;
            max = newMax;
            size = newMax - newMin;
            center = newMin + (size * 0.5f);
        }

        public void SetCenterSize(float3 newCenter, float3 newSize)
        {
            center = newCenter;
            size = newSize;
            
            min = center - size * 0.5f;
            max = center - size * 0.5f;
        }

        public void Expand(float padding)
        {
            min -= padding;
            max += padding;
            size += padding * 2f;
            center = min + size * 0.5f;
        }

        public static NativeBounds Create(NativeSpline spline, float sampleDistance = 1f)
        {
            NativeArray<NativeBounds> bounds = new NativeArray<NativeBounds>(1, Allocator.TempJob);

            var boundsCalculationJob = new BoundsCalculationJob(spline, spline.GetLength(), sampleDistance, bounds);
            var boundsJobHandle = boundsCalculationJob.Schedule();
            boundsJobHandle.Complete();

            NativeBounds splineBounds = bounds[0];

            bounds.Dispose();

            return splineBounds;
        }
        
        [BurstCompile]
        public struct BoundsCalculationJob : IJob
        {
            private readonly int sampleCount;
            private readonly NativeSpline spline;

            [WriteOnly]
            public NativeArray<NativeBounds> boundsOut;

            public BoundsCalculationJob(NativeSpline spline, float splineLength, float searchInterval, NativeArray<NativeBounds> boundsOut)
            {
                sampleCount = Mathf.CeilToInt(splineLength / searchInterval);
                this.spline = spline;
                this.boundsOut = boundsOut;
            }

            public void Execute()
            {
                float3 min = new float3(float.MaxValue);
                float3 max = new float3(float.MinValue);

                for (int i = 0; i < sampleCount; i++)
                {
                    float t = i / (float)sampleCount;
                    
                    float3 position = spline.EvaluatePosition(t);
                    
                    min = math.min(position, min);
                    max = math.max(position, max);
                }

                NativeBounds bounds = new NativeBounds();
                bounds.SetMinMax(min, max);

                boundsOut[0] = bounds;
            }
        }
    }
}