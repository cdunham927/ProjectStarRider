// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace sc.splines.spawner.runtime
{
    [Serializable]
    [Modifier("Scale", "Scales objects uniformly, or on a specific axis")]
    public class Scale : Modifier
    {
        public enum Operation
        {
            Multiply,
            Add
        }
        public Operation operation = Operation.Multiply;
        
        public Vector3 axis = new Vector3(1f,1f,1f);
        public float uniform = 1f;
        
        //Randomization
        public float randomUniformMin = 0.8f;
        public float randomUniformMax = 1.2f;
        
        public Vector3 randomAxisMin = new Vector3(0.8f,0.8f,0.8f);
        public Vector3 randomAxisMax = new Vector3(1.2f,1.2f,1.2f);
        
        public float randomnessFrequency = 10f;
        
        //By spline distance
        public bool byCurveDistance;
        public bool invertCurveDistance;
        public Vector2 minMaxDistance = new Vector2(0f, 3f);
        [Min(0f)]
        public float distanceScaleMultiplier = 0.25f;

        public bool overCurveLength;
        public AnimationCurve scaleOverLength = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.1f));

        public enum ScaleMode
        {
            Uniform,
            PerAxis
        }
        public ScaleMode scaleMode = ScaleMode.Uniform;

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            private Operation operation;
            private ScaleMode scaleMode;
            
            private readonly float3 axis;
            private readonly float uniform;
            
            private float randomUniformMin;
            private float randomUniformMax;
            
            private readonly float3 randomAxisMin;
            private readonly float3 randomAxisMax;
            
            private readonly float randomnessFrequency;
            
            private bool byCurveDistance;
            private bool invertCurveDistance;
            private float2 minMaxDistance;
            private float distanceScaleMultiplier;

            private bool overCurveLength;
            private NativeCurve scaleOverLength;
            
            [NativeDisableParallelForRestriction]
            private NativeList<SpawnPoint> spawnPoints;
            
            public Job(Scale settings, ref NativeList<SpawnPoint> spawnPoints)
            {
                this.spawnPoints = spawnPoints;

                this.operation = settings.operation;
                this.scaleMode = settings.scaleMode;
                
                this.axis = settings.axis;
                this.uniform = settings.uniform;
                
                this.randomUniformMin = settings.randomUniformMin;
                this.randomUniformMax = settings.randomUniformMax;
                
                this.randomAxisMin = settings.randomAxisMin;
                this.randomAxisMax = settings.randomAxisMax;
                
                this.randomnessFrequency = settings.randomnessFrequency;
                
                this.byCurveDistance = settings.byCurveDistance;
                this.invertCurveDistance = settings.invertCurveDistance;
                this.minMaxDistance = settings.minMaxDistance;
                this.distanceScaleMultiplier = settings.distanceScaleMultiplier;
                
                this.overCurveLength = settings.overCurveLength;
                scaleOverLength = new NativeCurve(settings.scaleOverLength, Allocator.TempJob);
            }
            
            public void Execute(int i)
            {
                SpawnPoint spawnPoint = spawnPoints[i];
                
                //Skip any spawn points invalidated
                if(spawnPoint.isValid == false) return;
                
                SpawnPoint.Context context = spawnPoint.context;
                
                float2 noiseCoord = (context.noiseCoord) * randomnessFrequency;
                float noise = Unity.Mathematics.noise.cnoise(noiseCoord);
                float r = noise * 0.5f + 0.5f;

                float3 scale = scaleMode == ScaleMode.PerAxis ? axis : new float3(uniform);
                
                float3 random = scaleMode == ScaleMode.PerAxis ? 
                    math.lerp(randomAxisMin, randomAxisMax, r) : 
                    new float3(math.lerp(randomUniformMin, randomUniformMax, r));

                if (operation == Operation.Multiply)
                {
                    spawnPoint.scale *= scale * random;
                }
                else if (operation == Operation.Add)
                {
                    spawnPoint.scale += scale * random;
                }

                //Scale down by distance from spline
                if (byCurveDistance)
                {
                    float distance = math.distance(spawnPoint.position, context.position);
                    float distanceWeight = math.saturate((minMaxDistance.y - distance) / (minMaxDistance.y - minMaxDistance.x));

                    if(spawnPoint.context.invertDistance || invertCurveDistance) distanceWeight = 1f-distanceWeight;
                    
                    spawnPoint.scale *= math.lerp(distanceWeight, 1f, distanceScaleMultiplier);
                }

                if (overCurveLength)
                {
                    spawnPoint.scale *= math.max(0.05f, scaleOverLength.Sample(context.t));
                }
                
                spawnPoints[i] = spawnPoint;
            }
            
            public void Dispose()
            {
                scaleOverLength.Dispose();
            }
        }

        private Job job;
        public override JobHandle CreateJob(SplineSpawner spawner, ref NativeList<SpawnPoint> spawnPoints)
        {
            job = new Job(this, ref spawnPoints);
            
            JobHandle jobHandle = job.Schedule(spawnPoints.Length, DEFAULT_BATCHSIZE);

            return jobHandle;
        }

        public override void Dispose()
        {
            job.Dispose();
        }
    }
}