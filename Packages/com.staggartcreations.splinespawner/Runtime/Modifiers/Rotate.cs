// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;

namespace sc.splines.spawner.runtime
{
    [Serializable]
    [Modifier("Rotate", "Rotates objects along any given direction, or randomly")]
    public class Rotate : Modifier
    {
        [Tooltip("The rotation-space the rotation must be based on.")]
        public Space direction = Space.Object;
        public Vector3 rotation;
        
        public RandomMode randomMode = RandomMode.RandomBetween;
        public Vector3 randomMin;
        public Vector3 randomMax;
        public float randomnessFrequency = 10f;
        
        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            private Space direction;
            private readonly float3 rotation;
            private readonly RandomMode randomMode;
            private readonly float3 randomMin;
            private readonly float3 randomMax;

            private readonly float randomnessFrequency;
            
            [NativeDisableParallelForRestriction]
            private NativeList<SpawnPoint> spawnPoints;
            
            public Job(Rotate settings, ref NativeList<SpawnPoint> spawnPoints)
            {
                this.spawnPoints = spawnPoints;

                this.direction = settings.direction;
                this.rotation = settings.rotation;
                this.randomMode = settings.randomMode;
                this.randomMin = settings.randomMin;
                this.randomMax = settings.randomMax;
                this.randomnessFrequency = settings.randomnessFrequency;
            }

            public void Execute(int i)
            {
                SpawnPoint spawnPoint = spawnPoints[i];
                
                //Skip any spawn points invalidated
                if(spawnPoint.isValid == false) return;
                
                SpawnPoint.Context context = spawnPoint.context;
                
                float2 noiseCoord = (context.noiseCoord) * randomnessFrequency;
                float noise = Unity.Mathematics.noise.cnoise(noiseCoord) * 0.5f + 0.5f;

                float r = noise;
                
                if (randomMode == RandomMode.Alternate) r = (i/(int)math.max(1, randomnessFrequency)) % 2 == 0 ? 0 : 1;

                //Alternate every 'randomnessFrequency'
                //if (randomMode == RandomMode.Alternate) r = math.select(0f, 1f, (i / randomnessFrequency) % 2 == 0);
                
                //Square curve
                //if (randomMode == RandomMode.Alternate) r = (int)math.sign(math.sin(i % 2 == 0 ? 1 : 0 * math.PI * 2 * randomnessFrequency));
                
                float3 randomRotation = math.lerp(randomMin, randomMax, r);
                
                //Angle degrees per axis
                float angleX = randomRotation.x + rotation.x;
                angleX = math.radians(angleX);
                float angleY = randomRotation.y + rotation.y;
                angleY = math.radians(angleY);
                float angleZ = randomRotation.z + rotation.z;
                angleZ = math.radians(angleZ);
                
                //World-space
                quaternion baseRotation = quaternion.identity;
                float3 forward = math.forward();
                float3 right = math.right();
                float3 up = math.up();
                
                quaternion splineRotation = quaternion.LookRotationSafe(context.forward, context.up);
                
                if (direction == Space.SplineCurve)
                {
                    forward = context.forward;
                    right = context.right;
                    up = context.up;

                    baseRotation = splineRotation;
                }
                else if (direction == Space.Object)
                {
                    //Axis from previous rotation
                    //forward = math.mul(spawnPoint.rotation, context.forward);
                    //right = math.mul(spawnPoint.rotation, context.right);
                    //up = math.mul(spawnPoint.rotation, context.up);
                    
                    baseRotation = spawnPoint.rotation;
                }

                quaternion newRotation = baseRotation;
                
                newRotation = math.mul(baseRotation, quaternion.AxisAngle(right, angleX));
                newRotation = math.mul(newRotation, quaternion.AxisAngle(up, angleY));
                newRotation = math.mul(newRotation, quaternion.AxisAngle(forward, angleZ));
                
                spawnPoint.rotation = newRotation;
                
                spawnPoints[i] = spawnPoint;
            }
        }

        public override JobHandle CreateJob(SplineSpawner spawner, ref NativeList<SpawnPoint> spawnPoints)
        {
            Job job = new Job(this, ref spawnPoints);
            
            JobHandle jobHandle = job.Schedule(spawnPoints.Length, DEFAULT_BATCHSIZE);

            return jobHandle;
        }
    }
}