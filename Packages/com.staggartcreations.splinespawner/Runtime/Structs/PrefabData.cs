// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;
using ForwardDirection = sc.splines.spawner.runtime.SplineSpawner.SpawnableObject.ForwardDirection;

namespace sc.splines.spawner.runtime
{
    //Unmanaged version of prefab information
    public struct PrefabData
    {
        public int index;
        public SplineSpawner.SpawnableObject.Pivot pivot;
        public ForwardDirection forwardDirection;
        
        public float probability;
        public float distanceSelectionWeight;
        
        public float3 boundsMin;
        public float3 boundsMax;
        public float3 boundsSize;
        public float3 gameObjectScale;
        
        public float3 pivotOffset;

        public static PrefabData Create(SplineSpawner.SpawnableObject input, int index)
        {
            PrefabData pd = new PrefabData();
            pd.pivot = input.pivot;
            pd.forwardDirection = input.forwardDirection;
            pd.probability = input.probability;
            pd.distanceSelectionWeight = input.selectBySplineDistance;
            pd.index = index;

            if (input.prefab)
            {
                pd.CalculateBoundsSize(input.prefab);

                pd.gameObjectScale = input.prefab.transform.localScale;
                pd.gameObjectScale *= input.baseScale;
                
                if (math.length(pd.boundsSize * pd.gameObjectScale) < 0.05f)
                {
                    throw new WarningException($"Prefab bounds size of \"{input.prefab.name}\" is less than 0.05m! Check that no Meshes are missing");
                }
            }

            return pd;
        }
        
        /// <summary>
        /// Automatically grab a starting point for the area size, based on the attached mesh(es) or collider(s)
        /// </summary>
        public void CalculateBoundsSize(GameObject target)
        {
            //Default size, considering that the prefab doesn't have any geometry or colliders
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.5f);
            
            Vector3 minSum = Vector3.one * Mathf.Infinity;
            Vector3 maxSum = Vector3.one * Mathf.NegativeInfinity;

            //If a LOD group is present, consider LOD0 as the target object
            LODGroup lodGroup = target.GetComponent<LODGroup>();
            if (lodGroup && lodGroup.lodCount > 0)
            {
                LOD[] lods = lodGroup.GetLODs();

                if (lods[0].renderers.Length > 0)
                {
                    Renderer lod0 = lods[0].renderers[0];
                    target = lod0.gameObject;
                }
            }

            MeshFilter[]  meshes = target.GetComponentsInChildren<MeshFilter>();

            int meshCount = meshes.Length;
            if (meshCount > 0)
            {
                for (int i = 0; i < meshCount; i++)
                {
                    if (meshes[i].sharedMesh == null)
                    {
                        Debug.LogError($"Object \"{target.name}\" has one or more missing meshes. Failed to calculate bounds size.", target);
                        return;
                    }

                    minSum = Vector3.Min(minSum, meshes[i].sharedMesh.bounds.min);
                    maxSum = Vector3.Max(maxSum, meshes[i].sharedMesh.bounds.max);
                }
                bounds.SetMinMax(minSum, maxSum);
            }
            else
            {
                Collider[] colliders = target.GetComponentsInChildren<Collider>();

                if (colliders.Length > 0)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].isTrigger) continue;

                        minSum = Vector3.Min(minSum, colliders[i].bounds.min);
                        maxSum = Vector3.Max(maxSum, colliders[i].bounds.max);
                    }
                    bounds.SetMinMax(minSum, maxSum);
                }
            }
            
            boundsMin = bounds.min;
            boundsMax = bounds.max;
            boundsSize = bounds.size;
            pivotOffset = -(float3)bounds.center;
        }
        
        private int GetLengthAxis()
        {
            return (int)forwardDirection / 2;
        }

        public float GetForwardPivotOffset()
        {
            int component = GetLengthAxis();
            
            return pivotOffset[component];
        }
        
        public float3 GetPivotOffset()
        {
            float3 m_pivotOffset = pivotOffset;
            
            if (forwardDirection == ForwardDirection.PositiveZ || forwardDirection == ForwardDirection.NegativeZ)
            {
                m_pivotOffset *= math.forward();
            }

            //etc..
            
            return m_pivotOffset;
        }

        public float GetObjectBoundsMin()
        {
            int component = GetLengthAxis();
            
            return boundsMin[component];
        }
        
        public float GetObjectLength()
        {
            int component = GetLengthAxis();
            
            return (boundsSize[component]) * gameObjectScale[component];
        }

        public float GetRadiusXZ()
        {
            return math.max(boundsSize.x * gameObjectScale.x, boundsSize.z * gameObjectScale.z);
        }

        public quaternion GetForwardRotation(float3 forward, float3 right, float3 up)
        {
            if (forwardDirection == ForwardDirection.PositiveX || forwardDirection == ForwardDirection.NegativeX)
            {
                return quaternion.LookRotation(right, up);
            }
            if (forwardDirection == ForwardDirection.PositiveY || forwardDirection == ForwardDirection.NegativeY)
            {
                return quaternion.LookRotation(up, forward);
            }
            if (forwardDirection == ForwardDirection.PositiveZ || forwardDirection == ForwardDirection.NegativeZ)
            {
                return quaternion.LookRotation(forward, up);
            }
            
            return quaternion.identity;
        }
    }
}