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
    public struct SpawnOnGrid : IJob
    {
        //Input
        private NativeSpline spline;
        private float splineLength;
        private float4x4 splineTransform;

        private float3 boundsSize;
        private float3 minBounds;
        private float3 maxBounds;
        private float3 boundsCenter;
        private float centerheight;
        private float3 origin;
        
        [ReadOnly] private NativeList<PrefabData> prefabData;
        [WriteOnly] public NativeList<SpawnPoint> spawnPoints;
        
        [ReadOnly] Random random;
        
        private float totalChanceWeights;

        private bool concaveSupport;
        private bool createRows;
        private bool createColumns;
        private float rowSpacing;
        private float columnSpacing;

        private float spacingOnRows;
        private float spacingOnColumns;

        private float areaMargin;
        private float angle;
        private float minimumLength;

        private int rows;
        private int columns;

        private quaternion rotation;

        private float4x4 rotationMatrix;

        private DistributionSettings.Accuracy accuracy;
        //Scalar for distance between samples
        private float searchIntervalScalar;

        public SpawnOnGrid(NativeSpline targetSpline, float4x4 localToWorld, DistributionSettings distributionSettings, NativeList<PrefabData> prefabData,
            ref NativeList<SpawnPoint> spawnPoints)
        {
            DistributionSettings.Grid settings = distributionSettings.grid;
            
            this.spline = targetSpline;
            this.splineTransform = localToWorld;
            this.splineLength = spline.GetLength();
            
            this.areaMargin = settings.margin;
            
            this.angle = settings.angle;
            rotation = quaternion.AxisAngle(math.up(), angle * Mathf.Deg2Rad);
            
            //Bounds splineBounds = spline.CalculateBounds(splineLength, 2f);
            Bounds splineBounds = spline.GetBounds();
            
            splineBounds.Expand(new Vector3(-areaMargin, 0f, -areaMargin));
            this.boundsSize = splineBounds.size;
            centerheight = splineBounds.center.y;
            this.minBounds = splineBounds.min;
            this.maxBounds = splineBounds.max;
            this.boundsCenter = minBounds - maxBounds;
            
            rotationMatrix = float4x4.TRS(boundsCenter, rotation, new float3(1f));

            origin = minBounds;
            
            random = new Random(distributionSettings.GetSeed());
            totalChanceWeights = SplineFunctions.CalculateProbabilitySum(prefabData);

            this.concaveSupport = settings.concaveSupport;
            
            this.createRows = settings.createRows;
            this.createColumns = settings.createColumns;
            this.rowSpacing = settings.rowSpacing;
            this.columnSpacing = settings.columnSpacing;

            this.spacingOnRows = settings.spacingOnRows;
            this.spacingOnColumns = settings.spacingOnColumns;
            
            this.minimumLength = settings.minimumLength;

            rows = (int)math.ceil(boundsSize.z / rowSpacing);
            columns = (int)math.ceil(boundsSize.x / columnSpacing);
            
            this.prefabData = prefabData;
            this.spawnPoints = spawnPoints;
            
            this.accuracy = settings.accuracy;
            searchIntervalScalar = 1f;
            switch (accuracy)
            {
                case DistributionSettings.Accuracy.BestPerformance:
                {
                    searchIntervalScalar = 10f;
                    break;
                }
                case DistributionSettings.Accuracy.PreferPerformance:
                {
                    searchIntervalScalar = 7.5f;
                    break;
                }
                case DistributionSettings.Accuracy.Balanced:
                {
                    searchIntervalScalar = 5f;
                    break;
                }
                case DistributionSettings.Accuracy.PreferAccuracy:
                {
                    searchIntervalScalar = 2f;
                    break;
                }
                case DistributionSettings.Accuracy.HighestAccuracy:
                {
                    searchIntervalScalar = 1f;
                    break;
                }
            }
        }
        
        public void Execute()
        {
            if (splineLength < 1f) return;
            
            float3 forward = math.mul(splineTransform, new float4(math.forward(), 0f)).xyz;
            float3 right = math.mul(splineTransform, new float4(math.right(), 0f)).xyz;
            
            forward = math.forward();
            right = math.right();
            
            //forward = math.rotate(rotationMatrix, forward).xyz;
            //right = math.rotate(rotationMatrix, right).xyz;
            
            //X-axis
            if (createRows)
            {
                CreateLanes(forward, right, boundsSize.z, boundsSize.x, rows, spacingOnRows);
            }
            //Z-axis
            if (createColumns)
            {
                CreateLanes(right, forward, boundsSize.x, boundsSize.z, columns, spacingOnColumns);
            }
        }
        
        void CreateLanes(float3 axis, float3 direction, float width, float length, int count, float cellSize)
        {
            float spacing = width / (count);

            int startIndex = 0;
            int endIndex = count;
            
            //Can skip creating lanes at the very ends of the bounds, as these will be culled anyway
            if (areaMargin <= 0.5f)
            {
                startIndex = 1;
                endIndex = count-1;
            }
            
            float searchIntervalDistance = cellSize * searchIntervalScalar;
            
            for (int i = startIndex; i <= endIndex; i++)
            {
                float3 start = origin + (axis * (i * spacing));
                float3 end = start + (direction * length);

                bool startInside = true;
                bool endInside = true;

                if (concaveSupport == false)
                {
                    startInside = spline.IsInsideSpline(splineLength, start, searchIntervalDistance, areaMargin, out var _);

                    //Point falls outside of spline, start tracing inwards to find the first point that is
                    if (startInside == false)
                    {
                        if (spline.FindNearestPointWithinSpline(splineLength, start, end, direction, searchIntervalDistance, areaMargin, out float3 nearestStart))
                        {
                            start = nearestStart;
                            startInside = true;
                        }
                    }

                    //Bail out
                    if (startInside == false) continue;

                    endInside = spline.IsInsideSpline(splineLength, end, searchIntervalDistance, areaMargin, out var _);

                    if (endInside == false)
                    {
                        if (spline.FindNearestPointWithinSpline(splineLength, end, start, -direction, searchIntervalDistance, areaMargin, out float3 nearestEnd))
                        {
                            end = nearestEnd;
                            endInside = true;
                        }
                    }

                    if (endInside == false) continue;
                }

                float distance = math.abs(math.distance(start, end));
                
                if(distance - (areaMargin * 2f) < minimumLength) continue;
                
                //Have a valid lane, create spawn points along it
                int pointCount = (int)math.ceil(distance / cellSize);

                //Transform points to world-space
                //start = math.mul(splineTransform, new float4(start.xyz, 0f)).xyz;
                //end = math.mul(splineTransform, new float4(end.xyz, 0f)).xyz;

                for (int j = 0; j < pointCount; j++)
                {
                    float t = (float)j / pointCount;

                    float3 spawnPos = math.lerp(start, end, t);

                    //float curve = ((math.sin(t * Mathf.PI) * 0.5f + 0.5f)) * curving;   
                    //spawnPos += axis * curve;

                    float3 nearestPosition = float3.zero;

                    //Re-check if the point is inside of the spline, as the line may be crossing through
                    if (concaveSupport)
                    {
                        bool isInside = spline.IsInsideSpline(splineLength, spawnPos, searchIntervalDistance, areaMargin, out nearestPosition);
                        
                        if(isInside == false) continue;
                    }
                    else
                    {
                        //Find best nearest point on spline
                        if (math.distance(spawnPos, start) < math.distance(spawnPos, end)) nearestPosition = start;
                        else nearestPosition = end;
                    }

                    float r = random.NextFloat(0f, 1f);
                    int prefabIndex = SplineFunctions.GetRandomPrefabIndex(r, totalChanceWeights, this.prefabData);

                    if (prefabIndex >= 0)
                    {
                        SpawnPoint spawnPoint = CreateSpawnPoint(spawnPos, prefabIndex);

                        //spawnPoint.context.forward = math.normalize(spawnPos - nearestPosition);
                        spawnPoint.context.forward = direction;
                        spawnPoint.context.right = math.cross(spawnPoint.context.forward, math.up());
                        ;
                        spawnPoint.context.up = math.up();

                        spawnPoint.context.position = nearestPosition;

                        spawnPoints.Add(spawnPoint);
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

            p.context.noiseCoord = new float2(spawnPos.x, spawnPos.z);
            p.context.random01 = random.NextFloat(0f, 1f);

            return p;
        }

        public void Dispose()
        {
            
        }
    }
}