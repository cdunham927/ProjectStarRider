// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using Unity.Burst;
using UnityEngine;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting.APIUpdating;
using RaycastHit = UnityEngine.RaycastHit;

namespace sc.splines.spawner.runtime
{
    [Serializable]
    [MovedFrom(true, "sc.splines.spawner.runtime", sourceClassName: "Drop")]
    [Modifier("Snap to Colliders", "Finds colliders below the object and aligns the position/rotation to the surface")]
    public class SnapToColliders : Modifier
    {
        public enum Direction
        {
            StraightDown,
            SplineDirection,
            ObjectAlignment
        }
        public Direction direction;
        
        [Tooltip("Starting from the object's center, a raycast is shot down from this hit to check for a collider. Decrease the value if the objects are under something like a bridge")]
        public float rayHeightOffset = 10f;

        public LayerMask layerMask = -1;
        
        [Range(0f,1f)]
        public float positioning = 1f;
        public float heightOffset = 0f;
        
        [Range(0f,1f)]
        public float alignRotationX;
        public float alignRotationZ;
        
        [MinMaxSlider(0f, 90f)]
        public Vector2 slopeLimit = new Vector3(0f, 90f);

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            private Direction direction;
            private float heightOffset;
            private float positioning;
            private float alignRotationX;
            private float alignRotationZ;
            private float2 slopeLimit;
            
            [ReadOnly]
            NativeArray<RaycastHit> raycastHits;
            
            [NativeDisableParallelForRestriction]
            private NativeList<SpawnPoint> spawnPoints;
            
            public Job(SnapToColliders settings, ref NativeList<SpawnPoint> spawnPoints, NativeArray<RaycastHit> raycastHits)
            {
                this.spawnPoints = spawnPoints;
                this.raycastHits = raycastHits;
                
                this.direction = settings.direction;
                this.heightOffset = settings.heightOffset;
                this.positioning = settings.positioning;
                this.alignRotationX = settings.alignRotationX;
                this.alignRotationZ = settings.alignRotationZ;
                this.slopeLimit = settings.slopeLimit;
            }
            
            public void Execute(int i)
            {
                #if UNITY_2022_3_OR_NEWER
                RaycastHit hit = raycastHits[i];
                
                //Miss
                if(hit.distance <= 0) return;
                
                SpawnPoint spawnPoint = spawnPoints[i];
                
                //Skip any spawn points invalidated
                if(spawnPoint.isValid == false) return;
                
                SpawnPoint.Context context = spawnPoint.context;
                
                float3 up = direction == Direction.StraightDown ? Vector3.up : math.normalize(spawnPoints[i].context.up);
                if (direction == Direction.ObjectAlignment) up = math.rotate(spawnPoints[i].rotation, math.up()).xyz;
                float3 normal = up;
                
                if (alignRotationZ > 0 || alignRotationX > 0 ||  slopeLimit.x > 0f || slopeLimit.y < 90f)
                {
                    float surfaceAngle = math.acos(math.dot(hit.normal, up));
                    surfaceAngle = math.degrees(surfaceAngle);
                    
                    //Reject if slope check fails
                    if (surfaceAngle < slopeLimit.x || surfaceAngle > slopeLimit.y)
                    {
                        spawnPoint.isValid = false;
                    }
                    else
                    {
                        spawnPoint.isValid = true;
                        
                        //float3 forward = math.mul(spawnPoint.rotation, math.forward());
                        float3 forward = context.forward;
                        
                        quaternion surfaceRotationZ = quaternion.LookRotation(forward, hit.normal);
                        quaternion surfaceRotationX = quaternion.LookRotation(context.right, hit.normal);
                        surfaceRotationX = math.mul(quaternion.EulerXYZ(0, 0, 0), surfaceRotationX);
                        
                        spawnPoint.rotation = math.slerp(spawnPoint.rotation, surfaceRotationX, alignRotationX);
                        spawnPoint.rotation = math.slerp(spawnPoint.rotation, surfaceRotationZ, alignRotationZ);
                        
                        //normal = math.mul(spawnPoint.rotation, up);
                    }
                }

                float3 newPosition = (float3)hit.point - (normal * heightOffset);

                spawnPoint.position = math.lerp(spawnPoint.position, newPosition, positioning);
                
                spawnPoints[i] = spawnPoint;
                #endif
            }
        }

        private NativeArray<RaycastCommand> raycastCommands;
        private NativeArray<RaycastHit> raycastHits;
        
        public override JobHandle CreateJob(SplineSpawner spawner, ref NativeList<SpawnPoint> spawnPoints)
        {
            //QueryParameters only exists in later versions
            #if UNITY_2022_3_OR_NEWER
            int spawnPointCount = spawnPoints.Length;

            int maxColliderHits = 1;
            
            raycastCommands = new NativeArray<RaycastCommand>(spawnPointCount, Allocator.TempJob);
            raycastHits = new NativeArray<RaycastHit>(spawnPointCount * maxColliderHits, Allocator.TempJob);

            QueryParameters queryParams = new QueryParameters();
            queryParams.hitBackfaces = true;
            queryParams.layerMask = layerMask;
            queryParams.hitTriggers = QueryTriggerInteraction.Ignore;

            for (var i = 0; i < spawnPointCount; ++i)
            {
                float m_rayHeightOffset = spawnPoints[i].scale.y + rayHeightOffset;
                float maxDepth = m_rayHeightOffset + 100f;
                
                float3 rayDir = direction == Direction.StraightDown ? Vector3.down : -math.normalize(spawnPoints[i].context.up);
                float3 origin = spawnPoints[i].position - (rayDir * m_rayHeightOffset);
                
                RaycastCommand cmd = new RaycastCommand(origin, rayDir, queryParams, maxDepth);
                raycastCommands[i] = cmd;
            }
            
            JobHandle raycastJobHandle = RaycastCommand.ScheduleBatch(raycastCommands, raycastHits, 1, maxColliderHits, default(JobHandle));
            raycastJobHandle.Complete();
            raycastCommands.Dispose();
            
            Job job = new Job(this, ref spawnPoints, raycastHits);
            JobHandle jobHandle = job.Schedule(spawnPoints.Length, DEFAULT_BATCHSIZE, raycastJobHandle);

            return jobHandle;
            #else
            return default(JobHandle);
            #endif
        }

        public override void Dispose()
        {
            if(raycastHits.IsCreated) raycastHits.Dispose();
        }
    }
}