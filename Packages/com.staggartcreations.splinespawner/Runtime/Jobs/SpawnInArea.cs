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
    [BurstCompile(FloatPrecision.Medium, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance, CompileSynchronously = true)]
    public struct SpawnInArea : IJob
    {
        //Input
        private NativeSpline spline;
        private float splineLength;

        private float3 boundsSize;
        private float3 minBounds;
        private float3 maxBounds;
        private float centerheight;

        [ReadOnly] private NativeList<PrefabData> prefabData;

        //Output
        [WriteOnly] public NativeList<SpawnPoint> spawnPoints;

        private float totalChanceWeights;

        private float spawnRadius;
        private float areaPadding;

        private Random random;
        private NativeArray<float> searchAngles;
        
        //Poisson
        private NativeList<float3> samples;
        private NativeList<float3> points;
        private NativeArray<int> grid;
        private int2 gridResolution;
        private float cellSize;

        private DistributionSettings.Accuracy overlapAccuracy;
        private DistributionSettings.Accuracy borderAccuracy;
        private int searchAttempts;
        private float searchIntervalScalar;

        public SpawnInArea(NativeSpline targetSpline, NativeBounds bounds,
            DistributionSettings distributionSettings, NativeList<PrefabData> prefabData,
            ref NativeList<SpawnPoint> spawnPoints)
        {
            DistributionSettings.InsideArea settings = distributionSettings.insideArea;
            
            this.spline = targetSpline;
            this.splineLength = spline.GetLength();

            this.overlapAccuracy = settings.overlapAccuracy;
            this.borderAccuracy = settings.borderAccuracy;
            
            searchAttempts = 1;
            searchIntervalScalar = 1f;
            searchAttempts = overlapAccuracy switch
            {
                DistributionSettings.Accuracy.BestPerformance => 2,
                DistributionSettings.Accuracy.PreferPerformance => 3,
                DistributionSettings.Accuracy.Balanced => 4,
                DistributionSettings.Accuracy.PreferAccuracy => 5,
                DistributionSettings.Accuracy.HighestAccuracy => 6,
                _ => searchAttempts
            };

            searchIntervalScalar = borderAccuracy switch
            {
                DistributionSettings.Accuracy.BestPerformance => 10f,
                DistributionSettings.Accuracy.PreferPerformance => 7f,
                DistributionSettings.Accuracy.Balanced => 5f,
                DistributionSettings.Accuracy.PreferAccuracy => 2f,
                DistributionSettings.Accuracy.HighestAccuracy => 1f,
                _ => searchIntervalScalar
            };

            this.boundsSize = bounds.size;
            centerheight = bounds.center.y;
            this.minBounds = bounds.min;
            this.maxBounds = bounds.max;

            float minSize = 0.25f;

            totalChanceWeights = 0;
            for (int i = 0; i < prefabData.Length; i++)
            {
                if (prefabData[i].probability > 0)
                {
                    minSize = math.max(minSize, prefabData[i].GetRadiusXZ() * 0.5f);
                }

                //Also sum the probabilities
                totalChanceWeights += prefabData[i].probability;
            }
            
            //Ensure the search interval distance is always larger than the smallest object
            //searchIntervalScalar = Mathf.Max(minSize * 0.5f, searchIntervalScalar);

            //Ensure that the spacing is never smaller than the smallest object
            this.spawnRadius = Mathf.Max(minSize, settings.spacing);

            this.areaPadding = settings.padding;
            float3 padding = new float3(spawnRadius);
            minBounds += padding;
            maxBounds -= padding;
            boundsSize -= padding * 0.5f;

            this.prefabData = prefabData;
            this.spawnPoints = spawnPoints;

            random = new Random(distributionSettings.GetSeed());

            //Setup
            {
                samples = new NativeList<float3>(8196, Allocator.TempJob);
                points = new NativeList<float3>(8196, Allocator.TempJob);

                float width = math.max(boundsSize.x, boundsSize.z);

                // Grid setup
                cellSize = spawnRadius / Mathf.Sqrt(2);
                gridResolution = new int2(
                    (int)math.ceil(width / cellSize),
                    (int)math.ceil(width / cellSize)
                );

                grid = new NativeArray<int>(gridResolution.x * gridResolution.y, Allocator.TempJob,
                    NativeArrayOptions.ClearMemory);
                //for (int i = 0; i < grid.Length; i++) grid[i] = -1; // Initialize grid cells to -1

                //Debug.Log($"Spawn radius: {spawnRadius}. Grid resolution: {gridResolution.x}x{gridResolution.y}. Grid scale: {width}. Cell count:{grid.Length}");
            }

            dirToSpline = math.right();

            //Use a set of fixed angles rather than random, to ensure optimal coverage and avoid under- or oversampling

            float angleStep = 22.5f;
            int angleCount = Mathf.CeilToInt(360 / angleStep);
            searchAngles = new NativeArray<float>(angleCount, Allocator.TempJob);

            float curAngle = 0;
            for (int i = 0; i < angleCount; i++)
            {
                searchAngles[i] = curAngle;
                
                curAngle += angleStep;
            }
        }

        private float3 dirToSpline;

        public void Execute()
        {
            if (splineLength < 1f) return;
            
            //Center of bounds
            var firstPoint = new float3(minBounds.x + (boundsSize.x * 0.5f), 0f, minBounds.z + (boundsSize.z * 0.5f));

            AddValidPoint(firstPoint);

            //Sampling loop
            while (samples.Length > 0)
            {
                int i = random.NextInt(0, samples.Length);
                var sampleCenter = samples[i];
                bool valid = false;

                for (int s = 0; s < searchAttempts; s++)
                {
                    float3 sample = RandomPointOnAnnulus(sampleCenter);

                    if (IsPointValid(sample))
                    {
                        AddValidPoint(sample);

                        float intervalDistance = spawnRadius * searchIntervalScalar;

                        //Check spline area
                        bool insideSpline = spline.IsInsideSpline(splineLength, sample, intervalDistance, areaPadding, out float3 nearestPosition);

                        //insideSpline = true;
                        if (insideSpline)
                        {
                            float r = random.NextFloat(0, 1f);

                            int prefabIndex = SplineFunctions.GetRandomPrefabIndex(r, totalChanceWeights, prefabData, math.distance(nearestPosition, sample));

                            if (prefabIndex >= 0)
                            {
                                SpawnPoint spawnPoint = CreateSpawnPoint(sample, r, prefabIndex);

                                int2 cell = PositionToGridCoord(sample);
                                float3 gridPosition = new float3(
                                    ((minBounds.x + (cell.x * cellSize))),
                                    0f,
                                    ((minBounds.z + (cell.y * cellSize))));
                                //spawnPoint.position = math.lerp(gridPosition, spawnPoint.position, poissonStrength);

                                spawnPoint.context.position = nearestPosition;

                                spawnPoints.Add(spawnPoint);
                            }
                        }

                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    samples.RemoveAtSwapBack(i);
                }
            }

        }

        [BurstCompile]
        private void AddValidPoint(float3 position)
        {
            samples.Add(position);
            points.Add(position);
            SetGridCell(position, points.Length - 1);
        }

        [BurstCompile]
        private bool IsPointValid(float3 point)
        {
            if (!SplineFunctions.IsInsideBounds(point, minBounds, maxBounds))
                return false;

            int2 gridPos = PositionToGridCoord(point);

            int xMin = math.max(gridPos.x - 2, 0);
            int xMax = math.min(gridPos.x + 2, gridResolution.x - 1);

            int yMin = math.max(gridPos.y - 2, 0);
            int yMax = math.min(gridPos.y + 2, gridResolution.y - 1);

            //Check cells around current grid cell (3x3)
            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    int index = (y * gridResolution.x + x);
                    int gridValue = grid[index];

                    if (gridValue > 0 && OutsideRadius(point, points[gridValue]))
                    {
                        return false;
                    }
                }
            }
            

            return true;
        }

        [BurstCompile]
        private int2 PositionToGridCoord(float3 pos)
        {
            return new int2((int)((pos.x - minBounds.x) / cellSize), (int)((pos.z - minBounds.z) / cellSize));
        }

        [BurstCompile]
        private void SetGridCell(float3 point, int pointIndex)
        {
            int2 cell = PositionToGridCoord(point);
            int index = cell.y * gridResolution.x + cell.x;

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (index > grid.Length)
            {
                Debug.LogError($"Grid Cell Index ({index}) out of bounds");
                return;
            }
            #endif

            grid[index] = pointIndex;
        }
            
        [BurstCompile]
        private float3 RandomPointOnAnnulus(float3 center)
        {
            //Random direction and distance
            int angleIndex = random.NextInt(0, searchAngles.Length);
            float angle = math.radians(searchAngles[angleIndex]) * 2f;

            //angle = random.NextFloat(0f, math.PI * 2f);
            float distance = random.NextFloat(spawnRadius, 2f * spawnRadius);
            float3 direction = new float3(math.cos(angle), 0f, math.sin(angle));
            
            return center + (direction * distance);
        }
        
        //Check if position falls within annulus
        [BurstCompile]
        private bool OutsideRadius(float3 center, float3 position)
        {
            float3 diff = center - position;
            return math.dot(diff, diff) < (spawnRadius * spawnRadius);

        }

        [BurstCompile]
        private SpawnPoint CreateSpawnPoint(float3 newPoint, float noise, int prefabIndex)
        {
            PrefabData data = prefabData[prefabIndex];

            newPoint.y = centerheight;
            
            SpawnPoint spawnPoint = new SpawnPoint
            {
                isValid = true,
                //spawnPoint.position = math.mul(splineTransform, new float4(newPoint, 1.0f)).xyz;
                prefabIndex = prefabIndex,
                position = newPoint,
                rotation = quaternion.identity,
                scale = data.gameObjectScale
            };

            spawnPoint.context = new SpawnPoint.Context()
            {
                noiseCoord = new float2(spawnPoint.position.x * 0.1f, spawnPoint.position.z * 0.1f),
                splineLength = splineLength,
                t = noise,
                curvature = 0,
                random01 = noise,
                forward = math.forward(),
                right = dirToSpline,
                up = math.up(),
                position = spawnPoint.position,
                invertDistance = true
            };

            return spawnPoint;
        }

        public void Dispose()
        {
            samples.Dispose();
            points.Dispose();
            grid.Dispose();
            searchAngles.Dispose();
        }
    }
}