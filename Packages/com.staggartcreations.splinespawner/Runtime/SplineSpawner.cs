// Spline Spawner by Staggart Creations (http://staggart.xyz)
// COPYRIGHT PROTECTED UNDER THE UNITY ASSET STORE EULA (https://unity.com/legal/as-terms)
//  • Copying or referencing source code for the production of new asset store, or public, content is strictly prohibited!
//  • Uploading this file to a public GitHub repository will subject it to an automated DMCA takedown request.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Profiling;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

namespace sc.splines.spawner.runtime
{
    [ExecuteAlways]
    [Icon(SplineSpawner.kPackageRoot + "/Editor/Resources/spline-spawner-icon-64px.psd")]
    [SelectionBase] //Select this object when selecting spawned objects instead
    [HelpURL("https://staggart.xyz/support/documentation/spline-spawner/")]
    public partial class SplineSpawner : MonoBehaviour
    {
        public const string VERSION = "1.0.0";

        public const string kPackageRoot = "Packages/com.staggartcreations.splinespawner";
        public const int CAPACITY = 8192;
        private const string PROFILER_PREFIX = "Spline Spawner: ";

        [Tooltip("The spline container that defines the curves for spawning")]
        public SplineContainer splineContainer;

        [Tooltip("The root transform under which spawned objects will be placed")]
        public Transform root;

        [Tooltip("Whether to hide spawned instances in the hierarchy")]
        public bool hideInstances;

        [Serializable]
        //Managed, front end
        public class SpawnableObject
        {
            public GameObject prefab;

            public enum Pivot
            {
                Original,
                Center,
                Back
            }
            public Pivot pivot;
            public ForwardDirection forwardDirection = ForwardDirection.PositiveZ;

            [Min(0)]
            public float selectBySplineDistance;
            [Range(0f, 100f)]
            public float probability = 100f;
            public Vector3 baseScale = Vector3.one;

            [NonSerialized]
            public Texture thumbnail;

            public SpawnableObject(GameObject prefab, float probability = 100f)
            {
                this.prefab = prefab;
                this.probability = probability;
            }

            public SpawnableObject() { }

            public enum ForwardDirection
            {
                [InspectorName("+X")]
                PositiveX,
                [InspectorName("-X")]
                NegativeX,
                [InspectorName("+Y")]
                PositiveY,
                [InspectorName("-Y")]
                NegativeY,
                [InspectorName("+Z")]
                PositiveZ,
                [InspectorName("-Z")]
                NegativeZ,
            }
        }

        [UnityEngine.Serialization.FormerlySerializedAs("prefabs")]
        [Tooltip("List of prefab objects to spawn along the spline")]
        public List<SpawnableObject> inputObjects = new List<SpawnableObject>();

        /// <summary>
        /// Settings for all of the distribution modes. This class contains several sub-classes, one for each mode.
        /// </summary>
        public DistributionSettings distributionSettings = new DistributionSettings();
        
        [Tooltip("List of modifiers that affect the spawned objects")]
        [SerializeReference]
        public List<Modifier> modifiers = new List<Modifier>();

        [SerializeField]
        private List<SplineInstanceContainer> containers = new List<SplineInstanceContainer>();

        public int InstanceCount { get; private set; }

        private NativeList<PrefabData> prefabData;
        private NativeList<SpawnPoint> spawnPoints;

#pragma warning disable CS0067
        public delegate void Action(SplineSpawner instance, int splineIndex);

        /// <summary>
        /// Pre- and post-respawn callbacks. The instance being passed is the Spline Spawner being respawned.
        /// </summary>
        public static event Action onPreRespawn, onPostRespawn;

        public delegate void SpawnPointModifyEvent(NativeList<SpawnPoint> spawnPoints);
        public event SpawnPointModifyEvent onAfterDistribution, onAfterModifiers;
#pragma warning restore CS0067

        /// <summary>
        /// Disable only when using the created spawn points and instantiating objects using custom code!
        /// </summary>
        [Tooltip("Whether to actually spawn GameObjects (disable to use spawn points only)")]
        public bool spawnObjects = true;

