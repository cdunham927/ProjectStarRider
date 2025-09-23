// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = Unity.Mathematics.Random;
using Selection = sc.splines.spawner.runtime.DistributionSettings.OnKnots.Selection;

namespace sc.splines.spawner.runtime
{
    [BurstCompile]
    public struct SpawnOnKnots : IJob
    {
        //Input
        private NativeSpline spline;
        private float splineLength;
        private float4x4 splineTransform;
        private float3 splineBoundsMax;
        private float3 splineBoundsMin;

        private Selection mode;
        private bool linearOnly;
        private bool mirrorLastRotation;
        private Vector2Int range;
        private DistributionSettings.OnKnots.LinkedKnotFilter linkedKnotFilter;
        
        [ReadOnly]
        private NativeList<PrefabData> prefabData;
        [ReadOnly]
        NativeArray<BezierKnot> bezierKnots;
        int knotCount;
        
        private Random random;
        private float totalChanceWeights;
        
        //Output
        [WriteOnly]
        public NativeList<SpawnPoint> spawnPoints;

        public SpawnOnKnots(NativeSpline targetSpline, SplineContainer container, int splineIndex, float4x4 localToWorld, DistributionSettings distributionSettings, NativeList<PrefabData> prefabData, ref NativeList<SpawnPoint> spawnPoints)
        {
            this.spline = targetSpline;
            this.splineTransform = localToWorld;
            this.splineLength = spline.GetLength();
            this.spawnPoints = spawnPoints;
            this.prefabData = prefabData;

            DistributionSettings.OnKnots settings = distributionSettings.onKnots;
            this.mode = settings.selection;
            this.linearOnly = settings.linearOnly;
            this.mirrorLastRotation = settings.mirrorLastRotation;
            this.range = settings.range;
            this.range.x = Mathf.Max(this.range.x, 0);
            this.range.y = Mathf.Min(this.range.y, spline.Count);
            this.linkedKnotFilter = settings.linkedKnotFilter;

            knotCount = spline.Count;
            
            int startIndex = 0;
            int endIndex = knotCount;
            int[] knotIndices = new int[knotCount];

            if (mode == Selection.All)
            {
                knotIndices = new int[knotCount];
                
                for (int i = startIndex; i < endIndex; i++)
                {
                    knotIndices[i] = i;
                }
            }
            else if (mode == Selection.FirstOnly)
            {
                knotCount = 1;
                Array.Resize(ref knotIndices, knotCount);
                
                knotIndices[0] = 0;
            }
            else if (mode == Selection.LastOnly)
            {
                knotCount = 1;
                Array.Resize(ref knotIndices, knotCount);
                
                knotIndices[0] = spline.Count-1;
            }
            else if (mode == Selection.FirstAndLast)
            {
                knotCount = 2;
                Array.Resize(ref knotIndices, knotCount);
                
                knotIndices[0] = 0;
                knotIndices[^1] = spline.Count-1;
            }
            else if (mode == Selection.AllExceptFirstAndLast)
            {
                knotCount = spline.Count-2;
                Array.Resize(ref knotIndices, knotCount);
                
                startIndex = 1;
                endIndex = spline.Count-1;
                
                int index = 0;
                for (int i = startIndex; i < endIndex; i++)
                {
                    knotIndices[index++] = i;
                }
            }
            else if (mode == Selection.Range)
            {
                knotCount = range.y - range.x;
                Array.Resize(ref knotIndices, knotCount);

                startIndex = range.x;
                endIndex = range.y;

                int index = 0;
                for (int i = startIndex; i < endIndex; i++)
                {
                    knotIndices[index++] = i;
                }
            }

            knotCount = knotIndices.Length;
            List<BezierKnot> knotList = new List<BezierKnot>();
            for (int i = 0; i < knotCount; i++)
            {
                int index = knotIndices[i];
                BezierKnot knot = spline.Knots[index];

                if (mirrorLastRotation && i == knotCount - 1)
                {
                    knot.Rotation = math.mul(knot.Rotation, quaternion.RotateY(math.radians(180f)));
                }

                if (linearOnly)
                {
                    if (math.length(knot.TangentIn) > 0.02f && math.length(knot.TangentOut) > 0.02f)
                    {
                        continue;
                    }
                }

                if (linkedKnotFilter != DistributionSettings.OnKnots.LinkedKnotFilter.None)
                {
                    if (container.KnotLinkCollection.TryGetKnotLinks(new SplineKnotIndex(splineIndex, index), out var linkedKnots))
                    {
                        bool isLinked = linkedKnots.Count > 0;
                        
                        //Linked, but unwanted
                        if (isLinked && linkedKnotFilter == DistributionSettings.OnKnots.LinkedKnotFilter.Exclude)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        //Not linked, but wanting only linked ones
                        if (linkedKnotFilter == DistributionSettings.OnKnots.LinkedKnotFilter.Exclusive)
                        {
                            continue;
                        }
                    }
                }
                
                knotList.Add(knot);
            }
            bezierKnots = new NativeArray<BezierKnot>(knotList.Count, Allocator.TempJob);
            bezierKnots.CopyFrom(knotList.ToArray());
            knotCount = bezierKnots.Length;
            
            Bounds bounds = spline.GetBounds(splineTransform);
            splineBoundsMin = bounds.min;
            splineBoundsMax = bounds.max;

            random = new Random(distributionSettings.GetSeed());
            
            totalChanceWeights = SplineFunctions.CalculateProbabilitySum(prefabData);
        }
        
        public void Execute()
        {
            for (int i = 0; i < knotCount; i++)
            {
                float t = (float)i / (knotCount);
                
                //Stable per instance
                float r = random.NextFloat(0f, 1f);
                
                int prefabIndex = SplineFunctions.GetRandomPrefabIndex(r, totalChanceWeights, this.prefabData);

                if (prefabIndex >= 0)
                {
                    PrefabData prefabData = this.prefabData[prefabIndex];

                    BezierKnot knot = bezierKnots[i];

                    SpawnPoint spawnPoint = new SpawnPoint
                    {
                        isValid = true,
                        prefabIndex = prefabIndex,
                        position = knot.Position,
                        rotation = knot.Rotation,
                        scale = prefabData.gameObjectScale
                    };

                    spawnPoint.context = new SpawnPoint.Context
                    {
                        t = t,
                        noiseCoord = new float2((splineBoundsMin.x + spawnPoint.position.x) / splineBoundsMax.x, (splineBoundsMin.z + spawnPoint.position.z) / splineBoundsMax.z),
                        splineLength = splineLength,
                        random01 = r,
                        position = spawnPoint.position,
                        forward = math.mul(knot.Rotation, math.forward()),
                        right = math.mul(knot.Rotation, math.right()),
                        up = math.mul(knot.Rotation, math.up()),
                        invertDistance = false
                    };

                    spawnPoints.Add(spawnPoint);
                }
            }

        }

        public void Dispose()
        {
            if(bezierKnots.IsCreated) bezierKnots.Dispose();
        }
    }
}