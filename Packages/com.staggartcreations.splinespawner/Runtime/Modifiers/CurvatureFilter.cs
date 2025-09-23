// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace sc.splines.spawner.runtime
{
    [Serializable]
    [Modifier("Curvature Filter", "Restricts object to a areas on the curve that are curved or straight enough", 
        new [] { DistributionSettings.DistributionMode.Grid, DistributionSettings.DistributionMode.OnKnots, DistributionSettings.DistributionMode.Radial, DistributionSettings.DistributionMode.InsideArea })]
    public class CurvatureFilter : Modifier
    {
        public float minAngle;
        public float maxAngle = 90f;

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            private readonly float minAngle;
            private readonly float maxAngle;
            
            [NativeDisableParallelForRestriction]
            private NativeList<SpawnPoint> spawnPoints;

            public Job(CurvatureFilter settings, ref NativeList<SpawnPoint> spawnPoints)
            {
                this.spawnPoints = spawnPoints;
                
                this.minAngle = settings.minAngle;
                this.maxAngle = settings.maxAngle;
            }

            public void Execute(int i)
            {
                SpawnPoint spawnPoint = spawnPoints[i];
                                
                //Skip any spawn points invalidated
                if(spawnPoint.isValid == false) return;
                
                SpawnPoint.Context context = spawnPoint.context;
                
                if (context.curvature < minAngle || context.curvature > maxAngle) spawnPoint.isValid = false;
                
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