        public void CreatePrefabData()
        {
            if (prefabData.IsCreated) prefabData.Dispose();

            prefabData = new NativeList<PrefabData>(Allocator.Persistent);

            for (int i = 0; i < inputObjects.Count; i++)
            {
                SpawnableObject input = inputObjects[i];

                PrefabData pd = PrefabData.Create(input, i);

                if (input.prefab)
                {
                    prefabData.Add(pd);
                }
            }
        }

        private void Reset()
        {
            splineContainer = GetComponentInParent<SplineContainer>();
            root = this.transform;

            Validate();

            InstanceCount = 0;
        }

        private partial void SubscribeSplineCallbacks();

        private partial void UnsubscribeSplineCallbacks();

        private void OnEnable()
        {
            SubscribeSplineCallbacks();
        }

        private void OnDisable()
        {
            UnsubscribeSplineCallbacks();

            Dispose();
        }

        public void Validate()
        {
            ValidateContainers();
        }

        private SpawnOnCurve spawnOnCurveJob;
        private SpawnInArea spawnInSplineJob;
        private SpawnOnKnots spawnOnKnotsJob;
        private RadialInsideSpline radialJob;
        private SpawnOnGrid gridJob;

        private JobHandle spawnPointJobHandle;

        private readonly System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        public float lastDistributionTime { get; private set; }
        public float lastMaskingTime { get; private set; }
        public float lastModifierStackTime { get; private set; }
        public float lastInstantiateTime { get; private set; }

        public float LastRespawnTime => lastDistributionTime + lastMaskingTime + lastModifierStackTime + lastInstantiateTime;

        /// <summary>
        /// Respawns from all splines using the current settings
        /// </summary>
        public void Respawn()
        {
            if (!splineContainer) return;

            splineCount = splineContainer.Splines.Count;

            //If the container transform was altered, the cached native splines will be required to update
            if (splineCount != nativeSplines.Count || splineContainer.transform.hasChanged)
            {
                splineContainer.transform.hasChanged = false;
                
                RebuildSplineCache();
            }

            for (int splineIndex = 0; splineIndex < splineCount; splineIndex++)
            {
                Respawn(splineIndex);
            }
        }

