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
    [Modifier("Grid Snap", "Snaps the position of the object to a virtual grid")]
    public class SnapToGrid : Modifier
    {
        [Min(0f)]
        public float gridSize = 1f;

        public Axis axis = Axis.X | Axis.Y | Axis.Z;

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            public float gridSize;
            public Axis axis;
            
            [NativeDisableParallelForRestriction]
            private NativeList<SpawnPoint> spawnPoints;

            public Job(SnapToGrid settings, ref NativeList<SpawnPoint> spawnPoints)
            {
                this.gridSize = Mathf.Max(0.01f, settings.gridSize);
                this.axis = settings.axis;
                this.spawnPoints = spawnPoints;
            }
            
            private bool IsAxisEnabled(Axis flag)
            {
                return (axis & flag) == flag;
            }
            
            private float3 SnapToGrid(float3 position, float cellSize)
            {
                return new float3(
                    IsAxisEnabled(Axis.X) ? SnapToGrid(position.x, cellSize) : position.x, 
                    IsAxisEnabled(Axis.Y) ? SnapToGrid(position.y, cellSize) : position.y, 
                    IsAxisEnabled(Axis.Z) ? SnapToGrid(position.z, cellSize) : position.z
                    );
            }

            private float SnapToGrid(float position, float cellSize)
            {
                return math.floor(position / cellSize) * (cellSize) + (cellSize * 0.5f);
            }
            
            public void Execute(int i)
            {
                SpawnPoint spawnPoint = spawnPoints[i];
                
                //Skip any spawn points invalidated
                if(spawnPoint.isValid == false) return;
                
                spawnPoint.position = SnapToGrid(spawnPoint.position, gridSize);

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