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
using Random = Unity.Mathematics.Random;

namespace sc.splines.spawner.runtime
{
    [BurstCompile]
    public struct RadialInsideSpline : IJob
    {
        //Input
        private NativeSpline spline;
        private float splineLength;
        private float4x4 splineTransform;

        private float3 boundsSize;
        private float3 minBounds;
        private float3 maxBounds;
        private float centerheight;
        
        [ReadOnly] private NativeList<PrefabData> prefabData;
        [WriteOnly] public NativeList<SpawnPoint> spawnPoints;
        
        [ReadOnly] Random random;
        
        private float radialSpacing;
        private float spacing;
        private float angleOffset;
        private float2 centerOffset;

        private float totalChanceWeights;

        public RadialInsideSpline(NativeSpline targetSpline, float4x4 localToWorld, DistributionSettings distributionSettings, NativeList<PrefabData> prefabData,
            ref NativeList<SpawnPoint> spawnPoints)
        {
            DistributionSettings.Radial settings = distributionSettings.radial;
            
            this.spline = targetSpline;
            this.splineTransform = localToWorld;
            this.splineLength = spline.GetLength();
            
            Bounds splineBounds = spline.GetBounds();
            this.boundsSize = splineBounds.size;
            centerheight = splineBounds.center.y;
            this.minBounds = splineBounds.min;
            this.maxBounds = splineBounds.max;

            random = new Random(distributionSettings.GetSeed());

            this.radialSpacing = settings.radialSpacing;
            this.spacing = settings.spacing;
            this.angleOffset = settings.offset;
            this.centerOffset = settings.center;

            totalChanceWeights = SplineFunctions.CalculateProbabilitySum(prefabData);
            
            this.prefabData = prefabData;
            this.spawnPoints = spawnPoints;
        }
        
        public void Execute()
        {
            if (splineLength < 1f) return;

            float3 center = new float3(minBounds.x + (boundsSize.x * 0.5f), 0f, minBounds.z + (boundsSize.z * 0.5f));
            center.x += centerOffset.x;
            center.z += centerOffset.y;
            
            float radius = math.max(boundsSize.x, boundsSize.z) * 0.5f;

            int rings = (int)math.ceil(radius / radialSpacing);

            for (int s = 0; s <= rings; s++)
            {
                float tStep = (float)s / (float)rings;

                float dist = (tStep + 0.01f) * radius;

                if(dist <= 0.1f) continue;
                
                float circumference = (2f * Mathf.PI * dist);
                int samplesPerRing = (int)math.ceil(circumference / spacing);
                
                float stepRotation = angleOffset * s;

                for (int b = 0; b < samplesPerRing; b++)
                {
                    float t = (float)b / samplesPerRing;
                    
                    float angle = (t * 360f);
                    
                    angle += stepRotation;
                    
                    angle *= Mathf.Deg2Rad;
                    float3 offset = new float3(math.sin(angle), 0f, math.cos(angle)) * dist;
                    float3 spawnPos = center + offset;

                    if (!SplineFunctions.IsInsideBounds(spawnPos, minBounds, maxBounds)) continue;

                    if (spline.IsInsideSpline(splineLength, spawnPos, spacing * 2f, 0f, out float3 nearest) == false)
                    {
                        continue;
                    }
                    
                    float r = random.NextFloat(0f, 1f);
                    int prefabIndex = SplineFunctions.GetRandomPrefabIndex(r, totalChanceWeights, this.prefabData);

                    if (prefabIndex >= 0)
                    {
                        SpawnPoint point = CreateSpawnPoint(spawnPos, prefabIndex);
                        point.context.position = nearest;
                        point.context.random01 = r;
                        point.context.noiseCoord = new float2(spawnPos.x * 0.1f, spawnPos.z * 0.1f);
                        
                        point.context.forward = math.normalize(center - spawnPos);
                        point.rotation = quaternion.LookRotationSafe(point.context.forward, math.up());
                        point.context.right = math.cross(point.context.forward, math.up());
                        point.context.up = math.up();
                        
                        spawnPoints.Add(point);
                    }
                }
            }
        }

        [BurstCompile]
        private SpawnPoint CreateSpawnPoint(float3 spawnPos, int prefabIndex)
        {
            PrefabData data = prefabData[prefabIndex];
            
            spawnPos.y = centerheight;
            
            SpawnPoint p = new SpawnPoint
            {
                isValid = true,
                position = spawnPos,
                prefabIndex = prefabIndex,
                scale = data.gameObjectScale
            };

            return p;
        }

        public void Dispose()
        {
            
        }
    }
}