        /// <summary>
        /// Respawn using a specific spline
        /// </summary>
        /// <param name="splineIndex"></param>
        public void Respawn(int splineIndex)
        {
            //if (splineIndex >= containers.Count)
            {
                //Debug.LogError($"No container present for spline #{splineIndex}.");
                Validate();
                //return;
            }

            onPreRespawn?.Invoke(this, splineIndex);
            
            SplineInstanceContainer container = containers[splineIndex];

            #if UNITY_EDITOR
            var flags = GameObjectUtility.GetStaticEditorFlags(this.gameObject);
            GameObjectUtility.SetStaticEditorFlags(container.gameObject, flags);
            #endif

            if (inputObjects.Count == 0)
            {
                Debug.LogWarning("Cannot spawn anything, 0 prefabs assigned...", this);
                return;
            }

            if (spawnPointJobHandle.IsCompleted == false)
            {
                Debug.LogWarning("Previous spawning job hasn't completed yet...", this);
                return;
            }

            Profiler.BeginSample($"{PROFILER_PREFIX} Setup");
            {
                InstanceCount -= container.InstanceCount;
                InstanceCount = Mathf.Max(InstanceCount, 0);

                //Delete current first, to ensure no colliders are in the way
                container.DestroyInstances();

                CreatePrefabData();

                //No prefab objects assigned
                if (prefabData.Length == 0)
                {
                    Debug.LogWarning($"None of the {inputObjects.Count} assigned objects were suitable for spawning. Either no prefabs were assigned, or they are miniscule in size...", this);
                    Profiler.EndSample();

                    return;
                }

                if (spawnPoints.IsCreated == false)
                {
                    spawnPoints = new NativeList<SpawnPoint>(CAPACITY, Allocator.Persistent);
                }
                spawnPoints.Clear();
            }
            Profiler.EndSample();

            #region Distribution
            stopWatch.Restart();
            int knotCount = splineContainer.Splines[splineIndex].Count;

            Profiler.BeginSample($"{PROFILER_PREFIX} Distribution ({distributionSettings.mode})");
            {
                NativeBounds splineBounds = nativeSplines[splineIndex].GetNativeBounds(2f);
                //Debug.Log($"Calculate bounds: Center: {splineBounds.center}, Size: {splineBounds.size}. Length: {nativeSplines[splineIndex].GetLength()}");

                if (distributionSettings.mode == DistributionSettings.DistributionMode.OnCurve)
                {
                    //Spawn on spline
                    spawnOnCurveJob = new SpawnOnCurve(nativeSplines[splineIndex], splineContainer.transform.localToWorldMatrix, distributionSettings, prefabData, ref spawnPoints);

                    spawnPointJobHandle = spawnOnCurveJob.Schedule();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.InsideArea)
                {
                    if (knotCount < 3)
                    {
                        Debug.LogWarning($"Cannot spawn within Spline #{splineIndex} area. It requires more than 3 knots (has {knotCount}).");
                        return;
                    }

                    //Safety clamping
                    distributionSettings.insideArea.spacing = Mathf.Max(0.5f, distributionSettings.insideArea.spacing);

                    spawnInSplineJob = new SpawnInArea(nativeSplines[splineIndex], splineBounds, distributionSettings, prefabData, ref spawnPoints);

                    spawnPointJobHandle = spawnInSplineJob.Schedule();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.Radial)
                {
                    if (knotCount < 3)
                    {
                        Debug.LogWarning($"Cannot spawn within Spline #{splineIndex}. It requires more than 3 knots (has {knotCount}).");
                        return;
                    }

                    radialJob = new RadialInsideSpline(nativeSplines[splineIndex], splineContainer.transform.localToWorldMatrix, distributionSettings, prefabData, ref spawnPoints);
                    spawnPointJobHandle = radialJob.Schedule();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.Grid)
                {
                    if (knotCount < 3)
                    {
                        Debug.LogWarning($"Cannot spawn within Spline #{splineIndex}. It requires more than 3 knots (has {knotCount}).");
                        return;
                    }

                    gridJob = new SpawnOnGrid(nativeSplines[splineIndex], splineContainer.transform.localToWorldMatrix, distributionSettings, prefabData, ref spawnPoints);
                    spawnPointJobHandle = gridJob.Schedule();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.OnKnots)
                {
                    if (knotCount <= 1)
                    {
                        Debug.LogWarning($"Cannot spawn within Spline #{splineIndex}. It requires more than 1 knot (has {knotCount})");
                        return;
                    }
                    
                    spawnOnKnotsJob = new SpawnOnKnots(nativeSplines[splineIndex], splineContainer, splineIndex, splineContainer.transform.localToWorldMatrix, distributionSettings, prefabData, ref spawnPoints);

                    spawnPointJobHandle = spawnOnKnotsJob.Schedule();
                }

                //Complete and sync
                spawnPointJobHandle.Complete();

                if (distributionSettings.mode == DistributionSettings.DistributionMode.OnCurve)
                {
                    spawnOnCurveJob.Dispose();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.InsideArea)
                {
                    spawnInSplineJob.Dispose();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.Radial)
                {
                    radialJob.Dispose();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.Grid)
                {
                    gridJob.Dispose();
                }
                else if (distributionSettings.mode == DistributionSettings.DistributionMode.OnKnots)
                {
                    spawnOnKnotsJob.Dispose();
                }
            }
            Profiler.EndSample();

            onAfterDistribution?.Invoke(spawnPoints);

            stopWatch.Stop();
            lastDistributionTime = (float)stopWatch.Elapsed.TotalMilliseconds;
            #endregion

            //Masking

            stopWatch.Restart();

            ProcessMasks();

            stopWatch.Stop();

            lastMaskingTime = (float)stopWatch.Elapsed.TotalMilliseconds;

            //Modifier stack
            stopWatch.Restart();

            Profiler.BeginSample($"{PROFILER_PREFIX} Modifier Stack");
            for (int j = 0; j < modifiers.Count; j++)
            {
                if (modifiers[j].enabled == false) continue;

                JobHandle jobHandle = modifiers[j].CreateJob(this, ref spawnPoints);
                jobHandle.Complete();
            }
            Profiler.EndSample();

            onAfterModifiers?.Invoke(spawnPoints);

            stopWatch.Stop();
            lastModifierStackTime = (float)stopWatch.Elapsed.TotalMilliseconds;

            stopWatch.Restart();
            if (spawnObjects)
            {
                Profiler.BeginSample($"{PROFILER_PREFIX} Instantiating");

                SpawnObjects(splineIndex);

                Profiler.EndSample();

            }
            stopWatch.Stop();
            lastInstantiateTime = (float)stopWatch.Elapsed.TotalMilliseconds;

            if (container.InstanceCount == 0)
            {
                //Debug.LogWarning("Spawning amounted to 0 instances");
            }

            DisposeJobs();

            onPostRespawn?.Invoke(this, splineIndex);
        }

