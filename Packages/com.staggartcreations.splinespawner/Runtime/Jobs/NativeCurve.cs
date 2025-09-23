// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace sc.splines.spawner.runtime
{
    [BurstCompile]
    public struct NativeCurve
    {
        [ReadOnly]
        private NativeArray<float> samples;
        private const int SampleCount = 16;
        
        public NativeCurve(AnimationCurve curve, Allocator allocator)
        {
            samples = new NativeArray<float>(SampleCount, allocator, NativeArrayOptions.UninitializedMemory);
            
            Update(curve);
        }

        public void Update(AnimationCurve curve)
        {
            for (int i = 0; i < SampleCount; i++)
            {
                float t = (float)i / (SampleCount - 1);
                samples[i] = curve.Evaluate(t);
            }
        }

        public float Sample(float t)
        {
            t = math.clamp(t, 0f, 1f);
            float scaled = t * (SampleCount - 1);
            int index = (int)math.floor(scaled);
            int nextIndex = math.min(index + 1, SampleCount - 1);
            float frac = scaled - index;

            return math.lerp(samples[index], samples[nextIndex], frac);
        }

        public void Dispose()
        {
            if (samples.IsCreated)
                samples.Dispose();
        }
    }
}