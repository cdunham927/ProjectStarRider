// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace sc.splines.spawner.runtime
{
    public partial class SplineSpawner
    {
        [Serializable]
        public class MaskRule
        {
            [SplineSpawnerMask.MaskLayer(false, false)]
            public int layer;
            public bool invert;

            public float minDistance = 1f;
            [Min(0.01f)]
            public float falloff = 0.01f;

            public MaskRule(int layer)
            {
                this.layer = layer;
            }
        }
        public MaskRule[] maskRules = Array.Empty<MaskRule>();
        
        private void ProcessMasks()
        {
            int maskCount = SplineSpawnerMask.Instances.Count;
            int spawnPointCount = spawnPoints.Length;
            
            if (maskCount > 0 && spawnPointCount > 0)
            {
                //Compose a list of spawn positions
                Vector3[] spawnPositions = new Vector3[spawnPointCount];
                for (int i = 0; i < spawnPointCount; i++)
                {
                    spawnPositions[i] = spawnPoints[i].position;
                }

                //Go over each configured masking rule
                for (int i = 0; i < maskRules.Length; i++)
                {
                    MaskRule maskSetting = maskRules[i];

                    for (int j = 0; j < SplineSpawnerMask.Instances.Count; j++)
                    {
                        SplineSpawnerMask splineMask = SplineSpawnerMask.Instances[j];

                        if (splineMask.IsOnLayer(maskSetting.layer))
                        {
                            //TODO: Check intersection with spawner's spline bounds first

                            if (splineMask.RequiresUpdate())
                            {
                                splineMask.RenderSDFIfNeeded();
                            }

                            RemoveMaskedSpawnPoints(spawnPoints, spawnPositions, splineMask, maskSetting);
                        }
                    }
                }
            }
        }
        
        private void RemoveMaskedSpawnPoints(NativeList<SpawnPoint> points, Vector3[] spawnPositions, SplineSpawnerMask splineMask, MaskRule maskRule)
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(1234); // or seed per frame

            float[] maskValues = splineMask.SampleSDF(spawnPositions);
            
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                float sample = maskValues[i];
                sample -= maskRule.minDistance;
                
                if(maskRule.invert) sample = -sample;
                
                //TODO: Check distance for every corner of the bounds, rather than just the pivot point
                
                bool remove = sample == 0f;

                //Gradient values, use randomization as a means to thin out the density
                if (remove == false)
                {
                    float removalChance = math.saturate(1f - (sample / maskRule.falloff));
                    remove = rng.NextFloat(0f, 1f) < removalChance;
                }

                if (remove)
                {
                    SpawnPoint point = spawnPoints[i];
                    point.isValid = false;
                    points[i] = point;
                }
            }
        }
    }
}