        /// <summary>
        /// Adds the GameObject (prefab or not) as a spawnable object
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="probability"></param>
        public SpawnableObject AddObject(GameObject prefab, float probability = 100f)
        {
            SpawnableObject spawnableObject = new SpawnableObject(prefab, probability);
            
            inputObjects.Add(spawnableObject);

            return spawnableObject;
        }
        
        /// <summary>
        /// Removes an object from the list of spawnable objects at the given index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveObject(int index)
        {
            if (inputObjects == null || index < 0 || index >= inputObjects.Count)
            {
                Debug.LogWarning($"RemoveObject: Index {index} is out of range.");
                return;
            }

            inputObjects.RemoveAt(index);
        }

        private void SpawnObjects(int splineIndex)
        {
            #if UNITY_EDITOR
            if (root && EditorUtility.IsPersistent(root))
            {
                Debug.LogWarning($"[Spline Spawner] Cannot spawn objects for Spline #{splineIndex}. The root object ({root.name}) does not exist in the scene.");
                return;
            }
            #endif
            
            var spawnPointsArray = spawnPoints.AsArray();
            int index = 0;
            
            foreach (var point in spawnPointsArray)
            {
                if (point.isValid)
                {
                    #if UNITY_EDITOR
                    if (float.IsNaN(point.position.x))
                    {
                        Debug.LogError($"[Spline Spawner] Spawnpoint at index #{index} is NaN.");
                        continue;
                    }
                    #endif
                    
                    int prefabIndex = point.prefabIndex;
                    SpawnableObject prefab = inputObjects[prefabIndex];
                    
                    containers[splineIndex].SpawnObject(point, prefab.prefab, root, hideInstances);
                    InstanceCount++;
                }
                index++;
            }
        }
        
        private void DisposeJobs()
        {
            for (int j = 0; j < modifiers.Count; j++)
            {
                modifiers[j].Dispose();
            }
            
            prefabData.Dispose();
        }
        
        private void Dispose()
        {
            if(spawnPoints.IsCreated) spawnPoints.Dispose();
            if(prefabData.IsCreated) prefabData.Dispose();
        }
        
        public void AddModifier(Modifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void RemoveModifier(Modifier modifier)
        {
            modifiers.Remove(modifier);
        }
        
        public void RemoveModifier(int index)
        {
            modifiers.RemoveAt(index);
        }

        public Modifier AddModifier(Type type)
        {
            Modifier modifier = Modifier.Create(type);

            modifiers.Add(modifier);

            return modifier;
        }
        
        public Modifier AddModifier(string typeName)
        {
            Type type = Type.GetType(typeName);

            return AddModifier(type);
        }

        public void OverwriteSpawnpoints(NativeList<SpawnPoint> points)
        {
            this.spawnPoints.CopyFrom(points);
        }
        
        public static void WarmUpAllObjectPools(int capacity = 1000)
        {
            SplineSpawner[] instances = FindObjectsByType<SplineSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            for (int i = 0; i < instances.Length; i++)
            {
                instances[i].WarmUpObjectPools(capacity);
            }
        }
        
        /// <summary>
        /// Objects are spawning using Object Pooling, this can lead to a ~20 slower first respawn. Use this function to pre-initialize the object pool for each spline
        /// </summary>
        /// <param name="capacity"></param>
        public void WarmUpObjectPools(int capacity = 1000)
        {
            foreach (SplineInstanceContainer container in containers)
            {
                foreach (var inputObject in inputObjects)
                {
                    if(inputObject.prefab) container.WarmUpObjectPool(inputObject.prefab, capacity);
                }
            }
        }
    }
}