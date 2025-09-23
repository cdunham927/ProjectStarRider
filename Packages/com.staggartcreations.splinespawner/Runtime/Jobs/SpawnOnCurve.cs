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
    public struct SpawnOnCurve : IJob
    {
        //Input
        private NativeSpline spline;
        private float splineLength;
        private float4x4 splineTransform;
        
        [ReadOnly]
        private NativeList<PrefabData> prefabData;

        private bool useInstanceCount;
        private float instanceCount;
        
        //Settings
        private DistributionSettings.OnCurve.SpacingMode spacingMode;
        private float spacing;
        private float2 spacingMinMax;
        private float2 trimming;
        private DistributionSettings.RotateToFitAxis rotateToFitAxis;
        
        //Output
        [WriteOnly]
        public NativeList<SpawnPoint> spawnPoints;

        private float totalChanceWeights;
        Random random;
        
        public SpawnOnCurve(NativeSpline targetSpline, float4x4 localToWorld, DistributionSettings distributionSettings, NativeList<PrefabData> prefabData, ref NativeList<SpawnPoint> spawnPoints)
        {
            DistributionSettings.OnCurve settings = distributionSettings.onCurve;
            this.spline = targetSpline;
            this.splineTransform = localToWorld;
            this.splineLength = spline.GetLength();
            
            this.prefabData = prefabData;
            this.spawnPoints = spawnPoints;

            totalChanceWeights = SplineFunctions.CalculateProbabilitySum(prefabData);

            this.spacingMode = settings.spacingMode;
            this.spacing = Mathf.Max(-5, settings.spacing);
            this.spacingMinMax.x = Mathf.Max(-5f, settings.spacingMinMax.x);
            this.spacingMinMax.y = Mathf.Max(settings.spacingMinMax.x, settings.spacingMinMax.y);
            this.trimming = settings.startEndTrimming;

            this.rotateToFitAxis = settings.rotateToFitAxis;
            
            useInstanceCount = settings.instanceCountMode == DistributionSettings.InstanceCountMode.Specific;
            instanceCount = Mathf.Max(1, settings.instanceCount);

            random = new Random(distributionSettings.GetSeed());
        }

        
        public void Execute()
        {
            float trimLength = (trimming.x + trimming.y);
            splineLength -= trimLength;

            if (splineLength < 1f) return;
            
            //T-values of the trimming
            float2 trimRange = new float2((trimming.x / splineLength), 1f - (trimming.y / splineLength));
            
            float distanceTraveled = 0f;
            float m_spacing = spacing;
            
            //Instance count mode
            if(useInstanceCount) m_spacing = splineLength / (float)instanceCount;

            //Set a starting value, otherwise an infinite loop may occur if only 1 prefab is used with a very low probability.
            float lastOffset = 0;
            //distanceTraveled += lastOffset * 0.5f;
            float lengthAlongSpline = 0;
            
            uint i = 0;
            while (distanceTraveled < splineLength)
            {
                float t = distanceTraveled / this.splineLength;
                
                t = math.clamp(t, 0.00001f, 0.99999f); //Ensure a tangent can always be derived
                //Remap normalized (0-1) t-range to trimmed range
                t = math.lerp(trimRange.x, trimRange.y, t);
 
                //Stable per instance
                float r = random.NextFloat(0f, 1f);

                int prefabIndex = SplineFunctions.GetRandomPrefabIndex(r, totalChanceWeights, prefabData);
                
                
                if (prefabIndex >= 0)
                {
                    PrefabData prefab = this.prefabData[prefabIndex];

                    if (spacingMode == DistributionSettings.OnCurve.SpacingMode.RandomBetween)
                    {
                        m_spacing = random.NextFloat(spacingMinMax.x, spacingMinMax.y);
                    }
                    lengthAlongSpline = prefab.GetObjectLength() + m_spacing;

                    //Current prefab no longer fits on the spline, end here
                    if ((splineLength - distanceTraveled) < lengthAlongSpline) return;
                    
                    float offset = 0;
                    /*
                    if (prefab.pivot == SplineSpawner.SpawnableObject.Pivot.Back)
                    {
                        offset += -(prefab.GetObjectBoundsMin());
                    }
                    else if (prefab.pivot == SplineSpawner.SpawnableObject.Pivot.Center)
                    {
                        offset += -prefab.GetForwardPivotOffset();
                    }
                    */
                    
                    float offsetT = (offset / splineLength);

                    //Spline sampling
                    spline.Evaluate(math.clamp(t + offsetT, 0.00001f, 0.99999f), out float3 position, out float3 tangent, out float3 up);
                    float3 forward = math.normalize(tangent);
                    float3 right = math.normalize(math.cross(forward, up));

                    //position += forward * (prefab.GetObjectLength() * 0.5f);
                    
                    SpawnPoint spawnPoint = new SpawnPoint
                    {
                        isValid = true,
                        prefabIndex = prefabIndex,
                        position = position,
                        pivotOffset = prefab.GetPivotOffset(),
                        scale = prefab.gameObjectScale
                    };

                    float stride = lengthAlongSpline/splineLength;

                    //Calculate the turning factor
                    float3 currentTangentXZ = tangent;
                    float3 nextTangentXZ = spline.EvaluateTangent(t + offsetT + stride);
                    
                    currentTangentXZ = math.normalize(currentTangentXZ);
                    nextTangentXZ = math.normalize(nextTangentXZ);
                    float3 cross = (math.cross(currentTangentXZ, nextTangentXZ));
  
                    spawnPoint.context = new SpawnPoint.Context
                    {
                        t = t,
                        //Scale the noise, otherwise minute frequency values become the norm
                        noiseCoord = new float2(t * splineLength + r, t * splineLength),
                        splineLength = splineLength,
                        random01 = r,
                        curvature = math.abs(math.degrees(math.acos(cross.y)) - 90f),
                        position = position,
                        forward = forward,
                        right = right,
                        up = up,
                        invertDistance = false
                    };
                    
                    //spawnPoint.rotation = prefab.GetForwardRotation(forward, right, up);
                    spawnPoint.rotation = quaternion.LookRotationSafe(spawnPoint.context.forward, spawnPoint.context.up);

                    //Calculate the rotation needed to position the object so that both its tip and end sit on the spline
                    //Particularly useful for fences or other long objects
                    if (rotateToFitAxis != DistributionSettings.RotateToFitAxis.Disabled)
                    {
                        float3 startPosition = position;
                        
                        float endT = t + offsetT + stride;
                        float3 endPosition = spline.EvaluatePosition(endT);

                        //SplineUtility.GetNearestPoint(spline, endPosition, out endPosition, out var _, SplineUtility.PickResolutionDefault, 1);

                        float3 delta = endPosition - startPosition;
                        //Calculate direction from start to end position
                        float3 direction = math.normalize(delta);
                        spawnPoint.context.forward = direction;
                        
                        //Full rotation (all axis)
                        quaternion rotation = quaternion.LookRotationSafe(direction, up);

                        if (rotateToFitAxis == DistributionSettings.RotateToFitAxis.Y)
                        {
                            //Extract the Y-axis rotation, so that can be set to negate other axis
                            float yAngle = GetYawDegrees(rotation);
                            rotation = quaternion.AxisAngle(up, math.radians(yAngle));
                        }

                        //spawnPoint.rotation = math.mul(spawnPoint.rotation, rotation);
                        spawnPoint.rotation = rotation;

                        //Recalculate occupied length
                        lengthAlongSpline = math.length(delta);
                    	
                    }

                    spawnPoints.Add(spawnPoint);

                }

                //Ensure that the distance is always incremented
                lastOffset =  math.max(0.02f, lengthAlongSpline);
                
                distanceTraveled += lastOffset;
                i++;
            }
        }
        
        float GetYawDegrees(quaternion q)
        {
            q = math.normalize(q);

            float siny_cosp = 2f * (q.value.w * q.value.y + q.value.z * q.value.x);
            float cosy_cosp = 1f - 2f * (q.value.y * q.value.y + q.value.x * q.value.x);

            float yaw = math.atan2(siny_cosp, cosy_cosp); // Radians
            return math.degrees(yaw);
        }

        private float3 TransformToWorld(float3 direction)
        {
            return math.normalize(math.mul(splineTransform, new float4(direction, 0.0f)).xyz);
        }

        private float RotatedLengthOnSpline(quaternion rotation, float3 scale, float3 forward, float3 right, float3 up)
        {
            //Rotate the local axes
            float3 globalAxisX = math.mul(rotation, right * scale.x);
            float3 globalAxisY = math.mul(rotation, up * scale.y);
            float3 globalAxisZ = math.mul(rotation, forward * scale.z);
                
            float length = math.abs(globalAxisX.z) + math.abs(globalAxisY.z) + math.abs(globalAxisZ.z);

            return length;
        }

        private quaternion RotateForward(SplineSpawner.SpawnableObject.ForwardDirection direction, float3 up)
        {
            float3 forward;

            switch (direction)
            {
                case SplineSpawner.SpawnableObject.ForwardDirection.PositiveX:
                    forward = math.right();
                    break;
                case SplineSpawner.SpawnableObject.ForwardDirection.NegativeX:
                    forward = -math.right();
                    break;
                case SplineSpawner.SpawnableObject.ForwardDirection.PositiveY:
                    forward = math.up();
                    break;
                case SplineSpawner.SpawnableObject.ForwardDirection.NegativeY:
                    forward = -math.up();
                    break;
                case SplineSpawner.SpawnableObject.ForwardDirection.PositiveZ:
                    forward = math.forward();
                    break;
                case SplineSpawner.SpawnableObject.ForwardDirection.NegativeZ:
                    forward = -math.forward();
                    break;
                default:
                    forward = math.forward(); // Default to +Z
                    break;
            }

            // Use up vector as Y+ unless forward is also Y+/- to avoid zero-cross issue
            //float3 up = math.abs(math.dot(forward, math.up())) > 0.99f ? math.forward() : math.up();
            return quaternion.LookRotationSafe(forward, up);
        }

        public void Dispose()
        {
            //spawnPoints.Dispose();
        }
    }
}