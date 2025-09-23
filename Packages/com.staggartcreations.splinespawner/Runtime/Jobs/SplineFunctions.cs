// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace sc.splines.spawner.runtime
{
    public static class SplineFunctions
    {
        public static float CalculateProbabilitySum(NativeList<PrefabData> prefabData)
        {
            float totalChanceWeights = 0f;
            for (int i = 0; i < prefabData.Length; i++)
            {
                totalChanceWeights += prefabData[i].probability;
            }

            return totalChanceWeights;
        }
        
        //'r' value should be normalized (0-1)
        [BurstCompile]
        public static int GetRandomPrefabIndex(float r, float totalChanceWeights, NativeList<PrefabData> prefabData, float distance = 0f)
        {
            //If multiple prefabs have a 100% probability, they are evenly weighted
            float scale = totalChanceWeights;

            //If all prefabs have 100% probability, only the first one gets picked.
            if (prefabData.Length == 1) scale = 100f;
            
            float cumulativeWeight = 0f;
            for (int i = 0; i < prefabData.Length; i++)
            {
                cumulativeWeight += prefabData[i].probability;
                
                if (distance > 0)
                {
                    cumulativeWeight *= (math.lerp(1f, 100f / distance, prefabData[i].distanceSelectionWeight));
                }
                
                if (r * scale <= cumulativeWeight)
                {
                    return i;
                }
            }
            
            //Remaining portion, chance to pick nothing
            if (r > totalChanceWeights)
                return -1;

            //None
            return -1;
        }
        
        [BurstCompile]
        public static bool IsInsideBounds(float3 position, float3 boundsMin, float3 boundsMax)
        {
            float3 boundsSize = boundsMax - boundsMin;
            
            return position.x >= boundsMin.x && position.x <= (boundsMin.x + boundsSize.x) &&
                   position.z >= boundsMin.z && position.z <= (boundsMin.z + boundsSize.z);
        }
        
        [BurstCompile]
        public static bool IsInsideSpline(this NativeSpline spline, float splineLength, float3 position, float searchInterval, float margin, out float3 nearestPosition)
        {
            float paddingSquared = margin * margin;
            int sampleCount = (int)math.floor(splineLength / searchInterval);

            nearestPosition = spline.EvaluatePosition(0);
            float3 previousPosition = nearestPosition;

            bool isInside = false;
            float minDistSq = float.MaxValue;

            //Skip first iteration since the spline start has already been sampled
            for (int i = 1; i <= sampleCount; i++)
            {
                float t = (float)i / (float)sampleCount;

                float3 splinePoint = spline.EvaluatePosition(t);

                //Check for distance from spline
                float distSq = math.distancesq(splinePoint, position);
                if (distSq < paddingSquared)
                {
                    return false;
                }

                //Track nearest point for output
                if (distSq < minDistSq)
                {
                    minDistSq = distSq;
                    nearestPosition = splinePoint;
                }

                // Optimized ray-edge intersection test
                if ((previousPosition.x <= position.x && splinePoint.x > position.x) || 
                    (splinePoint.x <= position.x && previousPosition.x > position.x))
                {
                    float intersectionZ = previousPosition.z + 
                                          (position.x - previousPosition.x) * 
                                          (splinePoint.z - previousPosition.z) / 
                                          (splinePoint.x - previousPosition.x);
                
                    if (intersectionZ > position.z)
                    {
                        isInside = !isInside;
                    }
                }
        
                previousPosition = splinePoint;
            }

            if (isInside)
            {
                //Slow, can account for over 60% of the processing time
                //SplineUtility.GetNearestPoint(spline, position, out nearestPosition, out var nearestT, 1, 1);
            }

            return isInside;
        }
        
        [BurstCompile]
        //If a position falls outside the spline, trace forward/backwards direction to find the nearest position that does.
        public static bool FindNearestPointWithinSpline(this NativeSpline spline, float splineLength, float3 start, float3 end, float3 direction, float sampleIntervalDistance, float margin, out float3 nearestPosition)
        {
            float laneLength = math.abs(math.distance(start, end));
            int sampleCount = (int)math.ceil(laneLength / sampleIntervalDistance);

            for (int j = 0; j < sampleCount; j++)
            {
                float t = j / (float)(sampleCount-1);
                        
                float3 samplePos = start + (direction * (t * laneLength));

                if (spline.IsInsideSpline(splineLength, samplePos, sampleIntervalDistance, margin, out var nearest))
                {
                    nearestPosition = nearest;
                    return true;
                }
                        
            }

            nearestPosition = start;
            return false;
        }

        public static Bounds BoundsFromKnots(this ISpline spline, float4x4 localToWorld)
        {
            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = Vector3.one * float.MinValue;
            
            int knotCount = spline.Count;

            for (int i = 0; i < knotCount; i++)
            {
                float3 position = spline[i].Position;
                position = math.mul(localToWorld, new float4(position, 1.0f)).xyz;
                
                min = Vector3.Min(position, min);
                max = Vector3.Max(position, max);
            }
            
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            return bounds;
        }

        public static NativeBounds GetNativeBounds(this NativeSpline spline, float sampleDistance)
        {
            return NativeBounds.Create(spline, sampleDistance);
        }
        
        public static Bounds CalculateBounds(this NativeSpline spline, float splineLength, float searchInterval)
        {
            int samples = Mathf.CeilToInt(splineLength / searchInterval);

            Vector3 min = Vector3.one * float.MaxValue;
            Vector3 max = Vector3.one * float.MinValue;
            
            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples;

                float3 position = spline.EvaluatePosition(t);

                min = Vector3.Min(position, min);
                max = Vector3.Max(position, max);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            return bounds;
        }
    }
}